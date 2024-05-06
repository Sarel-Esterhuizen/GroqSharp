using GroqSharp.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroqSharp
{
    internal record GroqClientRequest
    {
        #region Instance Properties

        [JsonPropertyName("model")]
        public string Model { get; init; }

        [JsonPropertyName("temperature")]
        public double? Temperature { get; init; }

        [JsonPropertyName("messages")]
        public Message[] Messages { get; init; }

        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; init; }

        [JsonPropertyName("top_p")]
        public double? TopP { get; init; }

        [JsonPropertyName("stop")]
        public string? Stop { get; init; }

        [JsonPropertyName("stream")]
        public bool Stream { get; init; }

        [JsonIgnore]
        public bool JsonResponse { get; set; } = false;

        [JsonPropertyName("tools")]
        public object Tools { get; set; }

        [JsonPropertyName("tool_choice")]
        public string ToolChoice { get; set; } = "none";

        #endregion

        #region Instance Methods

        public string ToJson(bool indented = false)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = indented
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indented }))
            {
                writer.WriteStartObject();

                writer.WriteString("model", Model);
                if (Temperature.HasValue)
                    writer.WriteNumber("temperature", Temperature.Value);
                if (MaxTokens.HasValue)
                    writer.WriteNumber("max_tokens", MaxTokens.Value);
                if (TopP.HasValue)
                    writer.WriteNumber("top_p", TopP.Value);
                if (!string.IsNullOrEmpty(Stop))
                    writer.WriteString("stop", Stop);
                writer.WriteBoolean("stream", Stream);

                // Serialize each message according to its actual type
                if (Messages != null)
                {
                    writer.WritePropertyName("messages");
                    writer.WriteStartArray();
                    foreach (var message in Messages)
                    {
                        JsonSerializer.Serialize(writer, message, message.GetType(), options);
                    }
                    writer.WriteEndArray();
                }

                if (JsonResponse)
                {
                    writer.WritePropertyName("response_format");
                    writer.WriteStartObject();
                    writer.WriteString("type", "json_object");
                    writer.WriteEndObject();
                }

                if (Tools != null)
                {
                    writer.WritePropertyName("tools");
                    JsonSerializer.Serialize(writer, Tools, options);
                }

                if (!string.IsNullOrEmpty(ToolChoice) && ToolChoice != "none")
                {
                    writer.WriteString("tool_choice", ToolChoice);
                }

                writer.WriteEndObject();
                writer.Flush();
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        #endregion
    }
}