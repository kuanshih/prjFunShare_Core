namespace prjFunShare_Core.ViewModels
{
    public class CProductPageViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierIntro { get; set; }
        public string? ProductIntro { get; set; }
        public string? Features { get; set; }
        public string? AgeGrade { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string?[] CityName { get; set; }
        public double? Rank { get; set; }
        public int RankCount { get; set; }
        public string? MainImage { get; set; }
        public string?[] ImagePath { get; set; }
        public byte[]? SupplierImage { get; set; }
        public string IsClass { get; set; }
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
        public string? SubCategoryName { get; set; }
        public int orderCount { get; set; }
        public string Interval { get; set; }
        public string Times { get; set; }
    }
}
