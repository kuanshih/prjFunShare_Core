using prjFunShare_Core.Models;

namespace prjFunShare_Core.ViewModels
{
    public class CChatHistory
    {
        public int ChatMessengerId { get; set; }

        public string ChatRoomId { get; set; }

        public int CustomerId { get; set; }

        public int SupplierId { get; set; }

        public string MessageContent { get; set; }

        public DateTime? MessageCreateTime { get; set; }
        public string? customerName { get; set; }
        public string? supplierName { get;set; }
        public byte[] customerPhoto { get; set; }

        public byte[] supplierPhoto { get; set; }

        public bool? WhoSend { get; set; }

    }
}
