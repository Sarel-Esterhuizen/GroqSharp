using GroqSharp;
using GroqSharp.Models;

var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
var apiModel = "llama3-70b-8192";

IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .SetTemperature(0.5)
    .SetMaxTokens(512)
    .SetTopP(1)
    .SetStop("NONE");

var jsonStructure = @"
        {
            ""name"": ""string"",
            ""powers"": {
                ""rank"": ""string"",
                ""abilities"": ""string""
            }
        }";

try
{
    var response = await groqClient.GetStructuredChatCompletionAsync(jsonStructure,
        new Message { Role = MessageRoleType.System, Content = "You are a helpful, smart, kind, and efficient AI assistant." },
        new Message { Role = MessageRoleType.User, Content = "Give me a few Pokemon characters." }
    );

    Console.WriteLine(response);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

Console.WriteLine("\n[Enter to continue]");
Console.ReadLine();