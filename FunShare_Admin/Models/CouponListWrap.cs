using FunShare_Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FunShare_Admin.Models
{
    public class CouponListWrap
    {
        private CouponList _coupon;
        public CouponList couponList
        {
            get { return _coupon; }
            set { _coupon = value; }
        }
        public CouponListWrap()
        {
            _coupon = new CouponList();
        }
        [DisplayName("優惠券編號")]
        public int CouponId {
            get { return _coupon.CouponId; }
            set { _coupon.CouponId = value; }
        }
        [DisplayName("名稱")]
        public string Name {
            get { return _coupon.Name; }
            set { _coupon.Name = value; }
        }
        [DisplayName("折扣")]
        public decimal? Discount {
            get { return _coupon.Discount; }
            set { _coupon.Discount = value; }
        }
        [DisplayName("折扣內容")]
        public string Description {
            get { return _coupon.Description; }
            set { _coupon.Description = value; }
        }
        [DisplayName("產品編號")]
        public int? ProductId {
            get { return _coupon.ProductId; }
            set { _coupon.ProductId = value; }
        }
        [DisplayName("截止日期"), DataType(DataType.Date)]
        public DateTime? DueDate {
            get { return _coupon.DueDate; }
            set { _coupon.DueDate = value; }
        }

        public virtual ICollection<MemberCoupon> MemberCoupon { get; set; } = new List<MemberCoupon>();

        public virtual ICollection<Order> Order { get; set; } = new List<Order>();
    }
}
