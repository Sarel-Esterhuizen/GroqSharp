namespace GroqSharp.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContextLengthAttribute : Attribute
    {
        public int Length { get; }

        public ContextLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
