namespace prjFunShare_Core.ViewModels
{
    public class CCartViewModel
    {
        public int ProductId { get; set; }
        public int ProductDetailId { get; set; }
        public string? ImagePath { get; set; }
        public string ProductName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? CityName { get; set; }
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
        public int Count { get; set; }
        public string Amount { get { return (this.Count * this._UnitPrice).ToString("###,###"); } }

        public string BillingAmount { get; set; }
        public int CouponId { get; set; }
        public string CouponDiscription { get; set; }
        public double CouponDiscount { get; set; }
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ResidentId { get; set; }
        public DateTime? BirthDate { get; set; }

        public string IsClass { get; set; }
        public int? Points { get; set; }
    }
}
