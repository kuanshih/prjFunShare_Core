using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace prjFunShare_backend.Models
{
    public class TProduct
    {
        private Product _prod = null;

        public Product product
        {
            get { return _prod; }
            set { _prod = value; }
        }
        private ProductDetail _detail = null;
        private List<ProductDetail>? _ProductDetail = null;
        public ProductDetail productDetail
        {
            get { return _detail; }
            set { _detail = value; }
        }
        public List<ProductDetail> ProductDetail
        {
            get { return _ProductDetail; }
            set { _ProductDetail = value; }
        }

        private ImageList _ImageList;
        public ImageList ImageList
        {
            get { return _ImageList; }
            set { _ImageList = value; }
        }
        public TProduct()
        {
            _prod = new Product();
            _detail = new ProductDetail();
            _ProductDetail = new List<ProductDetail>();
            _ImageList = new ImageList();
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
        public int SupplierId
        {
            get { return _prod.SupplierId; }
            set { _prod.SupplierId = value; }
        }

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
        [DisplayName("區間")]
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
        public string? Features
        {
            get { return _prod.Features; }
            set { _prod.Features = value; }
        }

        public virtual Age Age { get; set; }
        public virtual IntervalList Interval { get; set; }
        public virtual Status Status { get; set; }


        public int ProductDetailId { get; set; }


        //[DisplayName("開始時間"), DataType(DataType.DateTime)]
        //public DateTime? BeginTime
        //{
        //    get { return _detail.BeginTime; }
        //    set { _detail.BeginTime = value; }
        //}
        //[DisplayName("結束時間"), DataType(DataType.DateTime)]
        //public DateTime? EndTime
        //{
        //    get { return _detail.EndTime; }
        //    set { _detail.EndTime = value; }
        //}
        //[DisplayName("行政區")]
        //public int? DistrictId
        //{
        //    get { return _detail.DistrictId; }
        //    set { _detail.DistrictId = value; }
        //}
        ////[DisplayName("狀態")]
        ////[Required(ErrorMessage = "此為必填項目。")]
        ////public int StatusId
        ////{
        ////    get { return _detail.StatusId; }
        ////    set { _detail.StatusId = value; }
        ////}

        //[DisplayName("課程地址")]
        //public string? Address
        //{
        //    get { return _detail.Address; }
        //    set { _detail.Address = value; }
        //}

        //[DisplayName("庫存")]
        //[Required(ErrorMessage = "此為必填項目。")]
        //public int? Stock
        //{
        //    get { return _prod.Stock; }
        //    set { _prod.Stock = value; }
        //}
        //[DisplayName("單價"), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(18, 2)")]
        //[Required(ErrorMessage = "此為必填項目。")]
        //public decimal? UnitPrice
        //{
        //    get { return _detail.UnitPrice; }
        //    set { _detail.UnitPrice = value; }
        //}
        //[DisplayName("截止日期"), DataType(DataType.Date)]
        //public DateTime? Dealine
        //{
        //    get { return _detail.Dealine; }
        //    set { _detail.Dealine = value; }
        //}
        //[DisplayName("教室編號")]
        //public int? ClassId
        //{
        //    get { return _detail.ClassId; }
        //    set { _detail.ClassId = value; }
        //}

        [DisplayName("開始時間"), DataType(DataType.DateTime)]
        public DateTime[] BeginTime
        {
            get; set;
        }
        [DisplayName("結束時間"), DataType(DataType.DateTime)]
        public DateTime[] EndTime
        {
            get; set;
        }
        [DisplayName("行政區")]
        public int[] DistrictId
        {
            get; set;
        }
        //[DisplayName("狀態")]
        //[Required(ErrorMessage = "此為必填項目。")]
        //public int StatusId
        //{
        //    get { return _detail.StatusId; }
        //    set { _detail.StatusId = value; }
        //}

        [DisplayName("課程地址")]
        public string[] Address
        {
            get; set;
        }

        [DisplayName("庫存")]
        [Required(ErrorMessage = "此為必填項目。")]
        public int[] Stock
        {
            get; set;
        }

        [DisplayName("單價"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "此為必填項目。")]
        public decimal[] UnitPrice
        {
            get; set;
        }
        [DisplayName("截止日期"), DataType(DataType.Date)]
        public DateTime[] Dealine
        {
            get; set;
        }
       
        
        [DisplayName("教室編號")]
        public int[] ClassId
        {
            get; set;
        }

        public int ImageId
        {
            get { return _ImageList.ImageId; }
            set { _ImageList.ImageId = value; }
        }
        
        [DisplayName("圖片路徑")]        
        public string ImagePath
        {
            get { return _ImageList.ImagePath; }
            set { _ImageList.ImagePath = value; }
        }
        [DisplayName("是橫幅")]
        public bool? IsMain
        {
            get { return _ImageList.IsMain; }
            set { _ImageList.IsMain = value; }
        }
        public IFormFile photo { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<AchievementList> AchievementList { get; set; }
        public virtual ICollection<AdvertiseProductDetail> AdvertiseProductDetail { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<CouponList> CouponList { get; set; }

        public virtual ICollection<PocketList> PocketList { get; set; }
        public virtual ICollection<ProductCategories> ProductCategories { get; set; }
        // public virtual ICollection<ProductDetail> ProductDetail { get; set; }
        public virtual ProductDetail Class { get; set; }
        public virtual District District { get; set; }

        public virtual ICollection<ProductDetail> InverseClass { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        //public DateTime[] Dealines { get; set; }
    }

    internal class Emunerable<T>
    {
        
    }
}
