using System.ComponentModel;

namespace prjFunShare_backend.Models.ManagerProduct
{
    public class CProductDetail
    {
        //引用明細
        [DisplayName("課程明細編號")]
        public int FProductDetailId { get; set; }
        [DisplayName("開始時間")]
        public DateTime? FBeginTime { get; set; }
        [DisplayName("結束時間")]
        public DateTime? FEndTime { get; set; }
        [DisplayName("行政區編號")]
        public string? FDistrictName { get; set; }
        [DisplayName("明細狀態")]
        public string FDetailStatusDescription { get; set; }
        [DisplayName("課程地址")]
        public string FAddress { get; set; }
        [DisplayName("庫存")]
        public int? FStock { get; set; }
        [DisplayName("價格")]
        public decimal? FUnitPrice { get; set; }
        [DisplayName("截止日期")]
        public DateTime? FDealine { get; set; }
        [DisplayName("教室編號")]
        public int? FClassId { get; set; }
        public int FProductId { get; set; }//產品編號,用來傳送報名資訊
        public int? FSales { get; set; }//銷售數量
        public string? FProductClass_Status { get; set; }//改變顏色課程狀態
    }
}
