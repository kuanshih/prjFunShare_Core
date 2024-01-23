using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_Core.Models;
using prjFunShare_Core.Areas.backend.ViewModels;

namespace prjFunShare_Core.Areas.backend.Controllers
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
    }
}
