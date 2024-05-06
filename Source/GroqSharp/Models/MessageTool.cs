using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroqSharp.Models
{
    public class MessageTool : 
        Message
    {
        #region Instance Properties

        [JsonPropertyName("tool_call_id")]
        public string ToolCallId { get; set; }

        #endregion

        #region Instance Methods

        public override string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Serialize(this, options);
        }

        #endregion
    }
}