using System.ComponentModel;

namespace prjFunShare_Core.Areas.backend.Models.ManagerProduct
{
    public class CProductAttend
    {
        [DisplayName("課程編號")]
        public int FProductDetail_ID { get; set; }
        [DisplayName("報名人姓名")]
        public string FCustomer_Name { get; set; }
        [DisplayName("報名人E-mail")]
        public string FCustomer_Email { get; set; }
        [DisplayName("報名人電話")]
        public string FCustomer_Phone { get; set; }
        [DisplayName("課程名稱")]
        public string FProduct_Name { get; set; }
        [DisplayName("課程價格")]
        public decimal? FProduct_UnitPrice { get; set; }
        [DisplayName("付款狀態")]
        public string? FOrderPay_Status { get; set; }
        [DisplayName("參加狀態")]
        public bool? FProduct_IsAttend { get; set; }//修改出席狀態使用

        [DisplayName("結束時間")]
        public DateTime? FEndtime { get; set; }
        public int FOrderDetail_ID { get; set; }
        public int Fproduct_ID { get; set; }
    }
}
