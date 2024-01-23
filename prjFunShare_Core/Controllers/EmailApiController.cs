using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using prjFunShare_Core.Models;
using System.Net.Mime;
using QRCoder;

namespace prjFunShare_Core.Controllers
{
    public class EmailApiController : Controller
    {
        private readonly FUNShareContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpContext _httpContext;


        public EmailApiController(IConfiguration configuration, HttpContext httpContext, FUNShareContext context)
        {
            _context = context;
            _configuration = configuration;
            _httpContext = httpContext;
        }

        #region 付款成功寄信
        //todo 附件要有訂單細節和QRCode圖片
        public void SendPaymentSuccessfulMail(CPaymentSuccessful data)
        {
            //檢查資料庫是否有這個帳號
           var mailCheck = _context.CustomerInfomation.Where(m => m.Email.Equals(data.Email)).FirstOrDefault();
            if (mailCheck == null)
            {
                return;
            }
            string UserEmail = mailCheck.Email;

            // 信件內容範本
            //string mailContent = data.mailContent;
            //mailContent = mailContent;

            // 信件主題
            string mailSubject = data.mailSubject;

            // Google 發信帳號密碼
            string GoogleMailUserID = _configuration["SendMailSettings:GoogleMailUserID"];
            string GoogleMailUserPwd = _configuration["SendMailSettings:GoogleMailUserPwd"];

            // 使用 Google Mail Server 發信
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            MailMessage mms = new MailMessage();
            mms.From = new MailAddress(GoogleMailUserID);
            mms.Subject = mailSubject;
            // 加入信件HTML
            mms.AlternateViews.Add(data.alternateView);
            //mms.Attachments.Add(new Attachment(data.QRCode, "image/jpeg"));
            //mms.Attachments[0].ContentId = System.Guid.NewGuid().ToString();
            //mms.Attachments[0].ContentDisposition.Inline = true;
            //mms.Attachments[0].NameEncoding = Encoding.UTF8;

            //Attachment atta = new Attachment("./wwwroot/img/預設頭像.jpg");
            //Attachment atta = new Attachment(data.QRCode, "image/jpeg");
            //mms.Attachments.Add(atta);
            //ContentDisposition disposition = atta.ContentDisposition;
            //disposition.Inline = true;

            //mms.Body = mailContent;
            mms.IsBodyHtml = true;


            mms.SubjectEncoding = Encoding.UTF8;
            mms.To.Add(new MailAddress(UserEmail));
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
        }
        #endregion


        #region 訂單成立寄信
        public void SendOrderEstablishMail(COrderEstablish data)
        {
            // 檢查資料庫是否有這個帳號
            var mailCheck = _context.CustomerInfomation.Where(m => m.Email.Equals(data.Email)).FirstOrDefault();
            if (mailCheck == null)
            {
                return;
            }

            string UserEmail = mailCheck.Email;

            // 信件內容範本
            string mailContent = data.mailContent;
            mailContent = mailContent;

            // 信件主題
            string mailSubject = data.mailSubject;

            // Google 發信帳號密碼
            string GoogleMailUserID = _configuration["SendMailSettings:GoogleMailUserID"];
            string GoogleMailUserPwd = _configuration["SendMailSettings:GoogleMailUserPwd"];

            // 使用 Google Mail Server 發信
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            MailMessage mms = new MailMessage();
            mms.From = new MailAddress(GoogleMailUserID);
            mms.Subject = mailSubject;
            mms.Body = mailContent;
            mms.IsBodyHtml = true;
            mms.SubjectEncoding = Encoding.UTF8;
            mms.To.Add(new MailAddress(UserEmail));
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
        }
        #endregion

        #region Email驗證信
        public void SendVerifyMail(CEmailVerify data)
        {
            // 檢查資料庫是否有這個帳號
            var mailCheck = _context.CustomerInfomation.Where(m => m.Email.Equals(data.Email)).FirstOrDefault();
            if (mailCheck == null)
            {
                return;
            }

            string UserEmail = mailCheck.Email;


            // 取得系統自定密鑰，在 Web.config 設定
            string SecretKey = _configuration["SendMailSettings:SecretKey"];

            // 產生帳號+時間驗證碼
            string sVerify = data.Email + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // 將驗證碼使用 3DES 加密
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] buf = Encoding.UTF8.GetBytes(SecretKey);
            byte[] result = md5.ComputeHash(buf);
            string md5Key = BitConverter.ToString(result).Replace("-", "").ToLower().Substring(0, 24);
            DES.Key = UTF8Encoding.UTF8.GetBytes(md5Key);
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = UTF8Encoding.UTF8.GetBytes(sVerify);
            sVerify = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length)); // 3DES 加密後驗證碼

            // 將加密後密碼使用網址編碼處理
            sVerify = HttpUtility.UrlEncode(sVerify);

            // 網站網址
            string webPath = data.scheme + "://" + data.host + "/";

            // 從信件連結回到重設密碼頁面
            string receivePage = data.receivePage;
            string linkText = data.linkText;

            // 信件內容範本
            string mailContent = data.mailContent;
            mailContent = mailContent + "<a href='" + webPath + receivePage + "?verify=" + sVerify + "'  target='_blank'>" + linkText + "</a>";

            // 信件主題
            string mailSubject = data.mailSubject;

            // Google 發信帳號密碼
            string GoogleMailUserID = _configuration["SendMailSettings:GoogleMailUserID"];
            string GoogleMailUserPwd = _configuration["SendMailSettings:GoogleMailUserPwd"];

            // 使用 Google Mail Server 發信
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            MailMessage mms = new MailMessage();
            mms.From = new MailAddress(GoogleMailUserID);
            mms.Subject = mailSubject;
            mms.Body = mailContent;
            mms.IsBodyHtml = true;
            mms.SubjectEncoding = Encoding.UTF8;
            mms.To.Add(new MailAddress(UserEmail));
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);//寄信帳密 
                client.Send(mms); //寄出信件
            }
        }

        public bool VerifyCode(string verify)
        {

            // 由信件連結回來會帶參數 verify

            if (verify == null)
            {
                TempData["ErrorMsg"] = "未傳入驗證碼";
                return false;
            }

            // 取得系統自定密鑰，在 Web.config 設定
            string SecretKey = _configuration["SendMailSettings:SecretKey"];

            try
            {
                // 使用 3DES 解密驗證碼
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] buf = Encoding.UTF8.GetBytes(SecretKey);
                byte[] md5result = md5.ComputeHash(buf);
                string md5Key = BitConverter.ToString(md5result).Replace("-", "").ToLower().Substring(0, 24);
                DES.Key = UTF8Encoding.UTF8.GetBytes(md5Key);
                DES.Mode = CipherMode.ECB;
                DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(verify);
                string deCode = UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));

                verify = deCode; //解密後還原資料
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "驗證碼錯誤";
                return false;
            }

            // 取出帳號
            string UserID = verify.Split('|')[0];

            // 取得重設時間
            string ResetTime = verify.Split('|')[1];

            // 檢查時間是否超過 30 分鐘
            DateTime dResetTime = Convert.ToDateTime(ResetTime);
            TimeSpan TS = new System.TimeSpan(DateTime.Now.Ticks - dResetTime.Ticks);
            double diff = Convert.ToDouble(TS.TotalMinutes);
            if (diff > 30)
            {
                TempData["ErrorMsg"] = "驗證碼已過期，請再試一次";
                return false;
            }

            // 驗證碼檢查成功，加入 Session
            _httpContext.Session.SetString(CDictionary.SK_RESET_PWD_EMAIL, UserID);
            return true;
        }
        #endregion
    }

}
