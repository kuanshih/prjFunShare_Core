using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using prjFunShare_backend.ViewModels;
using Microsoft.Extensions.Hosting;




namespace prjFunShare_backend.Controllers
{
    public class ManagerCustomerController : Controller
    {
        private readonly FUNShareContext _context;
        IEnumerable<CustomerInfomation> datas = null;
        private readonly IWebHostEnvironment _host;
        public ManagerCustomerController(FUNShareContext c, IWebHostEnvironment host)
        {
            _context = c;
            _host = host;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List(CKeywordViewModel vm)
        {
            //to do:在 ASP.NET MVC 應用程式中，使用 Entity Framework 新增排序、篩選和分頁 https://learn.microsoft.com/zh-tw/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            
           
            if (string.IsNullOrEmpty(vm.txtKeyword))
            {
                datas = from cust in _context.CustomerInfomation.Include(d => d.District).Include(s => s.Status)
                        select cust;
            }
            else
                datas = _context.CustomerInfomation.Include(d => d.District).Include(s => s.Status).Where(cust => cust.Name.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    || cust.PhoneNumber.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    || cust.Email.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    || cust.ResidentId.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    || cust.District.DistrictName.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    || cust.Status.Description.ToUpper().Contains(vm.txtKeyword.ToUpper())
                    );
            return View(datas);
        }

        public IActionResult ReverseStatus(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            CustomerInfomation c = _context.CustomerInfomation.Find(id);
            c.StatusId = 1;
            c.SuspensionReason = "";
            _context.Update(c);
            _context.SaveChanges();

            return RedirectToAction("List");
        }
        public IActionResult ChangeStatus(int id)
        {
            if (id == null)
                return RedirectToAction("List");
            CustomerInfomation cust = _context.CustomerInfomation.Find(id);
            CustomerInfomationWrap custWrap = new CustomerInfomationWrap();
            custWrap.CustomerInfomation = cust;
            return View(custWrap);
        }
        [HttpPost]
        public IActionResult ChangeStatus(CustomerInfomationWrap cust)
        {

            if (cust != null)
            {
                CustomerInfomation c = _context.CustomerInfomation.Find(cust.MemberId);
                c.StatusId = 3;
                c.SuspensionReason = cust.SuspensionReason;
                _context.Update(c);
                _context.SaveChanges();
            }

            return RedirectToAction("List");
        }
        public IActionResult Create()
        {
            ViewData["DisctrictId"] = new SelectList(_context.District, "DistrictId", "DistrictName");
            ViewData["StatusId"] = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Customer_Infomation")), "StatusId", "Description");

            return View();
        }
        [HttpPost]
        public IActionResult Create(CustomerInfomation cust, IFormFile file)
        {
            
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    cust.Photo = memoryStream.ToArray();
                }
            }
            cust.StatusId = 2;
            _context.CustomerInfomation.Add(cust);
            _context.SaveChanges();
            
            return RedirectToAction("List");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");           

            CustomerInfomation cust = _context.CustomerInfomation.Find(id);
            ViewData["DisctrictId"] = new SelectList(_context.District, "DistrictId", "DistrictName", cust.DistrictId);
            ViewData["StatusId"] = new SelectList(_context.Status.Where(s => s.StatusType.Equals("Customer_Infomation")), "StatusId", "Description", cust.StatusId);

            if (cust == null)
                return RedirectToAction("List");
            CustomerInfomationWrap custwrap = new CustomerInfomationWrap();
            custwrap.CustomerInfomation = cust;
            return View(custwrap);
        }
        [HttpPost]
        public IActionResult Edit(CustomerInfomation pIn, IFormFile file)
        {

            
            CustomerInfomation cust = _context.CustomerInfomation.Find(pIn.MemberId);
            if (cust != null)
                {
                if (file != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        cust.Photo = memoryStream.ToArray();
                    }
                }
                cust.ResidentId = pIn.ResidentId;               
                cust.ParentId =pIn.ParentId;
                cust.Name = pIn.Name;                
                cust.Gender = pIn.Gender;
                cust.DistrictId = pIn.DistrictId;
                cust.Address = pIn.Address;
                cust.BirthDate = pIn.BirthDate;
                cust.Nickname = pIn.Nickname;
                cust.IsAllergy = pIn.IsAllergy;
                cust.AllergyDescription = pIn.AllergyDescription;
                cust.Note = pIn.Note;
                cust.StatusId = pIn.StatusId;                
                cust.SuspensionReason = pIn.SuspensionReason;
                
                _context.SaveChanges();

            }
            return RedirectToAction("List");
        }

        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
              
                CustomerInfomation cust = _context.CustomerInfomation.Find(id);
                if (cust != null)
                {
                    _context.CustomerInfomation.Remove(cust);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }
    }
}
