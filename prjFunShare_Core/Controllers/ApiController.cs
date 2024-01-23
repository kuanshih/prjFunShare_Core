using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFunShare_Core.Models;
using System.Text.Json;
using prjFunShare_Core.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Collections;
using Microsoft.Extensions.Options;
using QRCoder;
using prjFunShare_Core.ViewModels.Email;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using RazorEngine;
using RazorEngine.Templating;
using System.Net.Mime;
using static QRCoder.PayloadGenerator;
using RazorEngine;
using RazorEngine.Templating;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenAI_API.Images;

namespace prjFunShare_Core.Controllers
{
    public class ApiController : Controller
    {
        private readonly FUNShareContext _context;
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _configuration;

        public ApiController(FUNShareContext context, IWebHostEnvironment host, IConfiguration configuration)
        {
            _context = context;
            _host = host;
            _configuration = configuration;
        }
        #region 登入/註冊
        public IActionResult Login(CLoginViewModel vm)
        {
            //if (!Captcha.ValidateCaptchaCode(vm.txtCaptchaCode.ToUpper(), HttpContext))
            //{
            //    HttpContext.Session.SetString(CDictionary.LoginErrorMessage, "驗證碼錯誤，請重新登入");
            //    TempData["LoginErrorMessage"] = HttpContext.Session.GetString(CDictionary.LoginErrorMessage);
            //    return Content("驗證碼錯誤，請重新登入");
            //}
            CustomerInfomation user = _context.CustomerInfomation.FirstOrDefault(c => c.Email == vm.txtAccount);
            if (user != null && user.Password.Equals(vm.txtPassword))
            {
                if (user.StatusId == 3)
                {
                    return Content("您的帳號已被停權，請洽客服人員。");
                }
                if (user.RegistrationDay == null)
                {
                    return Content("您的帳號尚未完成信箱驗證，請確認信箱內是否有收到驗證信。");
                }
                SaveUserToSession(user);
                ViewBag.BeforeLog = "hidden";
                ViewBag.Logged = "";
                return Content("登入成功");
            }
            //HttpContext.Session.SetString(CDictionary.LoginErrorMessage, "帳號或密碼輸入錯誤，請重新登入");
            //TempData["LoginErrorMessage"] = HttpContext.Session.GetString(CDictionary.LoginErrorMessage);
            //vm.txtAccount = null;
            //vm.txtPassword = null;
            return Content("帳號或密碼輸入錯誤，請再次確認後重新登入");
        }
        public void SaveUserToSession(CustomerInfomation user)
        {
            //同時儲存完整會員資料&ID
            string json = JsonSerializer.Serialize(user);
            HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, json);
            HttpContext.Session.SetInt32(CDictionary.SK_LOGINED_ID, user.MemberId);

            //登入設備
            var loginDevice = HttpContext.Request.Headers["Sec-Ch-Ua-Platform"];
            //登入時間
            var now = DateTime.Now;
            var nowTime = now.ToString("F"); ;

            LoginLog logininfo = new LoginLog();
            logininfo.MemberId = user.MemberId;
            logininfo.LoginTime = now;
            logininfo.Device = loginDevice;
            _context.LoginLog.Add(logininfo);
            _context.SaveChanges();

