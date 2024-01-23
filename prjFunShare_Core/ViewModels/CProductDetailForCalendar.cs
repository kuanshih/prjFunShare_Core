namespace prjFunShare_Core.ViewModels
{
    public class CProductDetailForCalendar
    {
        public int ProductDetailId { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string DistrictName { get; set; }
        public string CityName { get; set; }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        public string Address { get; set; }

        public int? StockNow { get; set; }

        public decimal? UnitPrice { get; set; }
        public string UnitPriceString { get { return UnitPrice.GetValueOrDefault(0).ToString("###,###"); } }

        public DateTime? Dealine { get; set; }

        public int? ClassId { get; set; }
    }
}
