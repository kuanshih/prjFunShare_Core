namespace prjFunShare_Core.Models
{
    public class CEmailVerify
    {
        //驗證信需要的參數

        public string Email { get; set; }
        public string mailContent { get; set; }
        public string mailSubject { get; set; }
        public string receivePage { get; set; }
        public string linkText { get; set; }
        public string scheme { get; set; }
        public string host { get; set; }
    }
}
