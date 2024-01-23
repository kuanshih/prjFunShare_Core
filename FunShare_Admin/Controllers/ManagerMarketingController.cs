using FunShare_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FunShare_Admin.Controllers
{
    public class ManagerMarketingController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerMarketingController(FUNShareContext c)
        {
            _context = c;
        }
        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult BonusList()
        {
            var data = _context.Bonus.Include(c=>c.Member).ThenInclude(o=>o.Order);
            return View(data);
        }
        public IActionResult BonusCreate()
        {
            IEnumerable<CustomerInfomation> Cust = _context.CustomerInfomation.ToList();
            ViewBag.Cust = new SelectList(Cust, "MemberId","Name");            
            return View();
        }
        [HttpPost]
        public IActionResult BonusCreate(Bonus b)
        {           
            
            _context.Bonus.Add(b);
            _context.SaveChanges();
            return RedirectToAction("BonusList", "ManagerMarketing");
        }
        public IActionResult BonusDelete(int? id)
        {
            if (id != null)
            {
               Bonus bonus = _context.Bonus.Find(id);
                if (bonus != null)
                {
                    _context.Bonus.Remove(bonus);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("BonusList", "ManagerMarketing");
        }

        public IActionResult BonusEdit(int id)
        {
            if (id == null)
                return RedirectToAction("BonusList");
            Bonus bonus= _context.Bonus.Find(id);
            if (bonus == null)
                return RedirectToAction("BonusList");
            BonusWrap bonusWrap=new BonusWrap();
            bonusWrap.bonus = bonus;
            return View(bonusWrap);
        }
        [HttpPost]
        public ActionResult BonusEdit(BonusWrap pIn)
        {
            Bonus pDb = _context.Bonus.Find(pIn.PointsId);
            if (pDb != null)
            {            
                          
                pDb.MemberId = pIn.MemberId;
                pDb.Points = pIn.Points;
                pDb.OrderId = pIn.OrderId;
                pDb.EndDate = pIn.EndDate;
                pDb.MemberId = pIn.MemberId;               
                _context.SaveChanges();
            }
            return RedirectToAction("BonusList");
        }
        public IActionResult CouponList()
        {
            var data = _context.CouponList.Include(o=>o.Order).Include(m=>m.MemberCoupon).Include(p=>p.Product);
            return View(data);
        }
        public IActionResult CouponCreate()
        {            
            return View();
        }
        [HttpPost]
        public IActionResult CouponCreate(CouponList c)
        {

            _context.CouponList.Add(c);
            _context.SaveChanges();
            return RedirectToAction("CouponList", "ManagerMarketing");
        }
        public IActionResult CouponDelete(int? id)
        {
            if (id != null)
            {
                CouponList coupon = _context.CouponList.Find(id);
                if (coupon != null)
                {
                    _context.CouponList.Remove(coupon);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("CouponList", "ManagerMarketing");
        }
        public IActionResult CouponEdit(int? id)
        {
            if (id == null)
                return RedirectToAction("CouponList");
            CouponList coupon=_context.CouponList.Find(id);
            if (coupon == null)
                return RedirectToAction("CouponList");
            CouponListWrap couponWrap= new CouponListWrap();
            couponWrap.couponList = coupon;
            return View(couponWrap);
        }
        [HttpPost]
        public ActionResult CouponEdit(CouponListWrap pIn)
        {
            CouponList pDb = _context.CouponList.Find(pIn.CouponId);
            if (pDb != null)
            {
                pDb.Name = pIn.Name;                
                pDb.Discount =pIn.Discount;
                pDb.Description = pIn.Description;
                pDb.ProductId = pIn.ProductId;
                pDb.DueDate = pIn.DueDate;
                _context.SaveChanges();
            }
            return RedirectToAction("CouponList");
        }
        public IActionResult MemberCouponList()
        {
            var data = _context.MemberCoupon.Include(c => c.Coupon);
            return View(data);
        }
    }
}
