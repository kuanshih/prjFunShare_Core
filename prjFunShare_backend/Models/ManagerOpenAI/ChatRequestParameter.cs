namespace prjFunShare_backend.Models.ManagerOpenAI
{
    public class ChatRequestParameter
    {
        public string model { get; set; }
        public List<ChatRequestMessageParameter> messages { get; set; }

    }

    public class ChatRequestMessageParameter
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
