namespace prjFunShare_Core.ViewModels
{
    public class COrderItmeVIewModel
    {
        public int? ProductId { get; set; }
        public int? MemberId { get; set; }
        public string ProductName { get; set; }
        public string? Features { get; set; }
        //public string? AgeGrade { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public string? CityName { get; set; }
        //public double? Rank { get; set; }
        public string? ImagePath { get; set; }
        //public string IsClass { get; set; }

        public decimal? _UnitPrice;
        public string UnitPrice
        {
            get
            {
                return _UnitPrice.ToString(/*"###,###"*/);
            }
            set
            {
                _UnitPrice = decimal.Parse(value);
            }
        }
        //public string? SubCategoryName { get; set; }
        //public int orderCount { get; set; }

        public int orderId { get; set; } 
        public int orderDetailId { get; set; }

        public string Address { get; set; }
        public string? districtName { get; set; }
        public string SupplierName { get; set; }
        public int ?  SupplierId { get; set; }
            
        public DateTime?  beginTime { get; set; }
        public DateTime? endTime { get; set; }
        public DateTime OrderTime { get; set; }

        public string  OrderStatus { get; set; }
        public string OrderDetailStatus { get; set; }

        public int? count { get; set; }
        public bool? isAttend { get; set; }

        public string txtForQrcode { get; set; }

    }
}
