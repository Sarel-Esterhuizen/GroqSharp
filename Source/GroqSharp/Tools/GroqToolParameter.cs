namespace GroqSharp.Tools
{
    public class GroqToolParameter : 
        IGroqToolParameter
    {
        #region Instance Properties

        public GroqToolParameterType Type { get; private set; }

        public string Description { get; private set; }

        public bool Required { get; private set; } = true;

        public IDictionary<string, IGroqToolParameter> Properties { get; private set; }

        #endregion

        #region Constructors

        public GroqToolParameter(
            GroqToolParameterType type,
            string description,
            bool required = true)
        {
            Type = type;
            Description = description;
            Required = required;
            Properties = type == GroqToolParameterType.Object ? new Dictionary<string, IGroqToolParameter>() : null;
        }

        #endregion

        #region Instance Methods

        public void AddProperty(
            string name,
            IGroqToolParameter parameter)
        {
            if (Type != GroqToolParameterType.Object)
            {
                throw new InvalidOperationException("Only parameters of type 'Object' can have properties.");
            }
            Properties[name] = parameter;
        }

        public object ToJsonSerializableObject()
        {
            var result = new Dictionary<string, object>
            {
                ["type"] = Type.ToString().ToLower(),
                ["description"] = Description
            };

            if (Properties != null && Properties.Any())
            {
                result["properties"] = Properties.ToDictionary(
                    prop => prop.Key,
                    prop => prop.Value.ToJsonSerializableObject()
                );
                result["required"] = Properties.Where(p => p.Value.Required).Select(p => p.Key).ToList();
            }

            return result;
        }

        #endregion
    }
}