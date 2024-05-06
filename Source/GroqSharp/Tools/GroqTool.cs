using System.Text.Json;

namespace GroqSharp.Tools
{
    public abstract class GroqTool : 
        IGroqTool
    {
        #region Instance Properties

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract IDictionary<string, IGroqToolParameter> Parameters { get; }

        #endregion

        #region Instance Methods

        public string GetParameterSchemaJson()
        {
            var schemaObject = new
            {
                type = "object",
                properties = Parameters.ToDictionary(
                    param => param.Key,
                    param => param.Value.ToJsonSerializableObject()
                ),
                required = Parameters.Where(p => p.Value.Required).Select(p => p.Key).ToList()
            };

            return JsonSerializer.Serialize(schemaObject, new JsonSerializerOptions { WriteIndented = false });
        }

        public abstract Task<string> ExecuteAsync(
            IDictionary<string, object> parameters);

        #endregion
    }
}