            //LINE Notify
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "jE686yTBHTMLYDhu5ocb4RPPhUrgjFRdKyismhgY8cT");
            var content = new Dictionary<string, string>();
            content.Add("message", $"\nFunShare平台提醒您。\n\n會員「{user.Name}」目前已在{loginDevice}上登入。\n\n登入時間：{nowTime}");
            httpClient.PostAsync("https://notify-api.line.me/api/notify", new FormUrlEncodedContent(content));
        }
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode().ToUpper();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
        public IActionResult Register(CustomerInfomation customer)
        {
            CustomerInfomation user = _context.CustomerInfomation.FirstOrDefault(c => c.Email == customer.Email);
            if (user == null)
            {
                FileStream stream = new FileStream("./wwwroot/img/預設頭像.jpg", FileMode.Open);
                //C:\MSIT150_專題_FunShare(期末)\FunShare\prjFunShare_Core\wwwroot\img\預設頭像.jpg
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                customer.Photo = ms.ToArray();
                customer.IsAllergy = false;
                customer.DistrictId = 1;

                _context.CustomerInfomation.Add(customer);
                _context.SaveChanges();
                //SaveUserToSession(customer);

                NotVerified(customer.Email);
                return Content("寄送成功");
            }
            return Content("此用戶已註冊過，請直接登入");
        }
        public void NotVerified(string userEmail)
        {
            CEmailVerify data = new CEmailVerify()
            {
                Email = userEmail,
                mailSubject = "FunShare親子學習平台 信箱驗證連結",
                mailContent = "點擊以下連結，即可回到FunShare完成註冊驗證，連結將在 30 分鐘後失效。<br><br>",
                receivePage = "Api/VerifyMail",
                linkText = "點我啟用帳號",
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.ToString(),
            };
            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            api.SendVerifyMail(data);
            return;
        }
        public IActionResult VerifyMail(string verify)
        {
            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            if (!api.VerifyCode(verify))
            {
                RedirectToAction("VerifyError");
            }
            CustomerInfomation customer = _context.CustomerInfomation.Where(m => m.Email.Equals(HttpContext.Session.GetString(CDictionary.SK_RESET_PWD_EMAIL))).FirstOrDefault();
            if (customer != null)
            {
                customer.RegistrationDay = DateTime.Now;
                _context.SaveChanges();
            }

            return View();
        }
        public IActionResult VerifyError()
        {
            return View();
        }
        #endregion

        #region 會員
        public IActionResult UserInfo(CustomerInfomation x, IFormFile file)
        {
            CustomerInfomation customer = _context.CustomerInfomation.FirstOrDefault(c => c.MemberId == x.MemberId);
            if (customer != null)
            {
                //存進DB;
                customer.Name = x.Name;
                customer.ResidentId = x.ResidentId;
                customer.Email = x.Email;
                customer.PhoneNumber = x.PhoneNumber;
                customer.Gender = x.Gender;
                customer.Nickname = x.Nickname;
                customer.DistrictId = x.DistrictId;
                customer.BirthDate = x.BirthDate;
                customer.Address = x.Address;
                customer.IsAllergy = x.IsAllergy;
                //是否過敏待確認
                if ((bool)x.IsAllergy)
                    customer.AllergyDescription = x.AllergyDescription;
                else
                    customer.AllergyDescription = null;
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        customer.Photo = ms.ToArray();
                    }
                }
                _context.SaveChanges();
                //return RedirectToAction("List");
                return Content("儲存成功");
            }
            return Content("查無此會員");
        }
        public IActionResult AddFamily(CustomerInfomation customer, bool IsChild, IFormFile file)
        {
            //親屬註冊狀態判斷
            var NewMember = _context.CustomerInfomation.Where(c => c.ResidentId == customer.ResidentId).FirstOrDefault();
            //是否已有該親屬(會員資料)
            bool Memberflag;
            if (NewMember != null)
            {
                customer = NewMember;
            }
            else
            {
                //新增進資料庫
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        customer.Photo = ms.ToArray();
                    }
                }
                else
                {
                    FileStream stream = new FileStream("./wwwroot/img/預設頭像.jpg", FileMode.Open);
                    //C:\MSIT150_專題_FunShare(期末)\FunShare\prjFunShare_Core\wwwroot\img\預設頭像.jpg
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    customer.Photo = ms.ToArray();
                }
                _context.CustomerInfomation.Add(customer);
                _context.SaveChanges();
            }
            //待修改
            //親屬關係
            int? LoginId = HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);
            CustomerInfomation oldmember = _context.CustomerInfomation.Where(c => c.MemberId == LoginId).FirstOrDefault();
            if (!IsChild)
            {
                if (oldmember.ParentId == null)
                {
                    oldmember.ParentId = customer.MemberId;
                    //MessageBox.Show("已更新親屬關聯");
                }
                else
                {
                    if (oldmember.ParentId == customer.MemberId)
                    return Content("您曾綁定過該成員為您的父母，已取消新增親屬關聯");

                    else
                    {
                        return Content("您先前綁定過其他會員為您的父母，已取消新增親屬關聯");

                        //DialogResult = MessageBox.Show("您先前綁定過其他會員為您的父母，請問是否需做變更", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        //if (DialogResult == DialogResult.OK)
                        //{
                        //    oldmember.ParentId = customer.MemberId;
                        //    MessageBox.Show("已更新親屬關聯");
                        //}
                        //else
                        //    MessageBox.Show("已取消新增親屬關聯");
                    }
                }
            }
            else
            {
                if (customer.ParentId == null)
                {
                    customer.ParentId = oldmember.MemberId;
                    //MessageBox.Show("已更新親屬關聯");
                }

                else
                {
                    if (customer.ParentId == oldmember.MemberId)
                        return Content("您曾綁定過該會員為您的小孩，已取消新增親屬關聯");
                    else
                    {
                        return Content("該會員先前綁定過其他會員為父母，已取消新增親屬關聯");
                        //MessageBox.Show("該會員先前綁定過其他會員為父母，無法變更", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //        MessageBox.Show("已取消新增親屬關聯");
                            }
                }
            }
            _context.SaveChanges();
            return Content("新增成功");
            //return RedirectToAction("myFamily");

        }
        public IActionResult FamilyEdit(CustomerInfomation x, IFormFile file)
        {
            CustomerInfomation customer = _context.CustomerInfomation.FirstOrDefault(c => c.MemberId == x.MemberId);
            if (customer != null)
            {
                //存進DB;
                customer.Name = x.Name;
                customer.ResidentId = x.ResidentId;
                customer.Gender = x.Gender;
                customer.BirthDate = x.BirthDate;
                customer.IsAllergy = x.IsAllergy;
                //是否過敏待確認
                if ((bool)x.IsAllergy)
                    customer.AllergyDescription = x.AllergyDescription;
                else
                    customer.AllergyDescription = null;
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        customer.Photo = ms.ToArray();
                    }
                }
                _context.SaveChanges();
                //return RedirectToAction("List");
                return Content("儲存成功");
            }
            return Content("查無此會員");
        }

        public IActionResult Cities()
        {
            var citys = _context.City.Select(c => c.CityName);

            return Json(citys);
        }
        public IActionResult Districts(string city)
        {
            var districts = _context.District.Where(c => c.City.CityName == city).Select(d => d.DistrictName);

            return Json(districts);
        }
        public IActionResult FindDistrictId(string city, string district)
        {
            var districtID = _context.District.Where(d => d.DistrictName == district && d.City.CityName == city).Select(d => d.DistrictId).FirstOrDefault();
            //todo 有些找不到ID
            return Json(districtID);
        }
        public IActionResult getCategoryAbility(string category)
        {
            var dataone = _context.SubCategory.Where(s => s.SubCategoryName == category).Select(s => s.AbilityOne);
            var datatwo = _context.SubCategory.Where(s => s.SubCategoryName == category).Select(s => s.AbilityTwo);
            var datathree = _context.SubCategory.Where(s => s.SubCategoryName == category).Select(s => s.AbilityThree);

            var datas = dataone.Concat(datatwo).Concat(datathree);

            return Json(datas);
        }
        public IActionResult getCategory()
        {
            var datas = _context.SubCategory.Select(s => s.SubCategoryName);
            return Json(datas);
        }
        public IActionResult getLearningRecord(int memberid, string category)
        {
            var datas = _context.OrderDetail.Where(o => o.MemberId == memberid && o.IsAttend == true && o.ProductDetail.Product.ProductCategories.First().SubCategory.SubCategoryName.Equals(category)).Select(o => o.ProductDetail.Product.ProductName).Distinct();

            return Json(datas);
        }

        public IActionResult getBadgeDescription(string badgeFileName)
        {
            var datas = _context.Achievement.Where(a => a.AchievementFileName == badgeFileName).Select(a => a.AchievementDescription).First();

            return Json(datas);
        }
        public IActionResult getBadge(int memberid)
        {
            var datas = _context.MemberAchievement.Include(m => m.Achievement).Where(m => m.MemberId == memberid).Select(m => m.Achievement.AchievementFileName).Distinct();
            ArrayList badge = new ArrayList(datas.ToArray());
            var achievementID = _context.MemberAchievement.Include(m => m.Achievement).Where(m => m.MemberId == memberid).Select(m => m.Achievement.AchievementId);
            //todo 計算次數新增徽章

            //徽章-參加5次以上課程
            if (achievementID.Count() >= 5)
            {
                string achievement = _context.Achievement.Find(4).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-參加3次以上「航太體驗」類型活動
            if (achievementID.Where(a => (a == 3)).Count() >= 3)
            {
                string achievement = _context.Achievement.Find(8).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-參加3次以上「戶外體驗」類型活動
            if (achievementID.Where(a => (a == 14)).Count() >= 3)
            {
                string achievement = _context.Achievement.Find(9).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-參加3次以上活動/課程
            if (achievementID.Count() >= 3)
            {
                string achievement = _context.Achievement.Find(10).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-參加3次以上「自然科學」類型課程
            if (achievementID.Where(a => (a == 13)).Count() >= 3)
            {
                string achievement = _context.Achievement.Find(23).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-參加3次以上「運動」類型課程
            if (achievementID.Where(a => (a == 5)).Count() >= 3)
            {
                string achievement = _context.Achievement.Find(24).AchievementFileName;
                badge.Add(achievement);
            }

            //labels: ['身體動作與健康', '美感', '情緒', '社會', '語文', '認知']
            int[] abilityValue = getAbilityValue(memberid);

            //徽章-「身體動作與健康」能力累積5點以上
            if (abilityValue[0] >= 5)
            {
                string achievement = _context.Achievement.Find(12).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-「美感」能力累積5點以上
            if (abilityValue[1] >= 5)
            {
                string achievement = _context.Achievement.Find(18).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-「情緒」能力累積5點以上
            if (abilityValue[2] >= 5)
            {
                string achievement = _context.Achievement.Find(15).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-「社會」能力累積5點以上
            if (abilityValue[3] >= 5)
            {
                string achievement = _context.Achievement.Find(22).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-「語文」能力累積5點以上
            if (abilityValue[4] >= 5)
            {
                string achievement = _context.Achievement.Find(17).AchievementFileName;
                badge.Add(achievement);
            }
            //徽章-「認知」能力累積5點以上
            if (abilityValue[5] >= 5)
            {
                string achievement = _context.Achievement.Find(25).AchievementFileName;
                badge.Add(achievement);
            }

            return Json(badge);
        }
        public int[] getAbilityValue(int memberid)
        {
            //labels: ['身體動作與健康', '美感', '情緒', '社會', '語文', '認知']
            int[] abilityValue = new int[6] { 0, 0, 0, 0, 0, 0 };
            var dataone = _context.OrderDetail.Include(o => o.ProductDetail).Include(o => o.ProductDetail.Product).Include(o => o.ProductDetail.Product.ProductCategories).Where(o => o.MemberId == memberid && o.IsAttend == true).Select(o => o.ProductDetail.Product.ProductCategories.First().SubCategory.AbilityOne);
            var datatwo = _context.OrderDetail.Include(o => o.ProductDetail).Include(o => o.ProductDetail.Product).Include(o => o.ProductDetail.Product.ProductCategories).Where(o => o.MemberId == memberid && o.IsAttend == true).Select(o => o.ProductDetail.Product.ProductCategories.First().SubCategory.AbilityTwo);
            var datathree = _context.OrderDetail.Include(o => o.ProductDetail).Include(o => o.ProductDetail.Product).Include(o => o.ProductDetail.Product.ProductCategories).Where(o => o.MemberId == memberid && o.IsAttend == true).Select(o => o.ProductDetail.Product.ProductCategories.First().SubCategory.AbilityThree);
            string[] ability = dataone.Concat(datatwo).Concat(datathree).ToArray();

            int count = ability.Length;
            for (int i = 0; i < count; i++)
            {
                if (ability[i] == "身體動作與健康")
                {
                    abilityValue[0]++;
                }
                else if (ability[i] == "美感")
                {
                    abilityValue[1]++;
                }
                else if (ability[i] == "情緒")
                {
                    abilityValue[2]++;
                }
                else if (ability[i] == "社會")
                {
                    abilityValue[3]++;
                }
                else if (ability[i] == "語文")
                {
                    abilityValue[4]++;
                }
                else if (ability[i] == "認知")
                {
                    abilityValue[5]++;
                }
            }
            return abilityValue;
        }
        public void ForgotPassword(string userEmail)
        {
            CEmailVerify data = new CEmailVerify()
            {
                Email = userEmail,
                mailSubject = "FunShare親子學習平台 重設密碼連結",
                mailContent = "點擊以下連結，即可回到FunShare重新設定密碼，連結將在 30 分鐘後失效。<br><br>",
                receivePage = "Customer/ResetPassword",
                linkText = "點我重設密碼",
                scheme = HttpContext.Request.Scheme,
                host = HttpContext.Request.Host.ToString(),
            };
            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            api.SendVerifyMail(data);

            return;
        }
        //public IActionResult ResetPassword(string verify)
        //{
        //    EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
        //    if (!api.VerifyCode(verify))
        //    {
        //        RedirectToAction("Login");
        //    }
        //    return View();
        //}

        public void SavePassword(string userPassword)
        {
            CustomerInfomation customer = _context.CustomerInfomation.Where(c => c.Email.Equals(HttpContext.Session.GetString(CDictionary.SK_RESET_PWD_EMAIL))).FirstOrDefault();
            if (customer != null)
            {
                customer.Password = userPassword;
                _context.SaveChanges();
            }
            return;
        }
        #endregion

        #region 給【訂單成立】接寄送Email的Api(請自己接到各自的region範圍內)
        public void OrderSuccessEmail(string userEmail)
        {
            COrderEstablish data = new COrderEstablish()
            {
                Email = userEmail,
                //信件標題
                mailSubject = "FunShare親子學習平台 訂單成立通知",
                //信件內容
                mailContent = "您的訂購已完成\n訂單編號：XXXXXX(參數傳入)",
            };
            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            api.SendOrderEstablishMail(data);

            return;
        }
        #endregion

        #region 給【付款成功】接寄送Email的Api(請自己接到各自的region範圍內)
        public void PaymentSuccessfulEmail(int? id)
        {
            //未輸入Order ID 或 ID不存在
            if (id == null || !_context.Order.Any(x => x.OrderId == id) || !_context.OrderDetail.Any(x => x.OrderId == id))
                return;
            //todo... QRCode傳送
            //OrderController order = new OrderController(_context);
            //File file = order.getQrCodeImage(1) as File;

            //使用Email模板
            string templatePath = "Views/Email/OrderEmailTemplate.cshtml";

            var vm = _context.OrderDetail
                .Where(x => x.OrderId == id)
                .Select(x=> new COrderEmailViewModel{
                    MemberName = x.Order.Member.Name,
                    Email = x.Order.Member.Email,
                    ProductName = x.ProductDetail.Product.ProductName,
                    BeginTime = x.ProductDetail.BeginTime.Value.ToString("yyyy/MM/dd hh:mm"),
                    EndTime = x.ProductDetail.EndTime.Value.ToString("yyyy/MM/dd hh:mm"),
                    Address = x.ProductDetail.District.City.CityName + x.ProductDetail.District.DistrictName + x.ProductDetail.Address
                }).First();

            vm.ButtonLink = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.ToString() + "/Customer/myOrderList";
            vm.OrderDetails = _context.OrderDetail.Where(x => x.OrderId == id)
                .Select(x => new COrderEmailDetails
                {
                    AttendName = x.Member.Name,
                    OrderDetailNumber = $"OD{x.Order.OrderTime.Date.ToString("yyyyMMdd")}{x.OrderDetailId.ToString("0000")}",
                }).ToList();

            //建立連結資源
            List<LinkedResource> linkedrs = new List<LinkedResource>();
            foreach (var od in vm.OrderDetails)
            {
                //取得qrcode二進位陣列
                byte[] bytes = getQrCodeBytes(od.OrderDetailNumber);
                //轉成MemoryStream
                MemoryStream qrcode = new MemoryStream(bytes);
                //存入資源中
                var res = new LinkedResource(qrcode);
                res.ContentId = Guid.NewGuid().ToString();
                linkedrs.Add(res);
                //使用<img src="/img/loading.svg" data-src="cid:..."方式引用內嵌圖片
                od.ImagePath = $@"cid:{res.ContentId}";
            }

            CPaymentSuccessful data = new CPaymentSuccessful();
            data.Email = vm.Email;
            //信件標題
            data.mailSubject = "FunShare親子學習平台 付款成功通知";
                //信件內容
                //mailContent = "您的訂單已付款成功\n",
                //mailContent = "您的訂單已付款成功\n" + content,
                //mailContent = "您的訂單已付款成功\n" + "<iframe><img src=\"@Url.Action(\"getQrCodeImage\",\"Order\")?id=1\" />\r\n</iframe>",

                //QRCode = order.getQrCodeImage(1) as File

            string mailContent = Engine.Razor.RunCompile(System.IO.File.ReadAllText(templatePath), "ordermail", typeof(COrderEmailViewModel), vm);
            data.alternateView = AlternateView.CreateAlternateViewFromString(mailContent, null, MediaTypeNames.Text.Html);
            //將圖檔資源加入AlternativeView
            foreach (var r in linkedrs)
            {
                data.alternateView.LinkedResources.Add(r);
            }

            EmailApiController api = new EmailApiController(_configuration, HttpContext, _context);
            api.SendPaymentSuccessfulMail(data);
        }

        //產生QRcode 二進位陣列
        public byte[] getQrCodeBytes(string txtForQrcode)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtForQrcode, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            return qrCodeAsBitmapByteArr;
        }
        #endregion

        public async Task<IActionResult> getProductsOnSale()
        {
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }

            var datas = from prod in _context.Product
                        where prod.StatusId == 12
                        select new CProductCardViewModel
                        {
                            ProductId = prod.ProductId,
                            ProductName = prod.ProductName,
                            Features = string.IsNullOrEmpty(prod.Features) ? prod.ProductIntro : prod.Features,
                            CategoryId = prod.ProductCategories.First().SubCategory.CategoryId,
                            SubCategoryName = prod.ProductCategories.First().SubCategory.SubCategoryName,
                            CityName = prod.ProductDetail.Select(x => x.District.City.CityName).Distinct().Count() > 1 ? "多縣市" : prod.ProductDetail.Select(x => x.District.City.CityName).First(),
                            ImagePath = prod.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                            IsClass = prod.IsClass ? "長期課程" : ((prod.ProductDetail.FirstOrDefault().EndTime - prod.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                            IsPocketed = prod.PocketList.Where(x => x.MemberId == id).Any(),
                            _UnitPrice = (decimal)prod.ProductDetail.Select(x => x.UnitPrice).Min(),
                            fDateMax = prod.ProductDetail.Select(x => x.EndTime).Max(),
                            fDateMin = prod.ProductDetail.Select(x => x.BeginTime).Min(),
                            fDates = prod.ProductDetail.Select(x => x.BeginTime.Value).ToArray(),
                            orderCount = _context.OrderDetail.Where(x => x.ProductDetail.ProductId == prod.ProductId).Count(),
                            Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == prod.ProductId).Select(x => x.Rank).Average().GetValueOrDefault(0).ToString("0.#")
                        };

            return Json(datas);
        }

        [HttpPost]
        public IActionResult getProductsSearch(CProductFilterViewModel vm)
        {
            int id = 0;

            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }

            var datas = _context.Product
                .Include(x => x.ProductDetail).ThenInclude(x => x.District).ThenInclude(x => x.City)
                .Include(x => x.ProductCategories).ThenInclude(x => x.SubCategory).ThenInclude(x => x.Category)
                .Include(x => x.ImageList)
                .Include(x => x.PocketList)
                .Include(x => x.Age)
                .AsEnumerable()
                .Where(prod => prod.StatusId == 12) //上架中
                .OrderByDescending(x => x.ReleasedTime) //依上架時間排序
                .Select(prod => new CProductCardViewModel
                {
                    ProductId = prod.ProductId,
                    ProductName = prod.ProductName,
                    Features = string.IsNullOrEmpty(prod.Features) ? prod.ProductIntro : prod.Features,
                    IsClass = prod.IsClass ? "長期課程" : ((prod.ProductDetail.FirstOrDefault().EndTime - prod.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                    ImagePath = prod.ImageList.FirstOrDefault().ImagePath,
                    CategoryId = prod.ProductCategories.First().SubCategory.CategoryId,
                    CategoryName = prod.ProductCategories.First().SubCategory.Category.CategoryName,
                    SubCategoryName = prod.ProductCategories.First().SubCategory.SubCategoryName,
                    CityName = prod.ProductDetail.Select(x => x.District.City.CityName).Distinct().Count() > 1 ? "多縣市" : prod.ProductDetail.Select(x => x.District.City.CityName).FirstOrDefault(""),
                    IsPocketed = prod.PocketList.Where(x => x.MemberId == id).Any(),
                    _UnitPrice = (decimal)prod.ProductDetail.Select(x => x.UnitPrice).Min(),
                    orderCount = _context.OrderDetail.Where(x => x.ProductDetail.ProductId == prod.ProductId).Count(),
                    Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == prod.ProductId).Select(x => x.Rank).Average().GetValueOrDefault(0).ToString("0.#"),
                    //篩選需要的資料
                    fAgeId = prod.AgeId,
                    fRegionIds = prod.ProductDetail.Select(x => x.District.City.RegionId).Distinct().ToArray(),
                    fCategoryIds = prod.ProductCategories.Select(x => x.SubCategory.CategoryId).ToArray(),
                    fDateMax = prod.ProductDetail.Select(x => x.EndTime).Max(),
                    fDateMin = prod.ProductDetail.Select(x => x.BeginTime).Min(),
                })
                .Where(prod => productFilter(prod, vm));

            return Json(datas);
        }

        public bool productFilter(CProductCardViewModel prod, CProductFilterViewModel vm)
        {
            //關鍵字
            if (!string.IsNullOrEmpty(vm.Keyword))
            {
                if (!prod.ProductName.Contains(vm.Keyword))
                    return false;
            }
            //價格
            if (prod._UnitPrice > vm.PriceMax || prod._UnitPrice < vm.PriceMin)
                return false;
            //分齡
            if (vm.AgeCheckbox != null && prod.fAgeId != null)
            {
                if (!vm.AgeCheckbox.Contains(0))
                {
                    if (!vm.AgeCheckbox.Contains((int)prod.fAgeId))
                        return false;
                }
            }
            //分類
            if (vm.CategoryCheckbox != null)
            {
                if (!vm.CategoryCheckbox.Contains(0))
                {
                    bool flag = false;
                    foreach (var c in prod.fCategoryIds)
                    {
                        if (vm.CategoryCheckbox.Contains(c))
                            flag = true;
                    }
                    if (!flag)
                        return false;
                }
            }
            //地區
            if (vm.RegionCheckbox != null && prod.fRegionIds != null)
            {
                if (!vm.RegionCheckbox.Contains(0))
                {
                    bool flag = false;
                    foreach (var r in prod.fRegionIds)
                    {
                        if (vm.RegionCheckbox.Contains((int)r))
                            flag = true;
                    }
                    if (!flag)
                        return false;
                }
            }
            //todo 日期需要再驗證
            if (vm.dateStart != null)
            {
                if (prod.fDateMin < vm.dateStart.Value)
                    return false;
            }
            if (vm.dateEnd != null)
            {
                if (prod.fDateMax > vm.dateEnd.Value)
                    return false;
            }
            //課程形式   0：單日體驗；1：多日體驗；2：長期課程
            if (vm.lsClassRadio != null)
            {
                if (prod.IsClass == "單日體驗" && vm.lsClassRadio == 0)
                    return true;
                else if (prod.IsClass == "多日體驗" && vm.lsClassRadio == 1)
                    return true;
                else if (prod.IsClass == "長期課程" && vm.lsClassRadio == 2)
                    return true;
                else
                    return false;
            }

            return true;
        }
        [HttpGet]
        public async Task<IActionResult> getProductsByKeyword(string? keyword)
        {
            int id = 0;

            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }

            var datas = from prod in _context.Product
                        where prod.StatusId == 12 && (string.IsNullOrEmpty(keyword) ? true : prod.ProductName.Contains(keyword))
                        orderby prod.ReleasedTime descending //依上架時間排序
                        select new CProductCardViewModel
                        {
                            ProductId = prod.ProductId,
                            ProductName = prod.ProductName,
                            Features = string.IsNullOrEmpty(prod.Features) ? prod.ProductIntro : prod.Features,
                            CategoryId = prod.ProductCategories.First().SubCategory.CategoryId,
                            CategoryName = prod.ProductCategories.First().SubCategory.Category.CategoryName,
                            SubCategoryName = prod.ProductCategories.First().SubCategory.SubCategoryName,
                            CityName = prod.ProductDetail.Select(x => x.District.City.CityName).Distinct().Count() > 1 ? "多縣市" : prod.ProductDetail.Select(x => x.District.City.CityName).First(),
                            //Rank = prod.Comment.Select(x => x.Rank).Average(),
                            ImagePath = prod.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                            IsClass = prod.IsClass ? "長期課程" : ((prod.ProductDetail.FirstOrDefault().EndTime - prod.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                            IsPocketed = prod.PocketList.Where(x => x.MemberId == id).Any(),
                            _UnitPrice = (decimal)prod.ProductDetail.Select(x => x.UnitPrice).Min()
                        };

            return Json(datas);
        }
        //搜尋列的autocomplete
        public IActionResult autoCompleteProducts(string? keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(null);
            }

            var products = _context.Product.Where(x => x.ProductName.Contains(keyword)).Select(x => x.ProductName);
            return  Json(products);
        }

        //搜尋頁抓統計數量
        public async Task<IActionResult> getProductSummary()
        {
            CProductSummaryViewModel vm = new CProductSummaryViewModel();
            //計算分類數量
            var categories = from prod in _context.ProductCategories.Include(x => x.SubCategory).Include(x => x.Product).AsEnumerable()
                             where prod.Product.StatusId == 12
                             group prod by prod.SubCategory.CategoryId into g
                             orderby g.Key
                             select g;

            vm.CategoriesCount = new List<int>();
            foreach (var g in categories)
            {
                vm.CategoriesCount.Add(g.Count());
            }
            //計算分區數量
            var regions = from prod in _context.ProductDetail.Include(x => x.Product)
                          .Include(x => x.District).ThenInclude(x => x.City).AsEnumerable()
                          where prod.Product.StatusId == 12
                          group prod by prod.District.City.RegionId into g
                          orderby g.Key
                          select g;

            vm.RegionsCount = new List<int>();
            foreach (var g in regions)
            {
                List<int> productIds = new List<int>();
                foreach (var item in g)
                {
                    productIds.Add(item.ProductId);
                }
                vm.RegionsCount.Add(productIds.Distinct().Count());
            }
            //計算分齡數量
            var ages = from prod in _context.Product.Include(x => x.Age).AsEnumerable()
                       where prod.StatusId == 12
                       group prod by prod.AgeId into g
                       orderby g.Key
                       select g;

            vm.AgesCount = new List<int>();
            foreach (var g in ages)
            {
                vm.AgesCount.Add(g.Count());
            }

            return Json(vm);
        }

        // 收藏功能
        public IActionResult addToPocket(int? id)
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER) || id == null)
                return Json(0);

            int memberId = (int)HttpContext.Session.GetInt32(CDictionary.SK_LOGINED_ID);

            if (_context.PocketList.Any(x => x.MemberId == memberId && x.ProductId == (int)id))
            {
                PocketList pocketListRemove = _context.PocketList.First(x => x.MemberId == memberId && x.ProductId == (int)id);
                _context.PocketList.Remove(pocketListRemove);
                _context.SaveChanges();
                return Json(1);
            }
            else
            {
                PocketList pocketList = new PocketList();
                pocketList.ProductId = (int)id;
                pocketList.MemberId = memberId;
                _context.PocketList.Add(pocketList);
                _context.SaveChanges();

                return Json(2);
            }
        }

        public IActionResult PocketCardItem()
        {

            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }
            ////----------------------------------------------------卡片資料//
            var datas = from prod in _context.PocketList
                        where prod.Product.StatusId == 12 && prod.MemberId == id
                        select new CProductCardViewModel
                        {
                            ProductId = prod.ProductId,
                            ProductName = prod.Product.ProductName,
                            Features = string.IsNullOrEmpty(prod.Product.Features) ? prod.Product.ProductIntro : prod.Product.Features,
                            CategoryId = prod.Product.ProductCategories.First().SubCategory.CategoryId,
                            SubCategoryName = prod.Product.ProductCategories.First().SubCategory.SubCategoryName,
                            CityName = prod.Product.ProductDetail.Select(x => x.District.City.CityName).Distinct().Count() > 1 ? "多縣市" : prod.Product.ProductDetail.Select(x => x.District.City.CityName).First(),
                            ImagePath = prod.Product.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                            IsClass = prod.Product.IsClass ? "長期課程" : ((prod.Product.ProductDetail.FirstOrDefault().EndTime - prod.Product.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                            IsPocketed = prod.Product.PocketList.Where(x => x.MemberId == id).Any(),
                            _UnitPrice = (decimal)prod.Product.ProductDetail.Select(x => x.UnitPrice).Min(),
                            fDateMax = prod.Product.ProductDetail.Select(x => x.EndTime).Max(),
                            fDateMin = prod.Product.ProductDetail.Select(x => x.BeginTime).Min(),
                            orderCount = _context.OrderDetail.Where(x => x.ProductDetail.ProductId == prod.ProductId).Count(),
                            Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == prod.ProductId).Select(x => x.Rank).Average().GetValueOrDefault(0).ToString("0.#")
                        };

            return Json(datas.ToList());
        }

        public IActionResult getMaxPrice()
        {
            decimal max = (decimal)_context.ProductDetail.Max(x => x.UnitPrice);
            return Content(max.ToString("######"));
        }


        public IActionResult getmyOrder(int? orderId)
        {
            //int memberId = 8; //todo 先寫死
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }

            var query = _context.Order
        .Include(x => x.OrderDetail)
            .ThenInclude(x => x.ProductDetail)
        .Where(order => order != null && order.MemberId == id);
            if (orderId.HasValue)
            {
                query = query.Where(order => order.OrderId == orderId);

            }

            var datas = query
                    .OrderByDescending(order => order.OrderDetail.First().ProductDetail.BeginTime)
        .Select(order => new COrderItmeVIewModel
        {
            //ProductId = 8,

            orderId = order.OrderId,
            orderDetailId = order.OrderDetail.First().OrderDetailId,
            MemberId = id,
            ProductId = order.OrderDetail.First().ProductDetail.ProductId,
            ProductName = order.OrderDetail.First().ProductDetail.Product.ProductName,
            Features = string.IsNullOrEmpty(order.OrderDetail.First().ProductDetail.Product.Features) ? order.OrderDetail.First().ProductDetail.Product.ProductIntro : order.OrderDetail.First().ProductDetail.Product.Features,
            CategoryId = order.OrderDetail.First().ProductDetail.Product.ProductCategories.First().SubCategory.CategoryId,
            CategoryName = order.OrderDetail.First().ProductDetail.Product.ProductCategories.First().SubCategory.Category.CategoryName,
            CityName = order.OrderDetail.First().ProductDetail.District.City.CityName,
            SupplierName = order.OrderDetail.First().ProductDetail.Product.Supplier.SupplierName,
            OrderTime = order.OrderTime,
            beginTime = order.OrderDetail.First().ProductDetail.BeginTime,
            endTime = order.OrderDetail.First().ProductDetail.EndTime,
            _UnitPrice = (int)order.OrderDetail.First().ProductDetail.UnitPrice,
            OrderStatus = order.Status.Description,
            OrderDetailStatus = order.OrderDetail.First().Status.Description,
            ImagePath = order.OrderDetail.First().ProductDetail.Product.ImageList.First().ImagePath,
            Address = order.OrderDetail.FirstOrDefault().ProductDetail.Address,
            districtName = order.OrderDetail.First().ProductDetail.District.DistrictName,
            isAttend = order.OrderDetail.First().IsAttend,
            count = order.Count,
            //txtForQrcode = $"OD{order.OrderTime.Date.ToString("yyyyMMdd")}{order.OrderDetail.First().OrderDetailId.ToString("0000")}"

        });
            return Json(datas);
        }
        public IActionResult getmyOrderDetail(int? orderId)
        {
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }
            var datas = _context.OrderDetail
                .Include(x => x.Order)
                .Where(orderDetail => orderDetail != null && orderDetail.Order.MemberId == id && orderDetail.OrderId == orderId)
            .Select(orderDetail => new COrderItmeVIewModel
            {
                orderId = orderDetail.OrderId,
                MemberId = id,
                orderDetailId = orderDetail.OrderDetailId,
                ProductId = orderDetail.ProductDetail.ProductId,
                ProductName = orderDetail.ProductDetail.Product.ProductName,
                Features = string.IsNullOrEmpty(orderDetail.ProductDetail.Product.Features) ? orderDetail.ProductDetail.Product.ProductIntro : orderDetail.ProductDetail.Product.Features,
                CategoryId = orderDetail.ProductDetail.Product.ProductCategories.First().SubCategory.CategoryId,
                CategoryName = orderDetail.ProductDetail.Product.ProductCategories.First().SubCategory.Category.CategoryName,
                CityName = orderDetail.ProductDetail.District.City.CityName,
                SupplierName = orderDetail.ProductDetail.Product.Supplier.SupplierName,
                SupplierId = orderDetail.ProductDetail.Product.Supplier.SupplierId,
                OrderTime = orderDetail.Order.OrderTime,
                beginTime = orderDetail.ProductDetail.BeginTime,
                endTime = orderDetail.ProductDetail.EndTime,
                _UnitPrice = (int)orderDetail.ProductDetail.UnitPrice,
                OrderStatus = orderDetail.Order.Status.Description,
                OrderDetailStatus = orderDetail.Status.Description,
                ImagePath = orderDetail.ProductDetail.Product.ImageList.First().ImagePath,
                Address = orderDetail.ProductDetail.Address,
                districtName = orderDetail.ProductDetail.District.DistrictName,
                isAttend = orderDetail.IsAttend,
                count = orderDetail.Order.Count,
            });
            return Json(datas);
        }

        public IActionResult now()
        {
            // Get current server time
            DateTime currentTime = DateTime.Now;

            // Return current time as JSON
            return Json(new { currentTime = currentTime });

        }

        // 綠界金流
        private static IMemoryCache cache /*= MemoryCache.Default()*/;

        [HttpPost]
        [Route("Api/AddPayInfo")]
        public HttpResponseMessage AddPayInfo(JObject info)
        {
            try
            {
                cache.Set(info.Value<string>("MerchantTradeNo"), info, DateTime.Now.AddMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }

        [HttpPost]
        [Route("Api/AddAccountInfo")]
        public HttpResponseMessage AddAccountInfo(JObject info)
        {
            try
            {
                cache.Set(info.Value<string>("MerchantTradeNo"), info, DateTime.Now.AddMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpPost]
        [Route("Api/AddOrders")]
        public string AddOrders(CStorage json)
        {
            string num = "0";
            try
            {
                Ecpay Orders = new Ecpay();
                Orders.MemberId = json.MerchantID;
                Orders.MerchantTradeNo = json.MerchantTradeNo;
                Orders.RtnCode = 0; //未付款
                Orders.RtnMsg = "訂單成功尚未付款";
                Orders.TradeNo = json.MerchantID; //.ToString();
                Orders.TradeAmt = json.TotalAmount;
                Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
                Orders.PaymentType = json.PaymentType;
                Orders.PaymentTypeChargeFee = "0";
                Orders.TradeDate = json.MerchantTradeDate;
                Orders.SimulatePaid = 0;
                _context.Ecpay.Add(Orders);
                _context.SaveChanges();
                num = "OK";
            }
            catch (Exception ex)
            {
                num = ex.ToString();
            }
            return num;
        }

        [HttpPost]
        public IActionResult PayInfo(FormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }

            string temp = id["MerchantTradeNo"];
            var ecpayOrder = _context.Ecpay.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                _context.SaveChanges();
            }
            return View("EcpayView", data);
        }
        /// step5 : 取得虛擬帳號 資訊
        [HttpPost]
        public IActionResult AccountInfo(FormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }

            string temp = id["MerchantTradeNo"];
            var ecpayOrder = _context.Ecpay.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                _context.SaveChanges();
            }
            return View("EcpayView", data);
        }

        public IActionResult EcpayView()
        {
            return View();

        }

        public IActionResult getCourses(/*int? id, DateTime date*/)
        {
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }
            var datas = from order in _context.Order
                        .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.ProductDetail)
                        where order != null && order.MemberId == id /*&& order.OrderDetail.First().ProductDetail.BeginTime >= date && order.OrderDetail.First().ProductDetail.EndTime <= date*/
                        orderby order.OrderDetail.First().ProductDetail.BeginTime descending

                        select new CMyScheduleViewModel
                        {
                            //ProductId = 8,
                            orderId = order.OrderId,
                            ProductName = order.OrderDetail.First().ProductDetail.Product.ProductName,
                            Feature = string.IsNullOrEmpty(order.OrderDetail.First().ProductDetail.Product.Features) ? order.OrderDetail.First().ProductDetail.Product.ProductIntro : order.OrderDetail.First().ProductDetail.Product.Features,
                            beginTime = order.OrderDetail.First().ProductDetail.BeginTime,
                            endTime = order.OrderDetail.First().ProductDetail.EndTime,
                            OrderStatus = order.Status.Description,
                            supplierId = order.OrderDetail.First().ProductDetail.Product.SupplierId,

                            location = order.OrderDetail.First().ProductDetail.District.City.CityName + order.OrderDetail.First().ProductDetail.District.DistrictName + order.OrderDetail.FirstOrDefault().ProductDetail.Address,
                        };
            return Json(datas);
        }
        public IActionResult getschedule(/*int? id*/)
        {
            int id = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                id = customer.MemberId;
            }

            List<CMyScheduleViewModel> cMyScheduleViewModels = new List<CMyScheduleViewModel>();
            var datas = from order in _context.Order
                        .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.ProductDetail)
                        where order != null && order.MemberId == id/* && order.OrderDetail.First().ProductDetail.BeginTime >= date && order.OrderDetail.First().ProductDetail.EndTime <= date*/
                        orderby order.OrderDetail.First().ProductDetail.BeginTime descending

                        select new CMyScheduleViewModel
                        {
                            productId = order.OrderDetail.First().ProductDetail.Product.ProductId,
                            productDetailId = order.OrderDetail.First().ProductDetail.ProductDetailId,
                            orderId = order.OrderId,
                            ProductName = order.OrderDetail.First().ProductDetail.Product.ProductName,
                            Feature = string.IsNullOrEmpty(order.OrderDetail.First().ProductDetail.Product.Features) ? order.OrderDetail.First().ProductDetail.Product.ProductIntro : order.OrderDetail.First().ProductDetail.Product.Features,
                            beginTime = order.OrderDetail.First().ProductDetail.BeginTime,
                            endTime = order.OrderDetail.First().ProductDetail.EndTime,
                            OrderStatus = order.Status.Description,
                            supplierId = order.OrderDetail.First().ProductDetail.Product.SupplierId,
                            isClass = order.OrderDetail.First().ProductDetail.Product.IsClass,
                            location = order.OrderDetail.First().ProductDetail.District.City.CityName + order.OrderDetail.First().ProductDetail.District.DistrictName + order.OrderDetail.FirstOrDefault().ProductDetail.Address,

                        };
            cMyScheduleViewModels.AddRange(datas);
            foreach (var data in datas)
            {
                if(data.isClass == true)
                {
                    var getdata = getisClassInfo(data.productId, data.productDetailId, data.orderId, data.ProductName, data.Feature,data.supplierId, data.location ,data.OrderStatus);
                    if(getdata != null)    
                    cMyScheduleViewModels.AddRange(getdata);
                }
            }
            
            return Json(cMyScheduleViewModels);
        }
        
        public IQueryable<CMyScheduleViewModel> getisClassInfo(int productId, int productDetailId, int orderId, string productName,string feature ,int supplierId,string location,string OrderStatus)
        {
            if (_context.ProductDetail.Any(x => x.ClassId == productDetailId) == false) { return null; }

            var datas = from prod in _context.ProductDetail
                        where prod.ClassId == productDetailId
                        select new CMyScheduleViewModel {
                            productId = prod.Product.ProductId,
                            productDetailId = productDetailId,
                            orderId = orderId,
                            supplierId = supplierId,
                            ProductName = prod.Product.ProductName,
                            Feature = feature,
                            beginTime = prod.BeginTime,
                            endTime = prod.EndTime,
                            OrderStatus = OrderStatus,
                            location = location,
                            isClass = true
                            };
            
            return datas;

        }
        public IActionResult getProductDetail(int? id)
        {
            var datas = _context.ProductDetail
                .Where(x => x.ProductId == id)
                .Select(x => new CProductDetailForCalendar
                {
                    ProductDetailId = x.ProductDetailId,
                    ClassId = x.ClassId,
                    BeginTime = x.BeginTime,
                    EndTime = x.EndTime,
                    Dealine = x.Dealine,
                    CityName = x.District.City.CityName,
                    DistrictName = x.District.DistrictName,
                    Address = x.Address,
                    StatusId = x.StatusId,
                    StatusDescription = x.Status.Description,
                    StockNow = x.Stock/* - x.OrderDetail.Count()*/,
                    UnitPrice = x.UnitPrice
                });
            return Json(datas.ToList());
        }

        public IActionResult createComment(Comment comment, IFormFile file)
        {

            //存入實體路徑
            string photoName = Guid.NewGuid().ToString() + ".jpg";  //產生不重複檔名
            string filepath = Path.Combine(_host.WebRootPath, "img/commentimg", photoName);
            using (var filestream = new FileStream(filepath, FileMode.Create))
            {
                file.CopyTo(filestream);
            }
            comment.ImagePath = photoName;
            _context.Comment.Add(comment);
            _context.SaveChanges();
            return Content("新增成功");
        }

