using FunShare_Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunShare_Admin.Models
{
    public class ProductWrap
    {
        private Product _prod = null;

        public Product product
        {
            get { return _prod; }
            set { _prod = value; }
        }

        public ProductWrap()
        {
            _prod = new Product();
        }


        [DisplayName("課程編號")]
        public int ProductId
        {
            get { return _prod.ProductId; }
            set { _prod.ProductId = value; }
        }

        [DisplayName("課程名稱")]
        [Required(ErrorMessage = "此為必填項目。")]
        public string ProductName
        {
            get { return _prod.ProductName; }
            set { _prod.ProductName = value; }
        }
        [DisplayName("課程介紹")]
        public string? ProductIntro
        {
            get { return _prod.ProductIntro; }
            set { _prod.ProductIntro = value; }
        }
        [DisplayName("合作夥伴")]
        public int SupplierId { get; set; }

        [DisplayName("分齡")]
        public int? AgeId
        {
            get { return _prod.AgeId; }
            set { _prod.AgeId = value; }
        }
        

        [DisplayName("難易度")]
        public int? Level
        {
            get { return _prod.Level; }
            set { _prod.Level = value; }
        }

        [DisplayName("庫存")]
        [Required(ErrorMessage = "此為必填項目。")]
        public int? Stock
        {
            get { return _prod.Stock; }
            set { _prod.Stock = value; }
        }

        [DisplayName("上架日"), DataType(DataType.Date)]
        public DateTime? ReleasedTime
        {
            get { return _prod.ReleasedTime; }
            set { _prod.ReleasedTime = value; }
        }
        [DisplayName("堂數")]
        public int? Times
        {
            get { return _prod.Times; }
            set { _prod.Times = value; }
        }
        [DisplayName("週期")]
        public int? IntervalId
        {
            get { return _prod.IntervalId; }
            set { _prod.IntervalId = value; }
        }
        [DisplayName("備註")]
        public string? Note
        {
            get { return _prod.Note; }
            set { _prod.Note = value; }
        }

        [DisplayName("狀態")]
        [Required(ErrorMessage = "此為必填項目。")]
        public int StatusId
        {
            get { return _prod.StatusId; }
            set { _prod.StatusId = value; }
        }

        [DisplayName("是長期課程?")]
        public bool IsClass
        {
            get { return _prod.IsClass; }
            set { _prod.IsClass = value; }
        }
        [DisplayName("傭金")]
        [Required(ErrorMessage = "此為必填項目。")]
        [Column(TypeName = "decimal(18, 2)")]
        public double? Commision
        {
            get { return _prod.Commision; }
            set { _prod.Commision = value; }
        }
        [DisplayName("課程特色")]
        public string? Features { get; set; }

        public virtual ICollection<AchievementList> AchievementLists { get; set; } = new List<AchievementList>();

        public virtual ICollection<AdvertiseProductDetail> AdvertiseProductDetails { get; set; } = new List<AdvertiseProductDetail>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<ImageList> ImageLists { get; set; } = new List<ImageList>();

        public virtual IntervalList? Interval { get; set; }

        public virtual ICollection<PocketList> PocketLists { get; set; } = new List<PocketList>();

        public virtual ICollection<ProductCategories> ProductCategories { get; set; } = new List<ProductCategories>();

        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
    }
}
