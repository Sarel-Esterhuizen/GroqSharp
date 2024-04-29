using GroqSharp.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroqSharp.Models
{
    public class Message
    {
        [JsonConverter(typeof(LowercaseEnumConverter<MessageRole>))]
        public MessageRole Role { get; set; }  

        public required string Content { get; set; }

        public string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
