using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjFunShare_backend.Models.ManagerProduct
{
    public class CProduct
    {
        //引用商品
        [DisplayName("課程編號")]
        public int FProductId { get; set; }
        [DisplayName("課程名稱")]
        public string FProductName { get; set; }
        [DisplayName("課程介紹")]
        public string FProductIntro { get; set; }
        [DisplayName("供應商編號")]
        public string FSupplierName { get; set; }
    
        [DisplayName("分齡")]
        public string? FAgeId { get; set; }
        [DisplayName("難易度")]
        public int? FLevel { get; set; }
        [DisplayName("上架日")]
        public DateTime? FReleasedTime { get; set; }
        [DisplayName("堂數")]
        public int? FTimes { get; set; }
        //[DisplayName("週期")]
        //public int? FIntervalId { get; set; }
        //[DisplayName("備註")]
        //public string? Note { get; set; }
        [DisplayName("狀態")]
        public string FStatusDescription { get; set; }
        [DisplayName("是長期課程?")]
        public bool FIsClass { get; set; }
        [DisplayName("傭金?")]

        public double? FCommision { get; set; }
        public List<CProductDetail> CProductDetails { get; set; }

        public int? FOrderCount { get; set; }
        public int FOrder_Detail_ID { get; set; }

        public string? FProductSale_Status { get; set; }//改變顏色銷售狀態
        


    }
}
