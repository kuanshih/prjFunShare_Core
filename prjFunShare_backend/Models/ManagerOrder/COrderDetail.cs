using System.ComponentModel;

namespace prjFunShare_backend.Models.ManagerOrder
{
    public class COrderDetail
    {

        [DisplayName("訂單編號")]
        public int FOrder_ID { get; set; }

        [DisplayName("訂購時間")]
        public DateTime? FOrder_Time { get; set; }

        [DisplayName("訂購狀態")]
        public int? FOrder_Status { get; set; }


        [DisplayName("身份證字號")]
        public string? FResident_ID { get; set; }

        [DisplayName("訂購人")]
        public string? FCustomer_Name { get; set; }

        [DisplayName("統編")]
        public int? FTax_ID { get; set; }

        [DisplayName("抬頭")]
        public string? FCompany_Name { get; set; }

        [DisplayName("產品名稱")]
        public string? FProduct_Name { get; set; }

        [DisplayName("產品ID")]
        public int? FProduct_ID { get; set; }

        [DisplayName("訂單金額")]
        public int? Order_Amount { get; set; }

        [DisplayName("付款狀態")]
        public int? FPayment_Status { get; set; }
    }
}
