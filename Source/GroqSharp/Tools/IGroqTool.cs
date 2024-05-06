namespace GroqSharp.Tools
{
    public interface IGroqTool
    {
        string Name { get; }

        string Description { get; }

        IDictionary<string, IGroqToolParameter> Parameters { get; }

        Task<string> ExecuteAsync(IDictionary<string, object> parameters);
    }
}