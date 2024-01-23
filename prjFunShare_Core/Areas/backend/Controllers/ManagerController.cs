using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using prjFunShare_Core.Models;
using prjFunShare_Core.Areas.backend.ViewModels;
using System.Text.Json;

namespace prjFunShare_Core.Areas.backend.Controllers
{
    [Area("backend")]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly prjFunShare_Core.Models.FUNShareContext _context;
        private readonly IConfiguration _config;

        public ManagerController(ILogger<ManagerController> logger, prjFunShare_Core.Models.FUNShareContext c, IConfiguration config)
        {
            _logger = logger;
            _context = c;
            _config = config;
        }

        public IActionResult Index()
        {//區分登入身分
         //供應商
         //if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
         //{
         //string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
         //Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);
            Supplier supplier = _context.Supplier.Find(1);
            string sender = "sender";
            Response.Cookies.Append(sender, "S" + supplier.SupplierId.ToString());
            CIndexInfoViewModel vm = new CIndexInfoViewModel();
                vm.Supplier = supplier;
                vm.ProductOnSaleCount = _context.Product.Where(x => x.SupplierId == supplier.SupplierId && x.StatusId == 12).Count();
                vm.ProductCount = _context.Product.Where(x => x.SupplierId == supplier.SupplierId).Count();
                vm.OrderCount = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId).Count();
                vm.OrderAmount = (decimal)_context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId).Select(x => x.ProductDetail.UnitPrice).Sum();
                //月營收成長率 =（當月營收 – 上月營收）÷ 上月營收 x 100%
                var lastMonthSales = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                             && x.Order.OrderTime.Year == DateTime.Now.Year && x.Order.OrderTime.Month == (DateTime.Now.Month - 1))
                                                                             .Select(x => x.ProductDetail.UnitPrice).Sum();
                var thisMonthSales = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                 && x.Order.OrderTime.Year == DateTime.Now.Year && x.Order.OrderTime.Month == DateTime.Now.Month)
                                                                 .Select(x => x.ProductDetail.UnitPrice).Sum();
                if (lastMonthSales != 0)
                    vm.MoM = (decimal)((thisMonthSales - lastMonthSales) / lastMonthSales);
                else
                    vm.MoM = 0;
                //年營收成長率
                var lastYearSales = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                                                        && x.Order.OrderTime.Year == DateTime.Now.Year - 3) //倒出來的數字比較好看
                                                                                                                .Select(x => x.ProductDetail.UnitPrice).Sum();
                var thisYearSales = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                                                        && x.Order.OrderTime.Year == DateTime.Now.Year - 2)
                                                                                                                .Select(x => x.ProductDetail.UnitPrice).Sum();
                if (lastYearSales != 0)
                    vm.YoY = (decimal)((thisYearSales - lastYearSales) / lastYearSales);
                else
                    vm.YoY = 0;

                return View(vm);
            //}
            //// Admin
            //else if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMIN))
            //{
            //    CIndexInfoViewModel vm = new CIndexInfoViewModel();
            //    vm.ProductOnSaleCount = _context.Product.Where(x => x.StatusId == 12).Count();
            //    vm.ProductCount = _context.Product.Count();
            //    vm.OrderCount = _context.OrderDetail.Count();
            //    vm.OrderAmount = (decimal)_context.OrderDetail.Select(x => x.ProductDetail.UnitPrice).Sum();
            //    //月營收成長率 =（當月營收 – 上月營收）÷ 上月營收 x 100%
            //    var lastMonthSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year
            //                                                                                                    && x.Order.OrderTime.Month == (DateTime.Now.Month - 1))
            //                                                                 .Select(x => x.ProductDetail.UnitPrice).Sum();
            //    var thisMonthSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year
            //                                                                                                   && x.Order.OrderTime.Month == DateTime.Now.Month)
            //                                                                 .Select(x => x.ProductDetail.UnitPrice).Sum();
            //    if (lastMonthSales != 0)
            //        vm.MoM = (decimal)((thisMonthSales - lastMonthSales) / lastMonthSales);
            //    else
            //        vm.MoM = 0;
            //    //年營收成長率
            //    var lastYearSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year - 3) //倒出來的數字比較好看
            //                                                                                                    .Select(x => x.ProductDetail.UnitPrice).Sum();
            //    var thisYearSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year - 2)
            //                                                                 .Select(x => x.ProductDetail.UnitPrice).Sum();
            //    if (lastYearSales != 0)
            //        vm.YoY = (decimal)((thisYearSales - lastYearSales) / lastYearSales);
            //    else
            //        vm.YoY = 0;

            //    return View(vm);
            //}

            //未登入
            //return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            //管理員登入
            string adminId = _config["Admin:Id"];
            string adminPassword = _config["Admin:Password"];

            if (adminId.Equals(vm.txtAccount) && adminPassword.Equals(vm.txtPassword))
            {
                HttpContext.Session.SetString(CDictionary.SK_LOGINED_ADMIN, "Admin");
                // 刪除廠商登入資料
                HttpContext.Session.Remove(CDictionary.SK_LOGINED_SUPPLIER);

                return RedirectToAction("Index");
            }
            //廠商登入
            Supplier sup = _context.Supplier.FirstOrDefault(
                t => t.TaxId.Equals(vm.txtAccount) && t.Password.Equals(vm.txtPassword));
            if (sup != null && sup.Password.Equals(vm.txtPassword))
            {
                string json = JsonSerializer.Serialize(sup);
                HttpContext.Session.SetString(CDictionary.SK_LOGINED_SUPPLIER, json);
                // 刪除管理員登入資料
                HttpContext.Session.Remove(CDictionary.SK_LOGINED_ADMIN);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOGINED_SUPPLIER);
            HttpContext.Session.Remove(CDictionary.SK_LOGINED_ADMIN);
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            IEnumerable<City> cities = _context.City.ToList();
            ViewBag.Cities = new SelectList(cities, "CityId", "CityName");
            return View();
        }
        [HttpPost]
        public IActionResult Register(Supplier supl)
        {
            City selectedCity = _context.City.Find(supl.CityId);

            if (selectedCity != null)
            {
                supl.StatusId = 16;
                supl.City = selectedCity;
            }
            _context.Supplier.Add(supl);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
