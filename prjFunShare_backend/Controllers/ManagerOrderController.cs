using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;
using prjFunShare_backend.Models.ManagerOrder;
using prjFunShare_backend.ViewModels;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class ManagerOrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly FUNShareContext _context;

        public ManagerOrderController(FUNShareContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult OrderPreview(CKeyword kw, int? page, int? itemsPerPage)
        {
            //區分登入身分
            //供應商
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                Supplier supplier = JsonSerializer.Deserialize<Supplier>(json);

                var query = _context.OrderDetail
                                            .Include(o => o.Order)
                                            .Include(pd => pd.ProductDetail)
                                            .ThenInclude(p => p.Product)
                                            .ThenInclude(s => s.Supplier)
                                            .ThenInclude(st => st.Status)
                                            .Include(c => c.Member)
                                            .Where(x=>x.ProductDetail.Product.SupplierId==supplier.SupplierId)
                                            .AsQueryable();
                if (kw.txtKeyword != null)
                {
                    var keyword = kw.txtKeyword.ToUpper();
                    query = query.Where(o =>
                        o.ProductDetail.Product.ProductName.ToUpper().Contains(keyword) ||
                        o.Status.Description.ToUpper().Contains(keyword) ||
                        o.Order.Status.Description.ToUpper().Contains(keyword) ||
                        // o.Order.OrderId == int.Parse(keyword) ||
                        //o.ProductDetail.UnitPrice == int.Parse(keyword) ||
                        o.Member.Name.ToUpper().Contains(keyword)
                    );
                }
                var datas = query.Select(q => new COrder
                {
                    FOrder_ID = q.OrderId,
                    FOrder_Time = q.Order.OrderTime,
                    FCustomer_PhoneNumber = q.Member.PhoneNumber,
                    FCustomer_Name = q.Member.Name,
                    FProduct_Name = q.ProductDetail.Product.ProductName,
                    FProduct_UnitPrice = q.ProductDetail.UnitPrice,
                    FOrder_Count = q.Order.Count,
                    FOrderPay_Status = q.Order.Status.Description,
                    FOrder_Status = q.Status.Description,
                })
                .OrderByDescending(o => o.FOrder_Time);
                return View(datas);
            }
            // Admin
            else if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMIN))
            { 
                var query = _context.OrderDetail
           .Include(o => o.Order)
           .Include(pd => pd.ProductDetail)
           .ThenInclude(p => p.Product)
           .ThenInclude(s => s.Supplier)
           .ThenInclude(st => st.Status)
           .Include(c => c.Member)
           .AsQueryable();
            if (kw.txtKeyword != null)
            {
                var keyword = kw.txtKeyword.ToUpper();
                query = query.Where(o =>
                    o.ProductDetail.Product.ProductName.ToUpper().Contains(keyword) ||
                    o.Status.Description.ToUpper().Contains(keyword) ||
                    o.Order.Status.Description.ToUpper().Contains(keyword) ||
                    // o.Order.OrderId == int.Parse(keyword) ||
                    //o.ProductDetail.UnitPrice == int.Parse(keyword) ||
                    o.Member.Name.ToUpper().Contains(keyword)
                );
            }
            var datas = query.Select(q => new COrder
            {
                FOrder_ID = q.OrderId,
                FOrder_Time = q.Order.OrderTime,
                FCustomer_PhoneNumber = q.Member.PhoneNumber,
                FCustomer_Name = q.Member.Name,
                FProduct_Name = q.ProductDetail.Product.ProductName,
                FProduct_UnitPrice = q.ProductDetail.UnitPrice,
                FOrder_Count = q.Order.Count,
                FOrderPay_Status = q.Order.Status.Description,
                FOrder_Status = q.Status.Description,
            })
            .OrderByDescending(o => o.FOrder_Time);
                return View(datas);
            }

            //    int itemsPerPageValue = itemsPerPage ?? 20;//每頁顯示資料
            //int pageNumber = page ?? 1;//未提供預設為1

            //ViewBag.CurrentPage = pageNumber;
            //ViewBag.TotalPages = Math.Ceiling((double)_context.Supplier.Count() / itemsPerPageValue);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            //// 將產品顯示在下拉選單內           
            var product = from p in _context.Product
                          where p.Stock > 1
                          select p.ProductName;

            ViewBag.product = product.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(Order order, OrderDetail orderDetail, CKeyword kw)
        {
            // 訂購時間為現在
            DateTime now = DateTime.Now;
            order.OrderTime = now;

            // 付款狀態 預設：未付款
            order.StatusId = 5;

            // 新增會員資料
            if (!orderDetail.MemberId.Equals(kw.txtMemberID.ToUpper()))
            {
                return Content("未查詢到會員，請確認會員資料正確或先註冊會員!");
            }
            else
            {
                var memberInfo = _context.CustomerInfomation.FirstOrDefault(ci => ci.ResidentId == kw.txtMemberID);
                if (memberInfo != null)
                {
                    orderDetail.MemberId = memberInfo.MemberId;
                    kw.txtMemberName = memberInfo.Name;
                    ViewBag.MemberName = kw.txtMemberName;
                }
            }

            // 新增產品
            string choice = Request.Form["product"];
            var productDetail = _context.ProductDetail.FirstOrDefault(pd => pd.Product.ProductName == choice);
            if (productDetail != null)
            {
                orderDetail.ProductDetailId = productDetail.ProductDetailId;
            }

            // 訂單狀態 預設：報名成功
            orderDetail.StatusId = 8;

            _context.Order.Add(order);
            _context.OrderDetail.Add(orderDetail);
            _context.SaveChanges();

            return RedirectToAction("OrderPreview");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                RedirectToAction("OrderPreview");
            }
            var Corder = _context.OrderDetail
            .Where(s => s.OrderId == id)
           .Include(o => o.Order)
           .Include(pd => pd.ProductDetail)
           .ThenInclude(p => p.Product)
           .ThenInclude(s => s.Supplier)
           .ThenInclude(st => st.Status)
           .Include(c => c.Member)
           .Select(q => new COrder
           {
               FOrder_ID = q.OrderId,
               FOrder_Time = q.Order.OrderTime,
               FCustomer_PhoneNumber = q.Member.PhoneNumber,
               FCustomer_Name = q.Member.Name,
               FProduct_Name = q.ProductDetail.Product.ProductName,
               FProduct_UnitPrice = q.ProductDetail.UnitPrice,
               FOrder_Count = q.Order.Count,
               FOrderPay_Status = q.Order.Status.Description,
               FOrderPay_StatusID = q.Order.StatusId,
               //   FOrder_StatusID = q.StatusId,
               FOrder_Status = q.Status.Description,
           }).FirstOrDefault();

            COrderEditViewModel order = new COrderEditViewModel();
            order.COrder = Corder;
            if (order == null)
            {
                return RedirectToAction("OrderPreview");
            }
            //取得狀態的部分
            //    var customer = _context.CustomerInfomation.ToList();
            var statuses = _context.Status.Where(s => s.StatusId >= 4 && s.StatusId <= 6).ToList();
            var statusesOD = _context.Status.Where(s => s.StatusId >= 7 && s.StatusId <= 8 || s.StatusId == 18).ToList();
            //將城市和狀態選項放入 ViewBag
            order.Statuspay = statuses;
            order.Statusod = statusesOD;
            //    order.CustomerInfomation = customer;
            return View(order);

        }
        [HttpPost]
        public IActionResult Edit(COrder cor, COrderDetail ord)
        {
            Order order = _context.Order.FirstOrDefault(o => o.OrderId == ord.FOrder_ID);
            OrderDetail orderDetail = _context.OrderDetail.FirstOrDefault(od => od.OrderId == ord.FOrder_ID);
            if (order != null)
            {
                //要多什麼功能下面自己加
                order.StatusId = cor.FOrderPay_StatusID;
                // order.StatusId = cor.FOrder_StatusID;
                _context.SaveChanges();
            }
            return RedirectToAction("OrderPreview");
        }

        public IActionResult Message(int id)
        {
            var message = _context.Message
                .Where(m => m.OrderId == id).ToList();  

            CMessage m = new CMessage();
            m.MessageList = message;
            m.OrderIdForView = id;

            return View(m);
        }
        [HttpPost]
        public IActionResult Message(CMessage cid)
        {
            Message a =new Message();
            a.OrderId = cid.OrderIdForView;
            a.Datetime=DateTime.Now;
            a.MessageContent = cid.MessageForView;
            a.CustomerServiceId = 1;
            _context.Message.Add(a);
            _context.SaveChanges();
            return RedirectToAction("Message", "ManagerOrder", new { cid.OrderIdForView});
        }

    }
}
