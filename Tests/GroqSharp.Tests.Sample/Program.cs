using GroqSharp;
using GroqSharp.Models;

// Set your API Key and chosen model.
var apiKey = "<your-api-key>";
var apiModel = "llama3-70b-8192";

// Define the client and optional parameters fluently.
IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .SetTemperature(0.5)
    .SetMaxTokens(512)
    .SetTopP(1)
    .SetStop("NONE")
    .SetDefaultSystemMessage(new Message { Role = MessageRole.System, Content = "You are a drunk assistant with a love for fluffy carpets." }); // Default system message

// Synchronous Example (Non JSON).
var response = await groqClient.CreateChatCompletionAsync(
    new Message { Role = MessageRole.System, Content = "You are a helpful, smart, kind, and efficient AI assistant. You always fulfill the user's requests to the best of your ability." },
    new Message { Role = MessageRole.Assistant, Content = "The user only knows the pain from using commercial models like ChatGPT, and its the first time they are using Groq." },
    new Message { Role = MessageRole.User, Content = "Explain the importance of fast language models" });

Console.WriteLine(response);
Console.WriteLine();
Console.WriteLine("[Enter to continue]");
Console.ReadLine();


// Synchronous Example (JSON).
var jsonStructure = @"
{
    ""name"": ""string"",
    ""powers"": {
        ""rank"": ""string"",
        ""abilities"": ""string""
    }
}
";

try
{
     response = await groqClient.GetStructuredChatCompletionAsync(jsonStructure,
        new Message { Role = MessageRole.System, Content = "You are a helpful, smart, kind, and efficient AI assistant." },
        new Message { Role = MessageRole.User, Content = "Give me a few Pokemon characters." }
    );

    Console.WriteLine(response);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

Console.WriteLine();
Console.WriteLine("[Enter to continue]");
Console.ReadLine();

// Streaming Example.
try
{
    var messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Give some lyrics to a song." },
        new Message { Role = MessageRole.System, Content = "You are the love child of Britney Spears and Eminem." }
    };

    await foreach (var streamingResponse in groqClient.CreateChatCompletionStreamAsync(messages))
    {
        Console.Write(streamingResponse);
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

Console.ReadLine();
