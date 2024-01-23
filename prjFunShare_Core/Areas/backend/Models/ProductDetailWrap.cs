using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using prjFunShare_Core.Models;

namespace prjFunShare_Core.Areas.backend.Models
{
    public class ProductDetailWrap
    {
        private ProductDetail _detail = null;

        public ProductDetail productDetail
        {
            get { return _detail; }
            set { _detail = value; }
        }

        public ProductDetailWrap()
        {
            _detail = new ProductDetail();
        }

        [DisplayName("產品明細編號")]
        public int ProductDetailId {
            get { return _detail.ProductDetailId; }
            set { _detail.ProductDetailId = value; }
        }

        [DisplayName("產品編號")]
        [Required(ErrorMessage = "此為必填項目。")]
        public int ProductId {
            get { return _detail.ProductId; }
            set { _detail.ProductId = value; }
        }
        [DisplayName("開始時間"), DataType(DataType.DateTime)]
        public DateTime? BeginTime {
            get { return _detail.BeginTime; }
            set { _detail.BeginTime = value; }
        }
        [DisplayName("結束時間"), DataType(DataType.DateTime)]
        public DateTime? EndTime {
            get { return _detail.EndTime; }
            set { _detail.EndTime = value; }
        }
        [DisplayName("行政區編號")]
        public int? DistrictId {
            get { return _detail.DistrictId; }
            set { _detail.DistrictId = (int) value; }
        }
        [DisplayName("狀態編號")]
        public int StatusId {
            get { return _detail.StatusId; }
            set { _detail.StatusId = value; }
        }
        [DisplayName("課程地址")]
        public string? Address {
            get { return _detail.Address; }
            set { _detail.Address = value; }
        }
        [DisplayName("單價"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "此為必填項目。")]
        public decimal? UnitPrice {
            get { return _detail.UnitPrice; }
            set { _detail.UnitPrice = value; }
        }
        [DisplayName("截止日期"), DataType(DataType.Date)]
        public DateTime? Dealine {
            get { return _detail.Dealine; }
            set { _detail.Dealine = value; }
        }
        [DisplayName("教室編號")]
        public int? ClassId {
            get { return _detail.ClassId; }
            set { _detail.ClassId = value; }
        }

        public virtual ProductDetail? Class { get; set; }

        public virtual District? District { get; set; }

        public virtual ICollection<ProductDetail> InverseClass { get; set; } = new List<ProductDetail>();

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual Product Product { get; set; } = null!;

        public virtual Status Status { get; set; } = null!;
    }
}
