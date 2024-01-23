using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFunShare_Core.Models;
using prjFunShare_Core.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace prjFunShare_Core.Controllers
{
    public partial class CustomerController : Controller
    {
        private readonly FUNShareContext _context;
        private readonly int _id;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerController(FUNShareContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, int memberId = 8)
        {
            _context = context;
            _id = memberId;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Customer
        public IActionResult List()
        {
            return View();
        }

        public IActionResult SendEmailTest()
        {
            return View();
        }
        public IActionResult TestModal()
        {
            return View();
        }

        public IActionResult UserInfo(int? id)
        {
            //測試
            //id = 8;
            id = HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);
            if (id == null)
                return RedirectToAction("List");
            CustomerInfomation customer = _context.CustomerInfomation.Include(c => c.District).Include(c => c.District.City).Where(c => c.MemberId == id).FirstOrDefault();
            if (customer == null)
                return RedirectToAction("List");
            return View(customer);
        }
        public IActionResult myFamily(int? id)
        {
            //測試 張家族譜
            //id = 39;
            id = HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);
            if (HttpContext.Session.Keys.Contains("EditFamilyInfoErrorMessage"))
            {
                ViewBag.EditFamilyInfoErrorMessage = HttpContext.Session.GetString(CDictionary.EditFamilyInfoErrorMessage);
                HttpContext.Session.Remove("EditFamilyInfoErrorMessage");
            }
            if (id == null)
                return RedirectToAction("Index", "Home");
            var member = _context.CustomerInfomation.Find(id);
            List<CFamilyView> customer = new List<CFamilyView>();
            FindParent(customer, member.MemberId, member.ParentId, 0);
            if (customer == null)
                return RedirectToAction("Index", "Home");
            return View(customer);
        }
        #region 遞迴找所有家人
        private void FindParent(List<CFamilyView> list, int NowMemberID, int? NowParentId, int nowLevel)
        {
            CustomerInfomation parent = null;
            if (NowParentId != null)
                parent = _context.CustomerInfomation.Where(p => p.MemberId == NowParentId).Select(p => p).First();

            CustomerInfomation my = _context.CustomerInfomation.Where(m => m.MemberId == NowMemberID).Select(m => m).First();

            //找到最上層的家長
            if (parent == null)
            {
                CFamilyView family = new CFamilyView();
                family.MemberId = my.MemberId;
                family.ParentId = my.ParentId;
                family.Name = my.Name;
                family.BirthDate = my.BirthDate;
                family.Photo = my.Photo;
                family.Level = nowLevel;

                list.Add(family);

                FindChild(list, NowMemberID, nowLevel + 1);
                return;
            }
            else
            {
                NowMemberID = (int)NowParentId;

                if (parent != null)
                    NowParentId = parent.ParentId;
                else
                    NowParentId = null;
                FindParent(list, NowMemberID, NowParentId, nowLevel - 1);
            }
        }

        private void FindChild(List<CFamilyView> list, int NowMemberID, int nowLevel)
        {
            List<CustomerInfomation> childList = _context.CustomerInfomation.Where(c => c.ParentId == NowMemberID).Select(c => c).ToList();

            if (childList.Count == 0)
                return;
            else
            {
                foreach (var child in childList)
                {
                    CFamilyView family = new CFamilyView();
                    family.MemberId = child.MemberId;
                    family.ParentId = child.ParentId;
                    family.Name = child.Name;
                    family.BirthDate = child.BirthDate;
                    family.Photo = child.Photo;
                    family.Level = nowLevel;

                    list.Add(family);
                    FindChild(list, child.MemberId, nowLevel + 1);
                }
            }
        }
        #endregion
        public IActionResult FamilyEdit(int id)
        {
            //確認該用戶是否註冊 => 未註冊才可以編輯
            CustomerInfomation customer = _context.CustomerInfomation.Where(c => c.MemberId == id).FirstOrDefault();
            if (customer == null)
            {
                HttpContext.Session.SetString(CDictionary.EditFamilyInfoErrorMessage, "此用戶不存在");
                return RedirectToAction("myFamily");
            }
            if (customer.StatusId != 2)
            {
                HttpContext.Session.SetString(CDictionary.EditFamilyInfoErrorMessage, "此用戶已註冊過，無法編輯資料");
                return RedirectToAction("myFamily");
            }
            return View(customer);
        }
        public IActionResult FamilyCV(int id)
        {
            CustomerInfomation customer = _context.CustomerInfomation.Where(c => c.MemberId == id).FirstOrDefault();
            if (customer == null)
                return RedirectToAction("myFamily");

            int totaltimes = _context.OrderDetail.Where(o => o.MemberId == id && o.IsAttend == true).Select(o => o.ProductDetail.Product.ProductName).Distinct().Count();
            ViewBag.totaltimes = totaltimes;
            return View(customer);
        }
        public IActionResult AddFamily()
        {
            return View();
        }
        public IActionResult myPocketList()
        {
            //var datas = from p in _context.Product.Where(p => p.PocketList.FirstOrDefault().MemberId == _id)
            //        .Include(p=>p.Status)
            //        .Include(p => p.ProductDetail)
            //        .ThenInclude(pp => pp.OrderDetail)
            //        .Include(p => p.Supplier)
            //        .Include(p => p.ImageList)
            //        .Include(p => p.ProductCategories)
            //        .ThenInclude(pp => pp.SubCategory)
            //        .ThenInclude(ppp => ppp.Category)
            //        .Include(p => p.ProductDetail)
            //        .ThenInclude(pp => pp.District)
            //        .ThenInclude(ppp => ppp.City)
            //            select p;
            return View(/*datas*/);
        }

        public IActionResult myOrderList()
        {
            //var datas = from o in _context.Order.Where(p => p.MemberId == _id)
            //.Include(p => p.OrderDetail)
            //.ThenInclude(pp => pp.ProductDetail)
            //.ThenInclude(ppp => ppp.Product)
            // .ThenInclude(pppp => pppp.Supplier)

            // .Include(p => p.OrderDetail)
            //.ThenInclude(pp => pp.Status)

            //.Include(p => p.Status)
            //.Include(p => p.OrderDetail)
            //.ThenInclude(pp => pp.ProductDetail)
            //.ThenInclude(ppp => ppp.Product)
            //.ThenInclude(pppp => pppp.ImageList)
            //            orderby o.OrderDetail.First().ProductDetail.BeginTime descending
            //            select o;
            //return View(datas);
            var datas = from p in _context.Product.Where(p => p.PocketList.FirstOrDefault().MemberId == _id)
                                   .Include(p => p.ProductDetail)
                                       .ThenInclude(pp => pp.OrderDetail)
                                                         .Include(p => p.Supplier)
                                                                        .Include(p => p.ProductCategories)
                                .ThenInclude(pp => pp.SubCategory)


                        select p;
            return View(datas);
        }
        public IActionResult myOrderDetail(int id)
        {
            return View();
        }

        public IActionResult myPoint()
        {
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }
            var datas = from b in _context.Bonus.Where(p => p.MemberId == id)
                        select b;
            return View(datas);
        }
        public IActionResult myCoupon()
        {
            int id = (int)HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);

            var datas = _context.MemberCoupon.Where(c => c.MemberId == id)
                .Select(p => new CMyCouponViewModel
                {
                    Name = p.Coupon.Name,
                    Discount = string.Format("{0:N2}", p.Coupon.Discount) + "%",
                    Description = p.Coupon.Description,
                    ProductName = p.Coupon.ProductId.HasValue? p.Coupon.Product.ProductName: "全站適用",
                    DueDate = p.Coupon.DueDate,
                    //todo DB關聯沒拉...
                    StatusDescription = _context.Status.Where(x => x.StatusId ==p.StatusId).First().Description,
                }) ;

            //var datas = from i in _context.CouponList
            //            .Include(p => p.Order)
            //            .ThenInclude(p => p.OrderDetail)
            //            .ThenInclude(pp => pp.ProductDetail)
            //            .ThenInclude(ppp => ppp.Product)
                        
            //            select i;
            return View(datas);
        }
        public IActionResult ChooseVerifyWay()
        {
            return View();
        }
        public IActionResult VerifyError()
        {
            return View();
        }
        public IActionResult ResetPassword(string verify)
        {
            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            if (!api.VerifyCode(verify))
            {
                RedirectToAction("VerifyError");
            }
            return View();
        }
        
        //先廢棄Partial
        public IActionResult EmailVerufucationPartial()
        {
            return PartialView();
        }
        //測試上傳照片用-Start
        public IActionResult EditUserPhoto()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EditUserPhoto(CustomerInfomation x, IFormFile file)
        {
            CustomerInfomation customer = _context.CustomerInfomation.FirstOrDefault(c => c.MemberId == x.MemberId);
            if (customer != null)
            {
                //存進DB
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        customer.Photo = ms.ToArray();
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("myFamily");
        }
        //測試上傳照片用-End

        public IActionResult CustomImage() 
        {
            return View();        
        }

        
            public IActionResult UploadImage(IFormFile file)
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        // 生成唯一的文件名，例如使用 GUID
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // 拼接文件的完整路径
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                        // 将文件保存到文件系统中
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        // 构建图像的URL
                        string imageUrl = Url.Content("~/img/" + uniqueFileName);

                        // 返回成功的响应，包括图像的URL
                        return Ok(new { imageUrl });
                    }
                    catch (Exception ex)
                    {
                        // 处理异常
                        return BadRequest($"上传文件失败: {ex.Message}");
                    }
                }
                else
                {
                    return BadRequest("未上传文件.");
                }
            }
        }

}