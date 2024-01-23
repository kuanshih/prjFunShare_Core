using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Identity.Client.Extensions.Msal;
using prjFunShare_Core.Models;
using prjFunShare_Core.ViewModels;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;


namespace prjFunShare_Core.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment Environment;
        private readonly FUNShareContext _context;
        public ProductController(IWebHostEnvironment e, FUNShareContext context, IWebHostEnvironment host, IConfiguration configuration)
        {
            Environment = e;
            _context = context;
            _host = host;
            _configuration = configuration;
        }

        [HttpGet]
        [ResponseCache(Duration = 30)]
        public IActionResult Search(string? keyword)
        {
            ViewBag.Keyword = keyword;

            CProductSearchViewModel vm = new CProductSearchViewModel();

            vm.PriceMax = (decimal)_context.ProductDetail.Max(x => x.UnitPrice);
            vm.Categories = _context.Categories.Distinct().ToList();
            vm.Regions = _context.Region.Distinct().ToList();
            vm.Citys = _context.City.Distinct().ToList();
            vm.Ages = _context.Age.Distinct().ToList();
            vm.OldestDate = (DateTime)_context.ProductDetail.Select(x => x.BeginTime).Min();
            vm.LatestDate = (DateTime)_context.ProductDetail.Select(x => x.EndTime).Max();

            return View(vm);
        }

        public IActionResult Crop()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Save(string intro, int id)
        {
            //存入實體路徑
            string photoName = Guid.NewGuid().ToString() + ".jpg";  //產生不重複檔名

            string base64 = Request.Form["imgCropped"];
            byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);

            string filePath = Path.Combine(this.Environment.WebRootPath, "img", "product", photoName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            //int id = 1;

            ImageList image = new ImageList();
            image.ProductId = id;
            image.ImagePath= photoName;
           // image.Images = bytes;
            image.IsMain = false;
            _context.ImageList.Add(image);

            var prod = _context.Product.Find(id);
            prod.ProductIntro = intro;


            _context.SaveChanges();

            return RedirectToAction("Crop");
        }

        //public IActionResult List()
        //{
        //    return View();
        //}
        //[HttpPost]


        //首頁點選任意產品進入產品頁，include/then include其他資料表
        public IActionResult List(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");

            var prod = from o in _context.Product.Where(p => p.ProductId == id)
                       .Include(p => p.ProductDetail)
                       .ThenInclude(pp => pp.District)
                       .ThenInclude(ppp => ppp.CustomerInfomation)
                       .ThenInclude(pppp => pppp.OrderDetail)
                       .ThenInclude(ppppp => ppppp.Order)
                       .ThenInclude(pppppp => pppppp.Bonus)

                        .Include(p => p.ProductDetail)
                        .ThenInclude(pp => pp.District)
                        .ThenInclude(ppp => ppp.CustomerInfomation)
                        .ThenInclude(pppp => pppp.MemberCoupon)
                        .ThenInclude(ppppp => ppppp.Coupon)

                        .Include(p => p.ImageList)      //圖片

                        .Include(p => p.Age)                //年齡

                        .Include(p => p.PocketList)     //收藏

                        .Include(p => p.Interval)         //區間

                       .Include(p => p.Supplier)         //供應商
                       select new CProductPageViewModel
                       {
                           ProductId = o.ProductId,
                           ProductName = o.ProductName,
                           SupplierId = o.SupplierId,
                           SupplierName = o.Supplier.SupplierName,
                           SupplierIntro = o.Supplier.Description,
                           //特色、介紹都要
                           ProductIntro = o.ProductIntro,
                           Features = o.Features,
                           AgeGrade = o.Age.Grade,
                           CategoryId = o.ProductCategories.First().SubCategory.CategoryId,
                           CategoryName = o.ProductCategories.First().SubCategory.Category.CategoryName,
                           //todo 理論上也要倒第二分類
                           SubCategoryName = o.ProductCategories.First().SubCategory.SubCategoryName,
                           //要show全部城市
                           CityName = o.ProductDetail.OrderBy(x => x.District.CityId).Select(x => x.District.City.CityName).Distinct().ToArray(),
                           //圖片要分主圖和小圖
                           MainImage = o.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                           ImagePath = o.ImageList.Where(x => x.IsMain == false).Select(x => x.ImagePath).ToArray(),
                           SupplierImage = o.Supplier.LogoImage,
                           IsClass = o.IsClass ? "長期課程" : ((o.ProductDetail.FirstOrDefault().EndTime - o.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                           _UnitPrice = (int)o.ProductDetail.Select(x => x.UnitPrice).Min(),
                           Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Select(x => x.Rank).Average(),
                           // 評價總數
                           RankCount = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Count(),
                           // 長期課程頻率
                           Interval = o.IsClass ? o.Interval.IntervalDescription:null,
                           Times = o.IsClass ? o.Times.ToString():null
                       };

            return View(prod.FirstOrDefault());
            ////Product prod = _context.Product.FirstOrDefault(x => x.ProductId == id);
            //var prod = from o in _context.Product.Where(p => p.ProductId ==id)
            //           .Include(p => p.ProductDetail)
            //           .ThenInclude(pp => pp.District)
            //           .ThenInclude(ppp => ppp.CustomerInfomation)
            //           .ThenInclude(pppp => pppp.OrderDetail)
            //           .ThenInclude(ppppp => ppppp.Order)
            //           .ThenInclude(pppppp => pppppp.Bonus)

            //            .Include(p => p.ProductDetail)
            //            .ThenInclude(pp => pp.District)
            //            .ThenInclude(ppp => ppp.CustomerInfomation)
            //            .ThenInclude(pppp => pppp.MemberCoupon)
            //            .ThenInclude(ppppp => ppppp.Coupon)

            //            .Include(p => p.ImageList)      //圖片

            //            .Include(p => p.Age)                //年齡

            //            .Include(p => p.PocketList)     //收藏

            //            .Include(p => p.Interval)         //區間

            //           .Include(p => p.Supplier)         //供應商
            //            select new CProductPageViewModel
            //            {
            //                ProductId = o.ProductId,
            //                ProductName = o.ProductName,
            //                SupplierId = o.SupplierId,
            //                SupplierName = o.Supplier.SupplierName,
            //                SupplierIntro = o.Supplier.Description,

            //                //特色、介紹都要
            //                ProductIntro = o.ProductIntro,
            //                Features = o.Features,
            //                AgeGrade = o.Age.Grade,
            //                CategoryId = o.ProductCategories.First().SubCategory.CategoryId,
            //                CategoryName = o.ProductCategories.First().SubCategory.Category.CategoryName,
            //                SubCategoryName = o.ProductCategories.First().SubCategory.SubCategoryName,
            //                //要show全部城市
            //                CityName = o.ProductDetail.Select(x => x.District.City.CityName).Distinct().ToArray(),
            //                //圖片要分主圖和小圖
            //                MainImage = o.ImageList.Where(x => x.IsMain == true).First().ImagePath,
            //                ImagePath = o.ImageList.Where(x => x.IsMain == false).Select(x => x.ImagePath).ToArray(),
            //                SupplierImage = o.Supplier.LogoImage,
            //                IsClass = o.IsClass ? "長期課程" : ((o.ProductDetail.FirstOrDefault().EndTime - o.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
            //                _UnitPrice = (int)o.ProductDetail.Select(x => x.UnitPrice).Min(),
            //                Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Select(x => x.Rank).Average(),
            //                // 評價總數
            //                RankCount = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Count(),
            //            };

            //if(id==null)
            //   return RedirectToAction("Index", "Home");
            //else
            //    return View(prod.FirstOrDefault());
        }

        public IActionResult NewList(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");

            var prod = from o in _context.Product.Where(p => p.ProductId == id)
                       .Include(p => p.ProductDetail)
                       .ThenInclude(pp => pp.District)
                       .ThenInclude(ppp => ppp.CustomerInfomation)
                       .ThenInclude(pppp => pppp.OrderDetail)
                       .ThenInclude(ppppp => ppppp.Order)
                       .ThenInclude(pppppp => pppppp.Bonus)

                        .Include(p => p.ProductDetail)
                        .ThenInclude(pp => pp.District)
                        .ThenInclude(ppp => ppp.CustomerInfomation)
                        .ThenInclude(pppp => pppp.MemberCoupon)
                        .ThenInclude(ppppp => ppppp.Coupon)

                        .Include(p => p.ImageList)      //圖片

                        .Include(p => p.Age)                //年齡

                        .Include(p => p.PocketList)     //收藏

                        .Include(p => p.Interval)         //區間

                       .Include(p => p.Supplier)         //供應商
                       select new CProductPageViewModel
                       {
                           ProductId = o.ProductId,
                           ProductName = o.ProductName,
                           SupplierId = o.SupplierId,
                           SupplierName = o.Supplier.SupplierName,
                           SupplierIntro = o.Supplier.Description,
                           //特色、介紹都要
                           ProductIntro = o.ProductIntro,
                           Features = o.Features,
                           AgeGrade = o.Age.Grade,
                           CategoryId = o.ProductCategories.First().SubCategory.CategoryId,
                           CategoryName = o.ProductCategories.First().SubCategory.Category.CategoryName,
                           //todo 理論上也要倒第二分類
                           SubCategoryName = o.ProductCategories.First().SubCategory.SubCategoryName,
                           //要show全部城市
                           CityName = o.ProductDetail.OrderBy(x=>x.District.CityId).Select(x => x.District.City.CityName).Distinct().ToArray(),
                           //圖片要分主圖和小圖
                           MainImage = o.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                           ImagePath = o.ImageList.Where(x => x.IsMain == false).Select(x => x.ImagePath).ToArray(),
                           SupplierImage = o.Supplier.LogoImage,
                           IsClass = o.IsClass ? "長期課程" : ((o.ProductDetail.FirstOrDefault().EndTime - o.ProductDetail.FirstOrDefault().BeginTime).Value.Days > 1 ? "多日體驗" : "單日體驗"),
                           _UnitPrice = (int)o.ProductDetail.Select(x => x.UnitPrice).Min(),
                           Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Select(x => x.Rank).Average(),
                           // 評價總數
                           RankCount = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == o.ProductId).Count(),
                       };

                return View(prod.FirstOrDefault());
        }



        [HttpGet]
        //我是購物車畫面
        public IActionResult Order(CSelectedProductDetail vm)
        {
            int memberId = 0;
            //int memberId = 1;
            //vm.ProductDetailId
            //先判斷是否登入，未登入導向登入頁
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
                return RedirectToAction("Login", "Home");

            //若已登入，取得session存放的customer id
            else 
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                memberId = customer.MemberId;
                CustomerInfomation c = _context.CustomerInfomation.Find(memberId);                                                  

            //已登入，傳客戶ID，產品detail ID和選擇時段/數量到結帳頁面
            var prod = from o in _context.ProductDetail.Where(p => p.ProductDetailId == vm.ProductDetailId)
                     .Include(p => p.Product)
                     .ThenInclude(pp => pp.ImageList)

                      .Include(p => p.District)
                      .ThenInclude(pp=>pp.CustomerInfomation)
                      .ThenInclude(pppp => pppp.OrderDetail)
                      .ThenInclude(ppppp => ppppp.Order)


                       .Include(pp => pp.District)
                       .ThenInclude(ppp => ppp.CustomerInfomation)
                       .ThenInclude(pppp => pppp.MemberCoupon)
                       .ThenInclude(ppppp => ppppp.Coupon)

                      .Include(p=>p.Status)

                      .Include(p => p.OrderDetail)
                    
                       select new CCartViewModel
                       {
                           ProductId = o.ProductId,
                           ImagePath = o.Product.ImageList.Where(x => x.IsMain == true).First().ImagePath,
                           ProductName = o.Product.ProductName,
                           CityName = o.District.City.CityName,
                           BeginTime = o.BeginTime,
                           _UnitPrice = (int)o.UnitPrice,
                            //Points = o.District.CustomerInfomation.Where(x=>x.MemberId ==memberId).FirstOrDefault().Bonus.FirstOrDefault().Points,
                           Points= _context.Bonus.Where(x=>x.MemberId==memberId).Select(x=>x.Points).Sum(),
                           MemberId = memberId,
                           Name = c.Name,
                           PhoneNumber = c.PhoneNumber,
                           Email = c.Email,
                           ResidentId = c.ResidentId,
                           BirthDate = c.BirthDate,
                           Count=vm.txtCount,
                           ProductDetailId=vm.ProductDetailId,

                       };
            if (prod == null)
                return RedirectToAction("List");
            else              
                return View(prod.FirstOrDefault());
            }
        }

        [HttpPost]
        //成立訂單
        public IActionResult Order(CCheckOutViewModel co)
        {
            int memberId = 0;
            string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
            memberId = customer.MemberId;
            CustomerInfomation c = _context.CustomerInfomation.Find(memberId);

            //1. 成立訂單，填入購買人ID
            Order order = new Order();
            MemberCoupon mc= new MemberCoupon();
            ProductDetail prod = new ProductDetail();
            Bonus bonus = new Bonus();

            order.OrderTime = DateTime.Now;
            order.MemberId = c.MemberId;
            order.StatusId = 4;
            order.Amount = Convert.ToDecimal(co.Amount);
            order.BillingAmount = Convert.ToDecimal(co.BillingAmount);
            order.Count = co.Count;
            _context.Order.Add(order);   //可成功寫入訂單

            mc = _context.MemberCoupon.Where(x => x.CouponId == co.CouponId).Select(x => x).FirstOrDefault();
            prod = _context.ProductDetail.Where(x => x.ProductDetailId==co.ProductDetailId).Select(x => x).FirstOrDefault();
         
            //使用coupon後，MemberCoupon狀態改為已使用
            if (mc != null )
            {             
                order.CouponId = co.CouponId;              
                mc.StatusId = 22;
                _context.MemberCoupon.Update(mc);  //檢查是否更新使用狀態
                _context.SaveChanges();
            }
    
            //ProductDetail裡面扣掉產品庫存
            if (prod!=null)
            {
                prod.Stock = prod.Stock - co.Count;  
                 _context.ProductDetail.Update(prod);    //檢查是否更新庫存
                _context.SaveChanges();
            }

            //結帳金額每滿100元給1點紅利點數存進Bonus
            if(order.BillingAmount>100)
            {
                bonus.MemberId = memberId;
                bonus.Points  =  (int)Math.Round((decimal)(order.BillingAmount / 100));
                bonus.OrderId= order.OrderId;
                bonus.EndDate = (DateTime.Today).AddYears(1);

                _context.Bonus.Add(bonus);
                _context.SaveChanges();
            }

            _context.SaveChanges();

                               
            //2. 課程參與人資訊要存入Order_Detail
            for (int i = 0; i < co.OrderMemberId.Length; i++)
            {
                OrderDetail orderDetail = new OrderDetail();
                CustomerInfomation cust = new CustomerInfomation();
                //先檢查是否已經有註冊家人              
                //若傳入之co.MemberId[i]==null，表示會員未註冊，直接將此上課人員資訊註冊
                if (co.OrderMemberId[i]==null)
                {
                    //註冊會員
                    cust.Name = co.OrderName[i];
                    cust.PhoneNumber = co.OrderPhoneNumber[i];
                    cust.Email = co.OrderEmail[i];
                    cust.ResidentId = co.OrderResidentId[i];
                    cust.BirthDate = co.OrderBirthDate[i];
                    cust.Password = co.OrderResidentId[i];   
                    _context.CustomerInfomation.Add(cust);
                    _context.SaveChanges();

                    //註冊完後，將此人加入Order_Detail
                    orderDetail.ProductDetailId = co.ProductDetailId;
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.MemberId =cust.MemberId;
                    orderDetail.StatusId = 8;
                    //orderDetail.IsAttend = false;
                    _context.OrderDetail.Add(orderDetail);
                    _context.SaveChanges();
                }
                //若customerinformation裡面已有這位家人的MemberId，則直接寫入Order_Detail
                else
                {
                    orderDetail.ProductDetailId = co.ProductDetailId;
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.MemberId = co.OrderMemberId[i];
                    orderDetail.StatusId = 8;
                    //orderDetail.IsAttend = false;
                    _context.OrderDetail.Add(orderDetail);
                    _context.SaveChanges();                 
                }
            }
            //寄送Email需要Order ID
            return Content(order.OrderId.ToString());
        }


        //我是結帳最後成功畫面
        public IActionResult OrderSubmitted()
        {         
            return View();
        }

        // ATM 付款
        public IActionResult OrderSubmittedATM(string billingAmount)
        {
            CATMViewModel atm = new CATMViewModel();
            atm.BillingAmount = billingAmount;

            return View(atm);
        }


        public IActionResult ContractForm() 
        {
            return View();
        }

        #region 綠界金流
        // 綠界金流
        public IActionResult ECPay(string prouctName, string billingAmount)
        {
            // 製作FunShare訂單編號
            DateTime orderTime = DateTime.Today;
            string OrderDate = orderTime.ToString("yyyyMMdd");
            var lastOrder = _context.Order
                            .OrderByDescending(o => o.OrderId)
                            .FirstOrDefault();

            int LastOrder = lastOrder.OrderId + 1;
            string funshareID = $"{OrderDate}{LastOrder}";

            // 填入網址
            var website = $"https://localhost:7208";

            // 製作OrderID
            var orderID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

            var order = new Dictionary<string, string>
    {
        //綠界參數
        // 1. MerchantID 
        {"MerchantID","3002607"},

        // 2. MerchantTradeNo
        { "MerchantTradeNo",orderID},

        // 3. MerchantTradeDate 交易時間 固定格式：yyyy/MM/dd HH:mm:ss
        { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},

        // 4. 交易類型 固定格式：aio
        { "PaymentType","aio"},

        // 5. TotalAmount 僅限新台幣，不可以有小數點
        { "TotalAmount",billingAmount},

        // 6. TradeDesc 交易描述
        {"TradeDesc","FunShare Product"},

        // 7. ItemName 交易描述
        {"ItemName",  prouctName},

        // 8. ReturnURL 付款完成通知回傳網址(付款結果通知回傳網址)
        {"ReturnURL",$"{website}/Api/AddPayInfo"},

        // 9. ChoosePayment 選擇預設付款方式 預設信用卡
        {"ChoosePayment","Credit"},

        // 10. EncryptType 預設1
        {"EncryptType","1"},

        // 11. 自訂名稱欄位 非必填先留著
        // 設定FunShare訂單編號
        { "CustomField1", funshareID},
        { "CustomField2",  ""},
        { "CustomField3",  ""},
        { "CustomField4",  ""},

        // 12. ClientBackURL 返回商店
        {"ClientBackURL",$"{website}/Product/OrderSubmitted"}

    };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            // 為CheckMacValue加密
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "pwFHCqoQZGmho4w6";
            //測試用的 HashIV
            var HashIV = "EkRm7iFT261dpevs";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }


        private string GetSHA256(string value)
        {
            // EncryptType CheckMacValue加密類型
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        #endregion

        public IActionResult Supplier(int? id)
        {
            int memberId = 0;
            // 判斷是否登入
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                CustomerInfomation customer = JsonSerializer.Deserialize<CustomerInfomation>(json);
                memberId = customer.MemberId;
            }

            var datas = from prod in _context.Product
                        where prod.StatusId == 12 && prod.SupplierId==id
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
                            IsPocketed = prod.PocketList.Where(x => x.MemberId == memberId).Any(),
                            _UnitPrice = (decimal)prod.ProductDetail.Select(x => x.UnitPrice).Min(),
                            Rank = _context.Comment.Where(x => x.Order.OrderDetail.First().ProductDetail.ProductId == prod.ProductId).Select(x => x.Rank).Average().GetValueOrDefault(0).ToString("0.#"),
                            Supplier = prod.Supplier
                        };

            return View(datas);
        }
    }
}