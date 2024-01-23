namespace prjFunShare_Core.Models
{
    public class CStorage
    {
        public string MerchantID { get; set; }

        public string MerchantTradeNo { get; set; }

        public int RtnCode { get; set; }

        public string RtnMsg { get; set; }

        public string TradeDate { get; set; }

        public int TotalAmount { get; set; }

        public string MerchantTradeDate
        { get; set; }

        public string PaymentType { get; set; }

        public string PaymentTypeChargeFee { get; set; }

    }
}
