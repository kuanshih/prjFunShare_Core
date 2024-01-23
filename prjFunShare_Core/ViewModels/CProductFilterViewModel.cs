namespace prjFunShare_Core.ViewModels
{
    public class CProductFilterViewModel
    {
        public string? Keyword { get; set; }
        // 0：單日體驗；1：多日體驗；2：長期課程
        public int? lsClassRadio { get; set; }
        // 0：全選；+CategoryId
        public int[]? CategoryCheckbox { get; set; }
        // 0：全選；+RegionId
        public int[]? RegionCheckbox { get; set; }
        public int[]? AgeCheckbox { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set;}
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
    }
}
