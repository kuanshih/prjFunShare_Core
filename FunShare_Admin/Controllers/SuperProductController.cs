using FunShare_Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace FunShare_Admin.Controllers
{
    public class SuperProductController : Controller
    {
        private readonly FUNShareContext _context;
        private IWebHostEnvironment _enviro = null;
        public SuperProductController(IWebHostEnvironment p, FUNShareContext c)
        {
            _enviro = p;
            _context = c;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            ViewBag.AgeId = new SelectList(_context.Age, "AgeId", "Grade");
            ViewBag.IntervalId = new SelectList(_context.IntervalList, "IntervalId", "IntervalDescription");
            ViewBag.StatusId1 = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Product")), "StatusId", "Description");
            ViewBag.StatusId2 = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Product_Detail")), "StatusId", "Description");
            ViewBag.DistrictId = new SelectList(_context.District, "DistrictId", "DistrictName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(TProduct T)
        {
            //save info to product
            Product p= new Product();
            p.ProductName = T.ProductName;
            p.ProductIntro = T.ProductIntro;
            p.SupplierId= T.SupplierId;
            p.AgeId= T.AgeId;            
            p.Level = T.Level;           
            p.ReleasedTime= T.ReleasedTime;
            p.Times= T.Times;
            p.IntervalId = T.IntervalId;
            p.Note= T.Note;
            p.StatusId= T.StatusId;
            p.IsClass = T.IsClass;
            p.Commision= T.Commision;
            p.Features= T.Features;
            _context.Product.Add(p);           
            _context.SaveChanges();

           p.ProductId= _context.Product.OrderBy(p=>p.ProductId).LastOrDefault().ProductId;



            //save to productDetails
            //List<ProductDetail> details = new List<ProductDetail>();
            //details = (List<ProductDetail>)T.ProductDetail;
            //if (details != null)
            //{
            //    foreach (ProductDetail item in details)
            //     {
            //     
            for(int i=0; i<T.BeginTime.Length; i++) {
            ProductDetail item = new ProductDetail();
            item.ProductId = p.ProductId;
            item.BeginTime = T.BeginTime[i];
            item.EndTime = T.EndTime[i];
            //item.District = T.District;
            item.StatusId = 9;
            item.Address = T.Address[i];
            item.Stock = T.Stock[i];
            item.UnitPrice = T.UnitPrice[i];
            item.Dealine = T.Dealine[i];
           // item.ClassId = T.ClassId[i];
            //details.Add(item);
            _context.ProductDetail.Add(item);
            _context.SaveChanges();
            }
            // }
            //  }


            //save to imagelist

            string photoName = Guid.NewGuid().ToString() + ".jpg";
                ImageList img = new ImageList();
                img.ProductId = p.ProductId;
                img.ImagePath = photoName;
            

                FileStream fs = new FileStream(_enviro.WebRootPath + "/img/product/" + photoName, FileMode.Create);
                T.photo.CopyTo(fs);
                _context.ImageList.Add(img);
                _context.SaveChanges();
          

            //long size = files.Sum(f => f.Length);
            //foreach (var formFile in files)
            //{
            //    if (formFile.Length > 0)
            //    {
            //        //var filePath = Path.GetTempFileName();
            //        var filePath = Path.Combine(_enviro.WebRootPath, "/img/product/", Path.GetRandomFileName());

            //        using (var stream = System.IO.File.Create(filePath))
            //        {
            //            await formFile.CopyToAsync(stream);
            //        }
            //    }
            //}
            //return Ok(new { count = files.Count, size });

            return RedirectToAction("List","ManagerProduct");

        }

        public IActionResult Edit(int id)
        {
            ViewBag.SupplierId = new SelectList(_context.Supplier, "SupplierId", "SupplierName");
            ViewBag.AgeId = new SelectList(_context.Age, "AgeId", "Grade");
            ViewBag.IntervalId = new SelectList(_context.IntervalList, "IntervalId", "IntervalDescription");
            ViewBag.StatusId1 = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Product")), "StatusId", "Description");
            ViewBag.StatusId2 = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Product_Detail")), "StatusId", "Description");
            ViewBag.DistrictId = new SelectList(_context.District, "DistrictId", "DistrictName");
            TProduct T=new TProduct();
            T.product = _context.Product.Find(id);
            T.ProductDetail= _context.ProductDetail.Where(p=>p.ProductId==id).ToList();
            T.ImageList = _context.ImageList.FirstOrDefault(p => p.ProductId == id);    
            return View(T);
        }
        [HttpPost]
        public IActionResult Edit(TProduct T)
        {
            //save info to product

            Product p = _context.Product.Find(T.ProductId);
            p.ProductName = T.ProductName;
            p.ProductIntro = T.ProductIntro;
            p.SupplierId = T.SupplierId;
            p.AgeId = T.AgeId;
            p.Level = T.Level;
            
            p.ReleasedTime = T.ReleasedTime;
            p.Times = T.Times;
            p.IntervalId = T.IntervalId;
            p.Note = T.Note;
            p.StatusId = T.StatusId;
            p.IsClass = T.IsClass;
            p.Commision = T.Commision;
            p.Features = T.Features;
            //_context.ProductDetail.Add(T.productDetail);
            //_context.ImageList.Add(T.ImageList);
            _context.SaveChanges();

            //List<ProductDetail> productDetails = (List<ProductDetail>)_context.ProductDetail.Where(p => p.ProductId == T.ProductId);

            //foreach (ProductDetail d in productDetails)
            //{
            //    d = _context.ProductDetail.Find(T.ProductDetailId);
            //    d.BeginTime = T.BeginTime.
            //    d.EndTime = T.EndTime;
            //    d.DistrictId= T.DistrictId;
            //    d.StatusId = T.StatusId;
            //    d.Address = T.Address;
            //    d.UnitPrice = T.UnitPrice;
            //    d.Dealine = T.Dealine;
            //    d.ClassId = T.ClassId;
            //    _context.SaveChanges();
            //}

            if (T.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                ImageList img = _context.ImageList.Find(T.ImageId);
                img.ImagePath = photoName;

                FileStream fs = new FileStream(_enviro.WebRootPath + "/img/product/" + photoName, FileMode.Create);
                T.photo.CopyTo(fs);
                _context.SaveChanges();
            }

            return RedirectToAction("List", "ManagerProduct");
        }

    }
}
