using GroqSharp.Tools;
using System.Text.Json;

namespace GroqSharp
{
    internal class GroqClientResponse
    {
        #region Instance Properties

        public List<string> Contents { get; internal set; } = new List<string>();

        public List<GroqToolCall> ToolCalls { get; internal set; } = new List<GroqToolCall>();

        #endregion

        #region Class Methods

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
                        if (choice.TryGetProperty("message", out var message))
                        {
                            // Handle normal content response.
                            if (message.TryGetProperty("content", out var content) && content.GetString() != null)
                            {
                                response.Contents.Add(content.GetString());
                            }

                            // Handle tool calls nested within message
                            if (message.TryGetProperty("tool_calls", out var toolCalls))
                            {
                                // Need to set the full contents message as assistant content.
                                string serializedMessage = message.GetRawText();
                                response.Contents.Add(serializedMessage); 

                                foreach (JsonElement toolCall in toolCalls.EnumerateArray())
                                {
                                    GroqToolCall toolCallInstance = ParseToolCall(toolCall);
                                    if (toolCallInstance != null)
                                    {
                                        response.ToolCalls.Add(toolCallInstance);
                                    }
                                }
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


        private static GroqToolCall ParseToolCall(
            JsonElement toolCall)
        {
            try
            {
                if (toolCall.TryGetProperty("id", out var id)&&
                    toolCall.TryGetProperty("type", out var type) &&
                    type.GetString() == "function" &&
                    toolCall.TryGetProperty("function", out var function))
                {
                    if (function.TryGetProperty("name", out var toolName) &&
                        function.TryGetProperty("arguments", out var arguments))
                    {
                        var argsString = arguments.GetString();
                        if (argsString != null)
                        {
                            var argsElement = JsonDocument.Parse(argsString).RootElement;
                            var parameters = argsElement.EnumerateObject().ToDictionary(
                                item => item.Name,
                                item => ConvertJsonElement(item.Value)
                            );

                            return new GroqToolCall
                            {
                                Id = id.GetString(),
                                ToolName = toolName.GetString(),
                                Parameters = parameters
                            };
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                throw new ArgumentException($"Failed to parse tool call JSON: {e.Message}", e);
            }

            return null;
        }

        private static object ConvertJsonElement(
            JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt32(out int i) ? (object)i : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
                JsonValueKind.Object => element.EnumerateObject().ToDictionary(prop => prop.Name, prop => ConvertJsonElement(prop.Value)),
                JsonValueKind.Undefined => throw new NotImplementedException(),
                JsonValueKind.Null => throw new NotImplementedException(),
                _ => element.GetRawText() 
            };
        }

        public static GroqClientResponse TryCreateFromJson(string json)
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

        #endregion
    }
}
