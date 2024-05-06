using GroqSharp;
using GroqSharp.Models;
using GroqSharp.Samples.ToolIntegration;

string apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
var apiModel = "llama3-70b-8192";

IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .RegisterTools(new CurrencyConversionTool()); // Custom tool for currency conversion

var messages = new List<Message>
            {
                new Message { Role = MessageRoleType.System, Content = "You can ask about currency conversion rates." },
                new Message { Role = MessageRoleType.User, Content = "How much is 100 USD in EUR?" }
            };

var response = await groqClient.CreateChatCompletionWithToolsAsync(messages);

Console.WriteLine(response);
Console.ReadLine();