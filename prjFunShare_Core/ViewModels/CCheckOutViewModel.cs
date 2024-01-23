namespace prjFunShare_Core.ViewModels
{
    public class CCheckOutViewModel
    {
        public decimal _UnitPrice;
        public string UnitPrice
        {
            get
            {
                return _UnitPrice.ToString("###,###");
            }
            set
            {
                _UnitPrice = decimal.Parse(value);
            }
        }
        public string Amount { get { return (this.Count * this._UnitPrice).ToString("###,###"); } }
        public string BillingAmount { get; set; }
        public int CouponId { get; set; }
        public int Count { get; set; }

        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public int BuyMemberId { get; set; }
        public string BuyName { get; set; }
        public string BuyPhoneNumber { get; set; }
        public string BuyEmail { get; set; }
        public string BuyResidentId { get; set; }
        public DateTime BuyBirthDate { get; set; }

        public int[] OrderMemberId { get; set; }
        public string[] OrderName { get; set; }
        public string[]? OrderPhoneNumber { get; set; }
        public string[] OrderEmail { get; set; }
        public string[] OrderResidentId { get; set; }
        public DateTime?[] OrderBirthDate { get; set; }

        public int Bonus { get; set; }
        public int? Points { get; set; }


    }
}
