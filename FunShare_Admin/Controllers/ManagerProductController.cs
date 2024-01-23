using FunShare_Admin.Models;
using FunShare_Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FunShare_Admin.Controllers
{
    public class ManagerProductController : Controller
    {
        private readonly FUNShareContext _context;
        private IWebHostEnvironment _enviro = null;
        IEnumerable<Product> datas = null;
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
            
            ProductViewModel model = new ProductViewModel();
            return View(model);
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
            return Content("Edited Product.");
          //  return RedirectToAction("List");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List(CKeywordViewModel vm)
        {

            if (string.IsNullOrEmpty(vm.txtKeyword))
            {
                datas = from p in _context.Product.Include(s => s.Supplier).Include(s => s.Status).Include(t => t.Interval).Include(d => d.ProductDetail)
                        select p;
            }
            else
                datas = from p in _context.Product.Include(s => s.Supplier).Include(s => s.Status).Where(p => p.ProductName.ToUpper().Contains(vm.txtKeyword.ToUpper())
                   || p.Supplier.SupplierName.ToUpper().Contains(vm.txtKeyword.ToUpper())
                   || p.Status.Description.ToUpper().Contains(vm.txtKeyword.ToUpper())
                   )
                        select p;
                   
            return View(datas);

            ////to do:在 ASP.NET MVC 應用程式中，使用 Entity Framework 新增排序、篩選和分頁 https://learn.microsoft.com/zh-tw/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            //var data = from p in _context.Product.Include(s => s.Supplier).Include(s => s.Status).Include(t => t.Interval).Include(d => d.ProductDetail).Include(p=>p.ImageList)
            //           select p;

            //ViewBag.IntervalId = new SelectList(_context.IntervalList, "IntervalId", "IntervalDescription");

            //if (!string.IsNullOrEmpty(search.textProductName))
            //{
            //    data = data.Where(s => s.ProductName.ToUpper().Contains(search.textProductName.ToUpper()));
            //}
            //if (!string.IsNullOrEmpty(search.textSupplierName))
            //{
            //    data = data.Where(s => s.Supplier.SupplierName.ToUpper().Contains(search.textSupplierName.ToUpper()));
            //}
            //if (search.textUnitPrice!=null)
            //{
            //    data = data.Where(s => s.ProductDetail.Max().UnitPrice< search.textUnitPrice * 10000);
            //}
            //return View(data);
        }

        public IActionResult ChangeStatustoDiscontinue(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            Product c = _context.Product.Find(id);
            c.StatusId = 13;            
            _context.Update(c);
            _context.SaveChanges();

            return RedirectToAction("List");
        }
        public IActionResult ChangeStatustoLaunch(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            Product c = _context.Product.Find(id);
            c.StatusId = 12;
            _context.Update(c);
            _context.SaveChanges();

            return RedirectToAction("List");
        }
        public IActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            ViewBag.AgeId = new SelectList(_context.Age, "AgeId", "Grade");
            ViewBag.IntervalId = new SelectList(_context.IntervalList, "IntervalId", "IntervalDescription");
            ViewBag.StatusId = new SelectList(_context.Status.Where(s=>s.StatusType.Equals("Product")), "StatusId", "Description");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product p)
        {
            _context.Product.Add(p);
            _context.SaveChanges();
            //return Content("Created Product");
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

       
    }
}