# region 找家人
        public IActionResult myFamily(int id)
        {
            var member = _context.CustomerInfomation.Find(id);
            List<CCheckOutFamilyViewModel> customer = new List<CCheckOutFamilyViewModel>();
            FindParent(customer, member.MemberId, member.ParentId/*, 0*/);

            return Json(customer);


            //var datas = from mem in _context.CustomerInfomation
            //            join mmm in _context.CustomerInfomation
            //            on mem.MemberId equals mmm.MemberId
            //            where mem.ParentId == id
            //            select mem;
            //            return Json(datas.ToList());
        }

        private void FindParent(List<CCheckOutFamilyViewModel> list, int NowMemberID, int? NowParentId/*, int nowLevel*/)
        {
            CustomerInfomation parent = null;
            if (NowParentId != null)
                parent = _context.CustomerInfomation.Where(p => p.MemberId == NowParentId).Select(p => p).First();

            CustomerInfomation my = _context.CustomerInfomation.Where(m => m.MemberId == NowMemberID).Select(m => m).First();

            //找到最上層的家長
            if (parent == null)
            {
                CCheckOutFamilyViewModel family = new CCheckOutFamilyViewModel();
                family.MemberId = my.MemberId;
                family.Name = my.Name;
                family.BirthDate = my.BirthDate;
                family.ResidentId = my.ResidentId;
                family.Email= my.Email;

                list.Add(family);

                FindChild(list, NowMemberID/*, nowLevel + 1*/);
                return;
            }
            else
            {
                NowMemberID = (int)NowParentId;

                if (parent != null)
                    NowParentId = parent.ParentId;
                else
                    NowParentId = null;
                FindParent(list, NowMemberID, NowParentId/*, nowLevel - 1*/);
            }
        }

        private void FindChild(List<CCheckOutFamilyViewModel> list, int NowMemberID/*, int nowLevel*/)
        {
            List<CustomerInfomation> childList = _context.CustomerInfomation.Where(c => c.ParentId == NowMemberID).Select(c => c).ToList();

            if (childList.Count == 0)
                return;
            else
            {
                foreach (var child in childList)
                {
                    CCheckOutFamilyViewModel family = new CCheckOutFamilyViewModel();
                    family.MemberId = child.MemberId;
                    family.Name = child.Name;
                    family.BirthDate = child.BirthDate;
                    family.ResidentId = child.ResidentId;
                    family.Email = child.Email;

                    list.Add(family);
                    FindChild(list, child.MemberId/*, nowLevel + 1*/);
                }
            }
        }

        #endregion

        public IActionResult myCoupon(int id, int pid)
        {
            var datas = from cou in _context.MemberCoupon
                           .Include(x => x.Coupon)
                        where cou.MemberId == id && cou.StatusId == 21 && ((DateTime)cou.Coupon.DueDate > DateTime.Now) && cou.Coupon.ProductId == pid
                        select new CMemberCouponViewModel
                        {
                            MemberCouponId = cou.MemberCouponId,
                            CouponName = cou.Coupon.Name,
                            CouponDescription = cou.Coupon.Description,
                            CouponDiscount = (decimal)cou.Coupon.Discount
                        };

            return Json(datas.ToList());
        }

        public IActionResult myBonus()
        {

            return View();
        }

        public IActionResult getComments()
        {
            var datas = from c in _context.Comment
                        let prodName = c.Order.OrderDetail.First().ProductDetail.Product.ProductName
                        orderby c.Rank descending
                        select new
                        {
                            Name = string.IsNullOrEmpty(c.Member.Nickname) ? c.Member.Name.Substring(0, 1) + "**" : c.Member.Nickname,
                            Icon = Convert.ToBase64String(c.Member.Photo),
                            ProductName = prodName.Length > 10 ? prodName.Substring(0, 10) + "..." : prodName,
                            ProductId = c.Order.OrderDetail.First().ProductDetail.ProductId,
                            Rank = c.Rank.Value.ToString("#.#"),
                            Comment = c.Review,
                            Date = c.Date.HasValue ? c.Date.Value.ToString("yyyy/MM/dd") : "Null",
                            ImagePath = c.ImagePath
                        };

            return Json(datas.Take(6).ToList());
        }
        public IActionResult getCommentsByDate()
        {
            var datas = from c in _context.Comment
                        let prodName = c.Order.OrderDetail.First().ProductDetail.Product.ProductName
                        where c.Rank > 3
                        orderby c.Date descending
                        select new
                        {
                            Name = string.IsNullOrEmpty(c.Member.Nickname) ? c.Member.Name.Substring(0, 1) + "**" : c.Member.Nickname,
                            Icon = Convert.ToBase64String(c.Member.Photo),
                            ProductName = prodName.Length > 10 ? prodName.Substring(0, 10) + "..." : prodName,
                            ProductId = c.Order.OrderDetail.First().ProductDetail.ProductId,
                            Rank = c.Rank.Value.ToString("#.#"),
                            Comment = c.Review,
                            Date = c.Date.HasValue ? c.Date.Value.ToString("yyyy/MM/dd") : "Null",
                            ImagePath = c.ImagePath
                        };

            return Json(datas.ToList());
        }

        //孝弘測試用(回傳coupon的JSON在include時會出現無窮迴圈造成回傳錯誤，不能直接回傳cou，把要回傳的資料塞進另外一個View Model中)...
        //public IActionResult MemberCoupon(int id, int pid)
        //{
        //    var datas = from cou in _context.MemberCoupon
        //                .Include(x => x.Coupon)
        //                where cou.MemberId == id  && cou.StatusId == 21 && ((DateTime)cou.Coupon.DueDate > DateTime.Now) && cou.Coupon.ProductId == pid
        //                select new CMemberCouponViewModel
        //                {
        //                    MemberCouponId = cou.MemberCouponId,
        //                };

        //    return Json(datas.ToList());
        //}

        public IActionResult getCusInfoForChat(int customerid)
        {
            var datas = from i in _context.CustomerInfomation.Where(c => c.MemberId == customerid)
                        select i;
            return Json(datas);
        }
        public IActionResult getSupInfoForChat(int supplierid)
        {
            var datas = from i in _context.Supplier.Where(c => c.SupplierId == supplierid)
                        select new { i.SupplierId, i.SupplierName, i.LogoImage };
            return Json(datas);
        }

        public IActionResult getCustomerMessage(string userid)
        
            {
            int customerid = 0;
            int supplierid = 0;
            List<CChatHistory> datas = new List<CChatHistory>();
            if (userid.Substring(0, 1) == "C")
            {
                customerid = int.Parse(userid.Substring(1));
                datas = (from i in _context.Chat.Where(s => s.CustomerId == customerid)
                            select new CChatHistory
                            {
                                ChatRoomId = i.ChatRoomId,
                                CustomerId = i.CustomerId,
                                SupplierId = i.SupplierId,
                                MessageContent = i.MessageContent,
                                MessageCreateTime = i.MessageCreateTime,
                                customerName = i.Customer.Name,
                                customerPhoto = i.Customer.Photo,
                                supplierName = i.Supplier.SupplierName,
                                supplierPhoto = i.Supplier.LogoImage,
                                WhoSend = i.WhoSend

                            }).ToList();
            }
            else
            {
                supplierid = int.Parse(userid.Substring(1));
                datas = (from i in _context.Chat.Where(s => s.SupplierId == supplierid)
                            select new CChatHistory
                            {
                                ChatRoomId = i.ChatRoomId,
                                CustomerId = i.CustomerId,
                                SupplierId = i.SupplierId,
                                MessageContent = i.MessageContent,
                                MessageCreateTime = i.MessageCreateTime,
                                customerName = i.Customer.Name,
                                customerPhoto = i.Customer.Photo,
                                supplierName = i.Supplier.SupplierName,
                                supplierPhoto = i.Supplier.LogoImage,
                                WhoSend = i.WhoSend

                            }).ToList();
            }
            
            return Json(datas);
        }



    }
}
