namespace GroqSharp.Tools
{
    public interface IGroqToolParameter
    {
        GroqToolParameterType Type { get; }

        string Description { get; }

        bool Required { get; }

        IDictionary<string, IGroqToolParameter> Properties { get; }

        object ToJsonSerializableObject();
    }
}