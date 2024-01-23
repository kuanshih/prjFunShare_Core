using prjFunShare_Core.Models;

namespace prjFunShare_Core.ViewModels
{
    public class CProductSearchViewModel
    {
        public List<Categories> Categories { get; set; }
        public List<Region> Regions { get; set; }
        public List<City> Citys { get; set; }
        public List<Age> Ages { get; set; }
        public decimal PriceMax { get; set; }
        public DateTime OldestDate { get; set; }
        public DateTime LatestDate { get; set; }
    }
}
