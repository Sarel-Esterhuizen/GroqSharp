using GroqSharp.Models;

namespace GroqSharp.Tests
{
    public class GroqClientRequestTests
    {
        [Fact]
        public void ToJson_SerializesAllPropertiesCorrectly()
        {
            // Arrange
            var tools = new
            {
                type = "function",
                function = new
                {
                    name = "example_tool",
                    description = "An example tool",
                    parameters = new { param1 = "value1" }
                }
            };

            var request = new GroqClientRequest
            {
                Model = "llama3-70b-8192",
                Temperature = 0.7,
                Messages = new Message[] { new Message { Content = "Hello", Role = MessageRoleType.User } },
                MaxTokens = 150,
                TopP = 0.9,
                Stop = "end",
                Stream = true,
                Tools = tools,
                ToolChoice = "auto"
            };

            // Act
            var json = request.ToJson();

            // Assert
            Assert.Contains("\"model\":\"llama3-70b-8192\"", json);
            Assert.Contains("\"temperature\":0.7", json);
            Assert.Contains("\"messages\":[{\"role\":\"user\",\"content\":\"Hello\"}]", json);
            Assert.Contains("\"max_tokens\":150", json);
            Assert.Contains("\"top_p\":0.9", json);
            Assert.Contains("\"stop\":\"end\"", json);
            Assert.Contains("\"stream\":true", json);
            Assert.Contains("\"tools\":{\"type\":\"function\"", json);
            Assert.Contains("\"tool_choice\":\"auto\"", json);
        }

        [Fact]
        public void ToJson_OmitsNullProperties()
        {
            // Arrange
            var request = new GroqClientRequest
            {
                Model = "llama3-70b-8192",
                Stream = false
            };

            // Act
            var json = request.ToJson();

            // Assert
            Assert.DoesNotContain("temperature", json);
            Assert.DoesNotContain("max_tokens", json);
            Assert.DoesNotContain("top_p", json);
            Assert.DoesNotContain("stop", json);
        }

        [Fact]
        public void ToJson_IndentsJsonWhenRequested()
        {
            // Arrange
            var request = new GroqClientRequest
            {
                Model = "llama3-70b-8192",
                Stream = true
            };

            // Act
            var json = request.ToJson(indented: true);

            // Assert
            string expectedString = $"{{{Environment.NewLine}  \"model\": \"llama3-70b-8192\"";
            Assert.Contains(expectedString, json);
        }


        [Fact]
        public void ToJson_AddsJsonResponseFormatWhenJsonResponseIsTrue()
        {
            // Arrange
            var request = new GroqClientRequest
            {
                Model = "llama3-70b-8192",
                JsonResponse = true
            };

            // Act
            var json = request.ToJson();

            // Assert
            Assert.Contains("\"response_format\":{\"type\":\"json_object\"}", json);
        }

        [Fact]
        public void ToJson_HandlesNullToolsAndDefaultToolChoiceCorrectly()
        {
            // Arrange
            var request = new GroqClientRequest
            {
                Model = "llama3-70b-8192",
                ToolChoice = "none"  // Explicitly setting default to verify it does not appear if not needed.
            };

            // Act
            var json = request.ToJson();

            // Assert
            Assert.DoesNotContain("\"tools\":", json);
            Assert.DoesNotContain("\"tool_choice\":", json);  // Assuming default 'none' does not need to be serialized.
        }
    }
}