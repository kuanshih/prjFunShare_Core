using prjFunShare_backend.Models.ManagerOrder;
using prjFunShare_backend.Models;
namespace prjFunShare_backend.ViewModels
{
    public class COrderEditViewModel
    {
        public List<CustomerInfomation> CustomerInfomation { get; set; } = new List<CustomerInfomation>();
        public List<Status> Statuspay { get; set; }
        public List<Status> Statusod { get; set; }
        public COrder COrder { get; set; }
    }
}
