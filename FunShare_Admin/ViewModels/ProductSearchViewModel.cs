using Microsoft.AspNetCore.Mvc.Rendering;

namespace FunShare_Admin.ViewModels
{
    public class ProductSearchViewModel
    {
        public string? textProductName { get; set; }
        public string? textSupplierName { get; set; }
        public int? textUnitPrice { get; set; }
        public SelectList? IntervalId { get; set; }
        public DateOnly? textReleaseDate { get; set; }
        public bool boolIsClass { get; set; }

    }
}
