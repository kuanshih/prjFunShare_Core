using prjFunShare_backend.Models;

namespace prjFunShare_backend.ViewModels
{
    public class CIndexInfoViewModel
    {   
        //供應商
        public Supplier Supplier { get; set; }
        //訂單總數
        public int OrderCount { get; set; }
        //總金額
        public decimal OrderAmount { get; set; }
        //銷售趨勢
        public decimal MoM { get; set; }
        public decimal YoY{ get; set; }
        //銷售中課程
        public int ProductOnSaleCount { get; set; }
        //總課程數
        public int ProductCount { get; set; }
        //銷售課程狀況
        public decimal CSS { get; set; }
        //報到
        public decimal Attend { get; set; }
        //廠商數
        public int SupplierCount { get; set; }
        //待審核廠商
        public int SupplierToBeApproved { get; set; }
        //銷售最好的課程
        public string[] ProductName { get; set; }
    }
}
