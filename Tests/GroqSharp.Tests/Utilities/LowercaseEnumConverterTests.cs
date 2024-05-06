using GroqSharp.Utilities;
using System.Text;
using System.Text.Json;

namespace GroqSharp.Tests.Utilities
{
    public enum SampleEnum
    {
        FirstValue,
        SecondValue,
        ThirdValue
    }

    public class LowercaseEnumConverterTests
    {
        [Fact]
        public void Write_ConvertsEnumToLowercase()
        {
            // Arrange
            var converter = new LowercaseEnumConverter<SampleEnum>();
            var options = new JsonSerializerOptions();
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            converter.Write(writer, SampleEnum.FirstValue, options);
            writer.Flush();

            // Assert
            string json = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal("\"firstvalue\"", json);
        }

        [Fact]
        public void Read_ConvertsLowercaseStringToEnum()
        {
            // Arrange
            var converter = new LowercaseEnumConverter<SampleEnum>();
            var json = "\"secondvalue\"";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();
            var options = new JsonSerializerOptions();

            // Act
            SampleEnum result = converter.Read(ref reader, typeof(SampleEnum), options);

            // Assert
            Assert.Equal(SampleEnum.SecondValue, result);
        }

        [Fact]
        public void Read_ThrowsExceptionForInvalidValue()
        {
            // Arrange
            var converter = new LowercaseEnumConverter<SampleEnum>();
            var json = "\"invalidvalue\"";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();
            var options = new JsonSerializerOptions();

            // Act
            Exception caughtException = null;
            try
            {
                converter.Read(ref reader, typeof(SampleEnum), options);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
        }

    }
}
