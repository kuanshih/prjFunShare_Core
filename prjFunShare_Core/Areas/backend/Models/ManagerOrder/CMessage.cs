using System.ComponentModel;

namespace prjFunShare_Core.Areas.backend.Models.ManagerOrder
{
    public class CMessage
    {
        [DisplayName("訂單編號")]
        public int FOrder_ID { get; set; }

        [DisplayName("訊息時間")]
        public DateTime FMessage_Time { get; set; }

        [DisplayName("訊息ID")]
        public int FMessage_ID { get; set; }

        [DisplayName("內容")]
        public string FMessage_Text { get; set; }

        public int FCustomerService_ID = 1;
    }
}
