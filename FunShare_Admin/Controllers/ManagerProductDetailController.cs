using Microsoft.AspNetCore.Mvc;
using FunShare_Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FunShare_Admin.Controllers
{
    public class ManagerProductDetailController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerProductDetailController( FUNShareContext c)
        {           
            _context = c;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Create(int pid)
        {
            ViewBag.StatusId = new SelectList(_context.Status.Where(s => s.StatusType.Equals("ProductDetaill")), "StatusId", "Description");
            ViewData["DistrictId"] = new SelectList(_context.District, "DistrictId", "DistrictName");
            ProductDetail detail = new ProductDetail();
            detail.ProductId= pid;
            return View(detail);
        }
        [HttpPost]
        public IActionResult Create(ProductDetail p)
        {
            _context.ProductDetail.Add(p);
            _context.SaveChanges();
            //return Content("Created ProductDetais.");
            return RedirectToAction("Create", "ManagerPhoto", p.ProductId);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.StatusId2 = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Product_Detail")), "StatusId", "Description");
            ViewBag.DistrictId = new SelectList(_context.District, "DistrictId", "DistrictName");
            ProductDetail p = _context.ProductDetail.Find(id);
            ProductDetailWrap pd = new ProductDetailWrap();
            pd.productDetail = p;
            //ProductDetail pd = _context.ProductDetail.Find(id);
            return View(pd);
        }
        [HttpPost]
        public IActionResult Edit(ProductDetail p)
        {
            ProductDetail productDetail = _context.ProductDetail.Find(p.ProductDetailId);
            productDetail.BeginTime = p.BeginTime;
            productDetail.EndTime= p.EndTime;
            productDetail.DistrictId= p.DistrictId;
            productDetail.StatusId= p.StatusId;
            productDetail.Address= p.Address;
            productDetail.Stock = p.Stock;
            productDetail.UnitPrice= p.UnitPrice;
            productDetail.Dealine= p.Dealine;
            productDetail.ClassId= p.ClassId;
            _context.SaveChanges();
            return RedirectToAction("Edit", "SuperProduct",new { id = p.ProductId });

        }
        public IActionResult List(int pid)
        {           
            var data = _context.ProductDetail.Where(p=>p.ProductId==pid);
            return View(data);
        }
    }
}
