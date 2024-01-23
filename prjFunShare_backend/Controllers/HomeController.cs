using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using prjFunShare_backend.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FUNShareContext _context;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, FUNShareContext c, IConfiguration config)
        {
            _logger = logger;
            _context = c;
            _config = config;
        }
        public IActionResult Index()
        {
            //區分登入身分
            //供應商
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);
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
                var lastYearSales = _context.OrderDetail.Where(x =>x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                                                        && x.Order.OrderTime.Year == DateTime.Now.Year-1) //倒出來的數字比較好看
                                                                                                                .Select(x => x.ProductDetail.UnitPrice).Sum();
                var thisYearSales = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId
                                                                                                        && x.Order.OrderTime.Year == DateTime.Now.Year)
                                                                                                                .Select(x => x.ProductDetail.UnitPrice).Sum();
                if (lastYearSales != 0)
                    vm.YoY = (decimal)((thisYearSales - lastYearSales) / lastYearSales);
                else
                    vm.YoY = 0;

                //var SaleClassStockTotal = _context.ProductDetail.Where(x => x.StatusId == 9).Sum(x => x.Stock ?? 0);
                //var OrderClassCount = _context.OrderDetail.Count();
                //vm.CSS = (decimal)(OrderClassCount / (decimal)SaleClassStockTotal) * 100;
                
                //產品Top3
                vm.ProductName = _context.OrderDetail.Where(x => x.ProductDetail.Product.SupplierId == supplier.SupplierId)
                .GroupBy(x => x.ProductDetail.Product.ProductName)
                .Select(x => new { x.Key, count = x.Count() })
                .OrderByDescending(x => x.count)
                .Select(x => x.Key)
                .Take(3).ToArray();

                return View(vm);
            }
            // Admin
            else if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMIN))
            {
                CIndexInfoViewModel vm = new CIndexInfoViewModel();
                vm.ProductOnSaleCount = _context.Product.Where(x => x.StatusId == 12).Count();
                vm.ProductCount = _context.Product.Count();
                vm.OrderCount = _context.OrderDetail.Count();
                vm.OrderAmount = (decimal)_context.OrderDetail.Select(x => x.ProductDetail.UnitPrice).Sum();
                //月營收成長率 =（當月營收 – 上月營收）÷ 上月營收 x 100%
                var lastMonthSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year
                                                                                                                && x.Order.OrderTime.Month == (DateTime.Now.Month - 1))
                                                                             .Select(x => x.ProductDetail.UnitPrice).Sum();
                var thisMonthSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year
                                                                                                               && x.Order.OrderTime.Month == DateTime.Now.Month)
                                                                             .Select(x => x.ProductDetail.UnitPrice).Sum();
                if (lastMonthSales != 0)
                    vm.MoM = (decimal)((thisMonthSales - lastMonthSales) / lastMonthSales);
                else
                    vm.MoM = 0;
                //年營收成長率
                var lastYearSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year-1) //倒出來的數字比較好看
                                                                                                                .Select(x => x.ProductDetail.UnitPrice).Sum();
                var thisYearSales = _context.OrderDetail.Where(x => x.Order.OrderTime.Year == DateTime.Now.Year)
                                                                             .Select(x => x.ProductDetail.UnitPrice).Sum();
                if (lastYearSales != 0)
                    vm.YoY = (decimal)((thisYearSales - lastYearSales) / lastYearSales);
                else
                    vm.YoY = 0;

                //var SaleClassStockTotal = _context.ProductDetail.Where(x => x.StatusId == 9).Sum(x => x.Stock ?? 0);
                //var OrderClassCount = _context.OrderDetail.Count();
                //vm.CSS = (decimal)(OrderClassCount / (decimal)SaleClassStockTotal) * 100;

                //廠商總數
                vm.SupplierCount = _context.Supplier.Count();
                //待審核
                vm.SupplierToBeApproved = _context.Supplier.Where(x => x.StatusId == 16).Count();

                return View(vm);
        }

            //未登入
             return RedirectToAction("Login");
    }
        public IActionResult Login()
        {
            return PartialView(nameof(Login));
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            //管理員登入
            string adminId = _config["Admin:Id"];
            string adminPassword = _config["Admin:Password"];

            if(adminId.Equals(vm.txtAccount) && adminPassword.Equals(vm.txtPassword)){
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
        public IActionResult Register(Supplier supl,IFormFile file)
        {
            
            FileStream stream = new FileStream("./wwwroot/img/NoImage.jpg", FileMode.Open);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            supl.LogoImage = ms.ToArray();


            City selectedCity = _context.City.Find(supl.CityId);       
            if (selectedCity != null)
            {
                supl.StatusId = 16;
                supl.City = selectedCity;
            }
            _context.Supplier.Add(supl);
            _context.SaveChanges();
            return Content("註冊成功");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}