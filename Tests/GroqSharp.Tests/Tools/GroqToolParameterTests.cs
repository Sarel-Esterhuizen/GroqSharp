using GroqSharp.Tools;
using System.Text.Json;

namespace GroqSharp.Tests.Tools
{
    public class GroqToolParameterTests
    {
        [Fact]
        public void GroqParameter_Initializes_Correctly()
        {
            // Arrange
            var expectedType = GroqToolParameterType.String;
            var expectedDescription = "Test parameter";

            // Act
            var param = new GroqToolParameter(expectedType, expectedDescription);

            // Assert
            Assert.Equal(expectedType, param.Type);
            Assert.Equal(expectedDescription, param.Description);
            Assert.True(param.Required);
            Assert.Null(param.Properties); // Properties should be null for non-object types
        }

        [Fact]
        public void GroqParameter_AddProperty_Throws_ForNonObjectTypes()
        {
            // Arrange
            var param = new GroqToolParameter(GroqToolParameterType.String, "Test parameter");
            var subParam = new GroqToolParameter(GroqToolParameterType.Integer, "Sub parameter");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => param.AddProperty("subParam", subParam));
            Assert.Equal("Only parameters of type 'Object' can have properties.", exception.Message);
        }

        [Fact]
        public void GroqParameter_ToJsonSerializableObject_RepresentsCorrectJson()
        {
            // Arrange
            var objectParam = new GroqToolParameter(GroqToolParameterType.Object, "Object parameter");
            objectParam.AddProperty("nested", new GroqToolParameter(GroqToolParameterType.String, "Nested parameter"));

            // Act
            var jsonObject = objectParam.ToJsonSerializableObject();
            var jsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });

            // Assert
            var expectedType = "\"type\": \"object\"";
            var expectedNestedType = "\"type\": \"string\"";
            Assert.Contains(expectedType, jsonString);
            Assert.Contains("\"nested\": {", jsonString);
            Assert.Contains(expectedNestedType, jsonString);
        }
    }
}
