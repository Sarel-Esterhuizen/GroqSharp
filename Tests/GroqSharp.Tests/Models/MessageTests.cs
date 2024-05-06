using GroqSharp.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
                Role = MessageRoleType.System,
                Content = "System initialization complete."
            };

            // Act
            var json = message.ToJson();

            // Assert
            Assert.Contains("\"role\":\"system\"", json);
        }

        [Fact]
        public void ToJson_SerializesContentCorrectly()
        {
            // Arrange
            var message = new Message
            {
                Role = MessageRoleType.User,
                Content = "Hello, World!"
            };

            // Act
            var json = message.ToJson();

            // Assert
            Assert.Contains("\"content\":\"Hello, World!\"", json);
        }
    }
}
