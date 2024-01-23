using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using prjFunShare_backend.ViewModels;
using System.Drawing;

namespace prjFunShare_backend.Controllers
{
    
    public class ManagerSupplierController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerSupplierController(FUNShareContext c)
        {
            _context = c;
        }
        public IActionResult List(CkeywordViewModelInSupplier inf, int? page, int? itemsPerPage)
        {
            int itemsPerPageValue = itemsPerPage ?? 15;//每頁顯示資料
            int pageNumber = page ?? 1;//未提供預設為1
            IEnumerable<Supplier> datas = null;
            if (string.IsNullOrEmpty(inf.txtKeyword))
            {
                datas = _context.Supplier.Include(s => s.City).Include(s => s.Status)
                    .OrderBy(s => s.SupplierId)
                    .Skip((pageNumber - 1) * itemsPerPageValue)
                    .Take(itemsPerPageValue)
                    .ToList();
            }
            else
            {
                datas = _context.Supplier.Include(s => s.City).Include(s => s.Status).Where(s => s.TaxId.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.SupplierName.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.Email.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.SupplierPhone.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.Address.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.Fax.ToUpper().Contains(inf.txtKeyword.ToUpper())
                || s.Status.Description.Contains(inf.txtKeyword)
                || s.City.CityName.Contains(inf.txtKeyword))
                .OrderBy(s => s.SupplierId)
                .Skip((pageNumber - 1) * itemsPerPageValue)
                .Take(itemsPerPageValue)
                .ToList();
            }
            //todo page的功能有點失敗還要查原因
            ViewBag.CurrentPage = pageNumber; // 傳遞當前頁數到view
            ViewBag.TotalPages = Math.Ceiling((double)_context.Supplier.Count() / itemsPerPageValue); // 傳遞總頁數到view
            return View(datas);
        }
        public IActionResult CheckList(CkeywordViewModelInSupplier inf)
        {
            IEnumerable<Supplier> datas = null;
            if (string.IsNullOrEmpty(inf.txtKeyword))
            {
                datas = _context.Supplier.Include(s => s.Status).Where(s => s.StatusId >=15 && s.StatusId <= 16)
                    .OrderBy(s => s.SupplierId).ToList();
            }
            return View(datas);
        }

        public IActionResult Create()
        {
            IEnumerable<City> cities = _context.City.ToList();
            IEnumerable<Status> statuses = _context.Status.Where(s => s.StatusId >= 14 && s.StatusId <= 16).ToList();
            ViewBag.Cities = new SelectList(cities, "CityId", "CityName");
            ViewBag.Status = new SelectList(statuses, "StatusId", "Description");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Supplier s)
        {
            City selectedCity = _context.City.Find(s.CityId);
           // Status selectedStatus = _context.Status.Find(s.StatusId);
            if (selectedCity != null)
            {
                s.StatusId = 16;
                s.City = selectedCity; 
            }
            _context.Supplier.Add(s);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            // id = 1;
            if (id == null)
                return RedirectToAction("List");
            //FunshareContext db = new FunshareContext();
            Supplier supplier = _context.Supplier.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
            {
                return RedirectToAction("List");
            }
            // 取得城市和狀態的部分
            IEnumerable<City> cities = _context.City.ToList();
            IEnumerable<Status> statuses = _context.Status.Where(s => s.StatusId >= 14 && s.StatusId <= 16).ToList();
            // 將城市和狀態選項放入 ViewBag
            ViewBag.Cities = new SelectList(cities, "CityId", "CityName", supplier.CityId);
            ViewBag.Statuses = new SelectList(statuses, "StatusId", "Description", supplier.StatusId);
            return View(supplier);
        }
        [HttpPost]
        public IActionResult Edit(Supplier pln)
        {
            Supplier sDB = _context.Supplier.FirstOrDefault();
            if (sDB != null)
            {
                sDB.SupplierName = pln.SupplierName;
                sDB.SupplierPhone = pln.SupplierPhone;
                sDB.Email = pln.Email;
                sDB.Fax = pln.Fax;
                sDB.Password = pln.Password;
                sDB.ContactPerson = pln.ContactPerson;
                sDB.Description = pln.Description;
                sDB.Address = pln.Address;
                sDB.CityId = pln.CityId;
                sDB.Status = pln.Status;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        //小C
        public IActionResult CheckSupplierApproved(int id)
        {
            Supplier supplier = _context.Supplier.FirstOrDefault(p => p.SupplierId == id);
            if (supplier != null)
            {
                supplier.StatusId = 14;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult CheckSuppliersBulkApproved(int[] SupplierId)
        {
            if (SupplierId == null)
                return RedirectToAction("List");
            int checkid;
            for(int i =0;i< SupplierId.Length;i++)
            {
                checkid = SupplierId[i];
                Supplier supplier = _context.Supplier.Where(p => p.SupplierId == checkid).FirstOrDefault();
                if (supplier != null)
                {
                    supplier.StatusId = 14;
                    _context.Update(supplier);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        public IActionResult CheckSupplierReject(int? id)
        {
            if (id != null)
            {
                Supplier supplier = _context.Supplier.FirstOrDefault(p => p.SupplierId == id);
                if (supplier != null)
                {
                    supplier.StatusId = 15;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                Supplier supplier = _context.Supplier.FirstOrDefault(p => p.SupplierId == id);
                if (supplier != null)
                {
                    _context.Supplier.Remove(supplier);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
