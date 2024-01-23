
namespace prjFunShare_Core.ViewModels.Email
{
    public class COrderEmailViewModel
    {
        public string MemberName { get; set; }
        public string Email { get; set; }
        //前往會員中心的訂單連結
        public string ButtonLink { get; set; }
        public string ProductName { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Address { get; set; }
        public List<COrderEmailDetails> OrderDetails { get; set; } 
    }
}
