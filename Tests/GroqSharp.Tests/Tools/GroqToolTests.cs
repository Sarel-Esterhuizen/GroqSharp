using GroqSharp.Tests.Mocks;

namespace GroqSharp.Tests.Tools
{
    public class GroqToolTests
    {
        [Fact]
        public void GroqTool_GetParameterSchemaJson_GeneratesCorrectSchema()
        {
            // Arrange
            var tool = new MockGroqTool();

            // Act
            string jsonSchema = tool.GetParameterSchemaJson();

            // Assert
            Assert.Contains("\"type\":\"object\"", jsonSchema);
            Assert.Contains("\"properties\"", jsonSchema);
            Assert.Contains("\"testParam\"", jsonSchema);
        }

        [Fact]
        public async Task GroqTool_ExecuteAsync_ReturnsExpectedResult()
        {
            // Arrange
            var tool = new MockGroqTool();
            var parameters = new Dictionary<string, object> { { "testParam", "value" } };

            // Act
            var result = await tool.ExecuteAsync(parameters);

            // Assert
            Assert.Equal("Executed", result);
        }
    }
}