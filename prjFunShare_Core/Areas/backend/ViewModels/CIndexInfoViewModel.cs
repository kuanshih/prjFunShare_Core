using prjFunShare_Core.Models;

namespace prjFunShare_Core.Areas.backend.ViewModels
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
    }
}
