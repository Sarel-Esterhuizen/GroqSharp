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
                .Select(p => $"\"{p.Name}\": {JsonTypeRepresentation(p.PropertyType)}")
                .ToArray();

            string result = $"{{{string.Join(", ", jsonElements)}}}";

            if (typeCache != null)
                typeCache[type] = result;
            return result;
        }

        public static string JsonTypeRepresentation(
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
