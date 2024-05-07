using GroqSharp.Annotations;
using System.Reflection;

namespace GroqSharp.Utilities
{
    public static class JsonStructureUtility
    {
        public static string CreateJsonStructureFromType(
            Type type,
            Dictionary<Type, string> typeCache = null)
        {
            if (typeCache != null && typeCache.TryGetValue(type, out string cachedResult))
            {
                return cachedResult;
            }

            var jsonElements = type.GetProperties()
                .Select(p => $"\"{p.Name}\": {JsonTypeRepresentation(p)}")
                .ToArray();

            string result = $"{{{string.Join(", ", jsonElements)}}}";

            if (typeCache != null)
                typeCache[type] = result;

            return result;
        }

        internal static string JsonTypeRepresentation(
            PropertyInfo property)
        {
            Type type = property.PropertyType;
            string baseRepresentation = JsonTypeRepresentation(type);

            var descriptionAttr = property.GetCustomAttribute<ContextDescriptionAttribute>();
            var lengthAttr = property.GetCustomAttribute<ContextLengthAttribute>();
            var rangeAttr = property.GetCustomAttribute<ContextRangeAttribute>();

            if (type == typeof(string))
            {
                string details = "\"string";
                if (descriptionAttr != null)
                {
                    details += $" | {descriptionAttr?.Description ?? ""}";
                }

                if (lengthAttr != null)
                {
                    details += $" | Length: {lengthAttr?.Length ?? 0} characters";
                }

                details += "\"";

                return details;
            }

            if (type == typeof(int) ||
                type == typeof(double) ||
                type == typeof(decimal))
            {
                if (descriptionAttr != null || rangeAttr != null)
                {
                    string details = "\"number";
                    if (descriptionAttr != null)
                    {
                        details += $" | {descriptionAttr?.Description ?? ""}";
                    }

                    if (rangeAttr != null)
                    {
                        details += $" | Min: {rangeAttr?.Min ?? 0} : Max: {rangeAttr?.Max ?? Double.MaxValue}";
                    }
                    details += "\"";
                    return details;
                }
            }

            return baseRepresentation;
        }

        private static string JsonTypeRepresentation(
            Type type)
        {
            if (type == typeof(string))
                return "\"string\"";
            if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double))
                return "0";
            if (type == typeof(bool))
                return "false";
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                Type itemType = type.GetGenericArguments()[0];
                return $"[{JsonTypeRepresentation(itemType)}]";
            }
            if (type.IsClass)
                return CreateJsonStructureFromType(type);

            return "\"unknown\"";
        }
    }
}
