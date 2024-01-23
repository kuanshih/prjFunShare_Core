using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using prjFunShare_Core.Models;
using prjFunShare_Core.ViewModels;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UAParser;

namespace prjFunShare_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FUNShareContext _context;

        public HomeController(ILogger<HomeController> logger, FUNShareContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> _Navbar()
        {
            CNavbarViewModel vm = null;
            //判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                ViewBag.BeforeLog = "hidden";
                ViewBag.Logged = "";
                int id = (int)HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);
                CustomerInfomation c = _context.CustomerInfomation.Find(id);

                vm = new CNavbarViewModel();
                vm.userPhoto = c.Photo;
                vm.userName = string.IsNullOrEmpty(c.Nickname) ? hideFullName(c.Name) : c.Nickname;
            }

            return PartialView(vm);
        }

        public string hideFullName(string fullName)
        {
            string shortName = "";
            string hide = "";

            for (int i = 0; i<fullName.Length-1; i++)
            {
                hide += "*";
            }

            shortName = fullName.Substring(0, 1) + hide;

            return shortName;
        }

        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> Index()
        {
            //var datas = from p in _context.ProductCategories
            //            where p.Product.StatusId == 12 //上架中
            //            select p.SubCategory.CategoryId;

            //ClndexViewModel vm = new ClndexViewModel();
            //foreach (var item in datas)
            //{
            //    switch (item)
            //    {
            //        case 1:
            //            vm.category1Count++;
            //            break;
            //        case 2:
            //            vm.category2Count++;
            //            break;
            //        case 3:
            //            vm.category3Count++;
            //            break;
            //        case 4:
            //            vm.category4Count++;
            //            break;
            //    }
            //}

            return View();
        }

        public IActionResult Login()
        {
            CLoginViewModel vm = new CLoginViewModel();
            return View(vm);
        }
        public IActionResult Register()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }        
        public IActionResult EmailVerufucationPartial()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOGINED_USER);
            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
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

        public IActionResult ChatWindow(string? receiverId)
        {

            int id = 0;
            
                                 
            // 判斷是否登入
            //Customer
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
                string sender = "sender";
                Response.Cookies.Append(sender, "C" + id.ToString());
                
            }
            return View();
            //Supplier supplier1 = _context.Supplier.FirstOrDefault(s => s.SupplierId == 1);
            //SaveSuppierToSession(supplier1);
            ////Supplier
            //if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            //{
            //    string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
            //    Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);
            //    id = supplier.SupplierId;
            //    string sender = "sender";
            //    Response.Cookies.Append(sender, "S" + id.ToString());
            //    return View();
            //}
            //return View();
        }

        public void SaveSuppierToSession(Supplier supplier)
        {
            //同時儲存完整會員資料&ID
            string json = JsonSerializer.Serialize(supplier);
            HttpContext.Session.SetString(CDictionary.SK_LOGINED_SUPPLIER, json);
           
        }
    }
}