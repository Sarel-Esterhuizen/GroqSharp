using GroqSharp.Models;

namespace GroqSharp.Tests.Models
{
    public class MessageToolTests
    {
        [Fact]
        public void ToJson_SerializesToolCallIdAsToolCallId()
        {
            // Arrange
            var messageTool = new MessageTool
            {
                Role = MessageRoleType.User,
                Content = "Tool execution started.",
                ToolCallId = "123abc"
            };

            // Act
            var json = messageTool.ToJson();

            // Assert
            Assert.Contains("\"tool_call_id\":\"123abc\"", json);
        }

        [Fact]
        public void ToJson_InheritsBaseToJson()
        {
            // Arrange
            var messageTool = new MessageTool
            {
                Role = MessageRoleType.User,
                Content = "Inherited serialization test.",
                ToolCallId = "inherit123"
            };

            // Act
            var json = messageTool.ToJson();

            // Assert
            Assert.Contains("\"role\":\"user\"", json);
            Assert.Contains("\"content\":\"Inherited serialization test.\"", json);
            Assert.Contains("\"tool_call_id\":\"inherit123\"", json);
        }
    }
}
