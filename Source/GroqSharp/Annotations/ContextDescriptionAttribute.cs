namespace GroqSharp.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContextDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ContextDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
