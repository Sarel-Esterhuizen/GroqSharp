using GroqSharp.Utilities;

namespace GroqSharp.Tests.Utilities
{
    public class JsonStructureUtilityTests
    {
        [Fact]
        public void CreateJsonStructureFromType_SimpleType_ReturnsCorrectStructure()
        {
            // Arrange
            var type = typeof(Simple);

            // Act
            var result = JsonStructureUtility.CreateJsonStructureFromType(type);

            // Assert
            Assert.Equal("{\"Name\": \"string\", \"Age\": 0}", result);
        }

        [Fact]
        public void CreateJsonStructureFromType_NestedType_ReturnsCorrectStructure()
        {
            // Arrange
            var type = typeof(Nested);

            // Act
            var result = JsonStructureUtility.CreateJsonStructureFromType(type);

            // Assert
            Assert.Equal("{\"Simple\": {\"Name\": \"string\", \"Age\": 0}}", result);
        }

        [Fact]
        public void CreateJsonStructureFromType_WithTypeCache_CachesResult()
        {
            // Arrange
            var type = typeof(Simple);
            var cache = new Dictionary<Type, string>();

            // Act
            var result1 = JsonStructureUtility.CreateJsonStructureFromType(type, cache);
            var result2 = JsonStructureUtility.CreateJsonStructureFromType(type, cache);

            // Assert
            Assert.Equal("{\"Name\": \"string\", \"Age\": 0}", result1);
            Assert.True(cache.ContainsKey(type));
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void JsonTypeRepresentation_ListType_ReturnsCorrectStructure()
        {
            // Arrange
            var type = typeof(List<int>);

            // Act
            var result = JsonStructureUtility.JsonTypeRepresentation(type);

            // Assert
            Assert.Equal("[0]", result);
        }

        [Fact]
        public void JsonTypeRepresentation_UnknownType_ReturnsUnknown()
        {
            // Arrange
            var type = typeof(DateTime);  // Assuming DateTime isn't specifically handled

            // Act
            var result = JsonStructureUtility.JsonTypeRepresentation(type);

            // Assert
            Assert.Equal("\"unknown\"", result);
        }

        public class Simple
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class Nested
        {
            public Simple Simple { get; set; }
        }
    }
}
