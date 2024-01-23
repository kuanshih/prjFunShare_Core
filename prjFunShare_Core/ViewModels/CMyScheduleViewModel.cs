namespace prjFunShare_Core.ViewModels
{
    public class CMyScheduleViewModel
    {
        public int productId{get; set; }
        public  int productDetailId { get; set; }

        public string ProductName { get; set; }
        public string Feature { get; set; }
        public int orderId { get; set; }
        public DateTime? beginTime { get; set; }
        public DateTime? endTime { get; set; }
        public string OrderStatus { get; set; }

        public int count { get; set; }
        public string location { get; set; }

        public int supplierId { get; set; }

        public bool? isClass { get; set; }
    }
}
