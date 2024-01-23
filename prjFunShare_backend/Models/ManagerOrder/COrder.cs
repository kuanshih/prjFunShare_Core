using System.ComponentModel;

namespace prjFunShare_backend.Models.ManagerOrder
{
    public class COrder
    {
        [DisplayName("訂單編號")]
        public int FOrder_ID { get; set; }

        [DisplayName("訂購時間")]
        public DateTime? FOrder_Time { get; set; }

        [DisplayName("身份證字號")]
        public string? FResident_ID { get; set; }

        [DisplayName("訂購人")]
        public string? FCustomer_Name { get; set; }

        [DisplayName("產品名稱")]
        public string? FProduct_Name { get; set; }

        [DisplayName("供應商名稱")]
        public string? FSupplier_Name { get; set; }

        [DisplayName("付款狀態")]
        public string? FPayment_Status { get; set; }

        [DisplayName("訂購狀態")]
        public string? FOrder_Status { get; set; }

        [DisplayName("統一編號")]
        public int? FTax_ID { get; set; }

        [DisplayName("訂單金額")]
        public string? FAmount { get; set; }
        [DisplayName("電話號碼")]
        public string? FCustomer_PhoneNumber { get; set; }
        [DisplayName("價格")]
        public decimal? FProduct_UnitPrice { get; set; }
        [DisplayName("數量")]
        public int? FOrder_Count { get; set; }
        [DisplayName("總金額")]
        public decimal? 小計
        {
            get
            {
                if (FProduct_UnitPrice.HasValue && FOrder_Count.HasValue)
                {
                    return FProduct_UnitPrice.Value * FOrder_Count.Value;
                }
                return null;
            }
        }
        [DisplayName("付款狀態")]
        public string? FOrderPay_Status { get; set; }

        public int FOrderPay_StatusID { get; set; }//修改用訂單付款狀態
        public int FOrder_StatusID { get; set; }//修改用訂單報名狀態

    }
}
