using GroqSharp;
using GroqSharp.Models;

var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
var apiModel = "llama3-70b-8192";

IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .SetTemperature(0.5)
    .SetMaxTokens(256)
    .SetTopP(1)
    .SetStop("NONE");

var response = await groqClient.CreateChatCompletionAsync(
    new Message { Role = MessageRoleType.System, Content = "You are a helpful, smart, kind, and efficient AI assistant. You always fulfill the user's requests to the best of your ability." },
    new Message { Role = MessageRoleType.Assistant, Content = "The user only knows the pain from using commercial models like ChatGPT, and it's the first time they are using Groq." },
    new Message { Role = MessageRoleType.User, Content = "Explain the importance of fast language models" });

Console.WriteLine(response);
Console.WriteLine("\n[Enter to continue]");
Console.ReadLine();