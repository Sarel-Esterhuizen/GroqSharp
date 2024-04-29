using GroqSharp.Models;

namespace GroqSharp.Tests.Models
{
    public class MessageTests
    {
        [Fact]
        public void ToJson_SerializesRoleAsLowercase()
        {
            // Arrange
            var message = new Message
            {
                Role = MessageRole.System,
                Content = "System initialization complete."
            };

            // Act
            var json = message.ToJson();

            // Assert
            Assert.Contains("\"role\":\"system\"", json);
        }
    }
}
