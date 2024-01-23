using prjFunShare_backend.Models.ManagerProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class ManagerProductController : Controller
    {
        private readonly FUNShareContext _context;
        private IWebHostEnvironment _enviro = null;
        public ManagerProductController(IWebHostEnvironment p, FUNShareContext c)
        {
            _enviro = p;
            _context = c;
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            Product prod = _context.Product.Find(id);
            if (prod == null)
                return RedirectToAction("List");
            ProductWrap prodWrap = new ProductWrap();
            prodWrap.product = prod;
            return View(prodWrap);
        }
        [HttpPost]
        public ActionResult Edit(ProductWrap pIn)
        {
            Product pDb = _context.Product.Find(pIn.ProductId);
            if (pDb != null)
            {
                //if (pIn.photo != null)
                //{
                //    string photoName = Guid.NewGuid().ToString() + ".jpg";
                //    pDb.FImagePath = photoName;
                //    pIn.photo.CopyTo(new FileStream(
                //        _enviro.WebRootPath + "/images/" + photoName,
                //        FileMode.Create));
                //}
                pDb.ProductName = pIn.ProductName;
                pDb.ProductIntro = pIn.ProductIntro;
               // pDb.SupplierId = pIn.ProductId;               
                pDb.Stock = pIn.Stock;
                pDb.ReleasedTime = pIn.ReleasedTime;
                pDb.Interval = pIn.Interval;
                pDb.Times = pIn.Times;
                pDb.Note = pIn.Note;
                pDb.StatusId = pIn.StatusId;
                pDb.IsClass = pIn.IsClass;
                pDb.Commision = pIn.Commision;
                pDb.AgeId = pIn.AgeId;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult Index()
        {
            return View();
        }
        //出席狀態
        public IActionResult DetailList(int pid, CKeyword kw, int id)
        {

            var result = _context.OrderDetail
            .Where(s => s.ProductDetailId == pid &&s.ProductDetail.ProductId== id)//將時間帶到裡面來
           .Include(o => o.Order)
           .Include(pd => pd.ProductDetail)
           .ThenInclude(p => p.Product)
           .ThenInclude(pdc => pdc.ProductCategories)
           .ThenInclude(p => p.Product)
           .ThenInclude(s => s.Supplier)
           .ThenInclude(st => st.Status)
           .Include(c => c.Member)
           .AsQueryable();
            if (kw.txtKeyword != null)
            {
                var keyword = kw.txtKeyword.ToUpper();
                result = result.Where(p =>
                    p.ProductDetail.Product.ProductName.ToUpper().Contains(keyword) ||
                    p.Status.Description.ToUpper().Contains(keyword) ||
                    p.Order.Status.Description.ToUpper().Contains(keyword) ||
                    p.Member.PhoneNumber.ToUpper().Contains(keyword) ||
                    p.Member.Email.ToUpper().Contains(keyword) ||
                    p.Member.Name.ToUpper().Contains(keyword)
                );
            }
            var datas = result
                .Select(q => new CProductAttend
            {
                FProductDetail_ID = q.ProductDetailId,
                FCustomer_Name = q.Member.Name,
                FCustomer_Email = q.Member.Email,
                FCustomer_Phone = q.Member.PhoneNumber,
                FProduct_Name = q.ProductDetail.Product.ProductName,
                FProduct_UnitPrice = q.ProductDetail.UnitPrice,
                FOrderPay_Status = q.Order.Status.Description,
                FOrderDetail_ID = q.OrderDetailId,
                FEndtime = q.ProductDetail.EndTime,
                Fproduct_ID = q.ProductDetail.ProductId,
                FProduct_IsAttend = q.IsAttend,
            }) 
            .OrderByDescending(p => p.FProductDetail_ID);
            return View(datas);
        }

        public IActionResult CheckCustomerAttend(int id, bool FProduct_IsAttend, int productdetailid)
        {

            int prodId = _context.ProductDetail.Find(productdetailid).ProductId;
            OrderDetail orderDetail = _context.OrderDetail.FirstOrDefault(p => p.OrderDetailId == id);
            if (orderDetail != null)
            {
                if (!orderDetail.IsAttend.HasValue)
                {
                    orderDetail.IsAttend = true; // 如果是空值，設為出席
                }
                else
                {
                    orderDetail.IsAttend = !orderDetail.IsAttend.Value;
                }
                _context.SaveChanges();
            }
            return RedirectToAction("DetailList", "ManagerProduct", new {id= prodId, pid = productdetailid });//對應到產品ID跟明細ID
        }
        public IActionResult List(CKeyword kw)
        {
            //供應商
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var query = _context.Product
            .Include(pd => pd.ProductDetail)
            .Include(s => s.Supplier)
            .Include(st => st.Status)
            .Where(p=>p.SupplierId==supplier.SupplierId)
            .AsQueryable();
                if (kw.txtKeyword != null)
                {
                    var keyword = kw.txtKeyword.ToUpper();
                    query = query.Where(o =>
                        o.ProductName.ToUpper().Contains(keyword) ||
                        o.Status.Description.ToUpper().Contains(keyword) ||
                        //o.ProductIntro.ToUpper().Contains(keyword) ||
                        o.Age.Grade.ToUpper().Contains(keyword) ||
                        o.Age.Grade.ToUpper().Contains(keyword) ||
                        o.Supplier.SupplierName.ToUpper().Contains(keyword)
                    );
                }
                var datas = query.Select(q => new CProduct
                {
                    FProductId = q.ProductId,
                    FProductName = q.ProductName,
                    FProductIntro = q.ProductIntro,
                    FSupplierName = q.Supplier.SupplierName,
                    FAgeId = q.Age.Grade,
                    FLevel = q.Level,
                    FReleasedTime = q.ReleasedTime,
                    FTimes = q.Times,
                    FStatusDescription = q.Status.Description,
                    FIsClass = q.IsClass,
                    FCommision = q.Commision,
                    FProductSale_Status = q.Status.Description,

                    //明細部分
                    CProductDetails = q.ProductDetail.Select(x => new CProductDetail
                    {
                        // 明細部分
                        FProductId = x.Product.ProductId,
                        FProductDetailId = x.ProductDetailId,
                        FBeginTime = x.BeginTime,
                        FEndTime = x.EndTime,
                        FDistrictName = x.District.DistrictName,
                        FDetailStatusDescription = x.Status.Description,
                        FAddress = x.Address,
                        FStock = x.Stock,
                        FUnitPrice = x.UnitPrice,
                        FDealine = x.Dealine,
                        FProductClass_Status = q.Status.Description,
                        FSales = x.OrderDetail.Count(),
                    }).ToList(),
                });
                return View(datas);
            }
            else { 
                //改成用product不然會抓到錯的資料 
                var query = _context.Product
          .Include(pd => pd.ProductDetail)
          .Include(s => s.Supplier)
          .Include(st => st.Status)
          .AsQueryable();
           if (kw.txtKeyword != null)
            {
                var keyword = kw.txtKeyword.ToUpper();
                query = query.Where(o =>
                    o.ProductName.ToUpper().Contains(keyword) ||
                    o.Status.Description.ToUpper().Contains(keyword) ||
                    //o.ProductIntro.ToUpper().Contains(keyword) ||
                    o.Age.Grade.ToUpper().Contains(keyword) ||
                    o.Age.Grade.ToUpper().Contains(keyword) ||
                    o.Supplier.SupplierName.ToUpper().Contains(keyword)
                );
            }
            var datas = query.Select(q => new CProduct
            {
                FProductId = q.ProductId,
                FProductName = q.ProductName,
                FProductIntro = q.ProductIntro,
                FSupplierName = q.Supplier.SupplierName,
                FAgeId = q.Age.Grade,
                FLevel = q.Level,
                FReleasedTime = q.ReleasedTime,
                FTimes = q.Times,
                FStatusDescription = q.Status.Description,
                FIsClass = q.IsClass,
                FCommision = q.Commision,
                FProductSale_Status = q.Status.Description,

                //明細部分
                CProductDetails = q.ProductDetail.Select(x => new CProductDetail
                {
                    // 明細部分
                    FProductId = x.Product.ProductId,
                    FProductDetailId = x.ProductDetailId,
                    FBeginTime = x.BeginTime,
                    FEndTime = x.EndTime,
                    FDistrictName = x.District.DistrictName,
                    FDetailStatusDescription = x.Status.Description,
                    FAddress = x.Address,
                    FStock = x.Stock,
                    FUnitPrice = x.UnitPrice,
                    FDealine = x.Dealine,
                    FProductClass_Status = q.Status.Description,
                    FSales = x.OrderDetail.Count(),
                }).ToList(),
            });
            return View(datas);
            }
        }

        public IActionResult DeleteDetail(int? pid)
        {
            if (pid != null)
            {
                ProductDetail prod = _context.ProductDetail.Find(pid);
                if (prod != null)
                {
                    _context.ProductDetail.Remove(prod);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        //    public IActionResult List()
        //{
        //    //to do:在 ASP.NET MVC 應用程式中，使用 Entity Framework 新增排序、篩選和分頁 https://learn.microsoft.com/zh-tw/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
        //    //var data = _context.Product.Include(s => s.Supplier).Include(s => s.Status).Include(t => t.Interval).Include(d => d.ProductDetail);
        //    //ViewBag.IntervalId = new SelectList(_context.IntervalList, "IntervalId", "IntervalDescription");
        //    //return View(data);
        //}
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Product p)
        {
            _context.Product.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Create","ManagerProductDetail",p.ProductId);
        }
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                Product prod= _context.Product.Find(id);
                if (prod != null)
                {
                    _context.Product.Remove(prod);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }


        public IActionResult ChangeStatustoDiscontinue(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            Product c = _context.Product.Find(id);
            c.StatusId = 13;
            //_context.Update(c);
            _context.SaveChanges();

            return RedirectToAction("List");
        }
        public IActionResult ChangeStatustoLaunch(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            Product c = _context.Product.Find(id);
            c.StatusId = 12;
            c.ReleasedTime = DateTime.Now;
            _context.Update(c);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

    }
}
