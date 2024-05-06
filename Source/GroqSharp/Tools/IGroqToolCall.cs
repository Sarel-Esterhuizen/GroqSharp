
namespace GroqSharp.Tools
{
    public interface IGroqToolCall
    {
        string Id { get; set; }

        Dictionary<string, object> Parameters { get; set; }

        string ToolName { get; set; }
    }
}