using System.ComponentModel;

namespace prjFunShare_backend.Models.ManagerOrder
{
    public class CMessage
    {
        public List<Message> MessageList { get; set; }
      
        public int OrderIdForView { get; set; }
        public string MessageForView { get; set; }
    }
}
