using System.Text.Json;

namespace GroqSharp
{
    internal class GroqClientResponse
    {
        public List<string> Contents { get; internal set; } = new List<string>();

        public static GroqClientResponse FromJson(
            string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (!root.TryGetProperty("choices", out var choices))
                        throw new JsonException("The 'choices' property is missing.");

                    var response = new GroqClientResponse();
                    foreach (JsonElement choice in choices.EnumerateArray())
                    {
                        if (choice.TryGetProperty("message", out var message) &&
                            message.TryGetProperty("content", out var content))
                        {
                            var contentStr = content.GetString();
                            if (contentStr != null)
                            {
                                response.Contents.Add(contentStr);
                            }
                        }
                    }

                    return response;
                }
            }
            catch (JsonException e)
            {
                throw new ArgumentException("Failed to parse JSON", e);
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException("Invalid JSON format", e);
            }
        }

        public static GroqClientResponse TryCreateFromJson(
            string json)
        {
            try
            {
                return FromJson(json);
            }
            catch (Exception)
            {
                return null; 
            }
        }
    }
}
