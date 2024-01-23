using System.ComponentModel.DataAnnotations;

namespace prjFunShare_Core.Models
{
    public class CaptchaResult
    {
        [Required]
        [StringLength(4)]
        public string CaptchaCode { get; set; }

        public byte[] CaptchaByteData { get; set; }
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
    }
}
