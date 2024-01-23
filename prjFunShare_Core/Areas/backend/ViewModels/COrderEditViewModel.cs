using prjFunShare_Core.Areas.backend.Models.ManagerOrder;
using prjFunShare_Core.Models;

namespace prjFunShare_Core.Areas.backend.ViewModels
{
    public class COrderEditViewModel
    {
        public List<CustomerInfomation> CustomerInfomation { get; set; } = new List<CustomerInfomation>();
        public List<Status> Statuspay { get; set; }
        public List<Status> Statusod { get; set; }
        public COrder COrder { get; set; }
    }
}
