using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroqSharp.Utilities
{
    internal class LowercaseEnumConverter<T> : 
        JsonConverter<T> where T : Enum
    {
        public override T Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            string value = reader.GetString();
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public override void Write(
            Utf8JsonWriter writer, 
            T value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }
}
