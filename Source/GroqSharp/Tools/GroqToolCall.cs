namespace GroqSharp.Tools
{
    public class GroqToolCall : 
        IGroqToolCall
    {
        public string Id { get; set; }

        public string ToolName { get; set; }

        public Dictionary<string, object> Parameters { get; set; }
    }
}