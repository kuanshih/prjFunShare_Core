namespace prjFunShare_backend.Models.ManagerOpenAI
{
    public class ChatResponseParameter
    {
        public string id { get; set; }
        public string Object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public ChatResponseChoiceParameter[] choices { get; set; }
        public ChatResponseUsageParameter usage { get; set; }

    }
    public class ChatResponseChoiceParameter
    {
        public int index { get; set; }
        public ChatResponseChoiceMessageParameter message { get; set; }
        public string finish_reason { get; set; }
    }
    public class ChatResponseChoiceMessageParameter
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ChatResponseUsageParameter
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}
