using GroqSharp.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroqSharp
{
    internal record GroqClientRequest
    {
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

                // Regretting not using Newtonsoft like a normal person right now.
                // Perhaps there is a wiser person out there that can get a less verbose version working.

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

                if (Messages != null)
                {
                    writer.WritePropertyName("messages");
                    JsonSerializer.Serialize(writer, Messages, options);
                }

                if (JsonResponse)
                {
                    writer.WritePropertyName("response_format");
                    writer.WriteStartObject();
                    writer.WriteString("type", "json_object");
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.Flush();
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
