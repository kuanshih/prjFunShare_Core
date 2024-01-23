using System.Net.Mail;

namespace prjFunShare_Core.Models
{
    public class CPaymentSuccessful
    {
        //付款成功信需要的參數

        public string Email { get; set; }
        // mailContent換成AlternateView模板
        public AlternateView alternateView { get; set; }
        public string mailSubject { get; set; }
        //public string mailBody { get; set; }
        
    }
}
