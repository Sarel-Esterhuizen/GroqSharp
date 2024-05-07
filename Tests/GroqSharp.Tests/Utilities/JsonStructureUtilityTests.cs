using GroqSharp.Annotations;
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
            Assert.Equal("{\"Name\": \"string | Name of the person | Length: 50 characters\", \"Age\": 0}", result);
        }

        [Fact]
        public void CreateJsonStructureFromType_NestedType_ReturnsCorrectStructure()
        {
            // Arrange
            var type = typeof(Nested);

            // Act
            var result = JsonStructureUtility.CreateJsonStructureFromType(type);

            // Assert
            Assert.Equal("{\"Simple\": {\"Name\": \"string | Name of the person | Length: 50 characters\", \"Age\": 0}}", result);
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
            Assert.Equal("{\"Name\": \"string | Name of the person | Length: 50 characters\", \"Age\": 0}", result1);
            Assert.True(cache.ContainsKey(type));
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void JsonTypeRepresentation_StringPropertyWithAttributes_ReturnsDetailedRepresentation()
        {
            // Arrange
            var propertyInfo = typeof(Simple).GetProperty(nameof(Simple.Name));

            // Act
            var result = JsonStructureUtility.JsonTypeRepresentation(propertyInfo);

            // Assert
            Assert.Equal("\"string | Name of the person | Length: 50 characters\"", result);
        }

        [Fact]
        public void JsonTypeRepresentation_ListType_ReturnsCorrectStructure()
        {
            // Arrange
            var propertyInfo = typeof(ClassWithList).GetProperty(nameof(ClassWithList.IntList));

            // Act
            var result = JsonStructureUtility.JsonTypeRepresentation(propertyInfo);

            // Assert
            Assert.Equal("[0]", result);
        }

        [Fact]
        public void JsonTypeRepresentation_UnknownType_ReturnsUnknown()
        {
            // Arrange
            var propertyInfo = typeof(ClassWithDateTime).GetProperty(nameof(ClassWithDateTime.DateTime));

            // Act
            var result = JsonStructureUtility.JsonTypeRepresentation(propertyInfo);

            // Assert
            Assert.Equal("\"unknown\"", result);
        }

        public class Simple
        {
            [ContextDescription("Name of the person")]
            [ContextLength(50)]
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class Nested
        {
            public Simple Simple { get; set; }
        }

        public class ClassWithList
        {
            public List<int> IntList { get; set; }
        }

        public class ClassWithDateTime
        {
            public DateTime DateTime { get; set; }
        }
    }
}
