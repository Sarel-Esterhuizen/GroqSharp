using GroqSharp;
using GroqSharp.Models;

var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
//var apiModel = "mixtral-8x7b-32768";
var apiModel = "llama3-70b-8192";

IGroqClient groqClient = new GroqClient(apiKey, apiModel);

var jsonStructure = @"
        {
            ""name"": ""string | The name of the pokemon"",
            ""description"": ""string | A detailed and long description of the pokemon | Length: 300 characters"",
            ""powers"": {
                ""rank"": ""number | A numeric number between 1 and 10 indicating stregnth"",
                ""abilities"": ""string | A comma seperated list of all the main powers""
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