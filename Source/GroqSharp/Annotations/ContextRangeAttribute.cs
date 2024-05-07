namespace GroqSharp.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContextRangeAttribute : Attribute
    {
        public double Min { get; }

        public double Max { get; }

        public ContextRangeAttribute(double min = 0, double max = Double.MaxValue)
        {
            Min = min;
            Max = max;
        }
    }
}
