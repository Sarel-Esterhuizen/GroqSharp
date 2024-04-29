# GroqSharp

GroqSharp is a C# client library that makes it easy to interact with  [GroqCloud](https://groq.com/). It's designed to provide a simple and flexible interface, allowing you to seamlessly integrate the Groq service into your C# applications.

## Why GroqSharp?

GroqSharp aims to simplify and streamline your interactions with Groq, offering:

- **Fluent API**: Set up client options and parameters fluently for convenient configuration.
- **Structured Responses**: Define specific JSON response structures for more predictable outputs.
- **Retry Mechanism**: Configurable retry policies ensure robust interactions, even in the face of transient errors.
- **Streaming Support**: Allows for streaming responses, processing data as it becomes available.

## Getting Started

### Installation

To install GroqSharp via NuGet:

```bash
dotnet add package GroqSharp
```

### API Key and Model

You'll need an API key and model to connect to the Groq service. Once acquired, they can be integrated as follows:

```csharp
var apiKey = "<your-api-key>";
var apiModel = "llama3-70b-8192"; // Or other supported

var groqClient = new GroqClient(apiKey, apiModel);
```

### Fluent Configuration

You can configure your client fluently by chaining various setup options:

```csharp
IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .SetTemperature(0.5)
    .SetMaxTokens(512)
    .SetTopP(1)
    .SetStop("NONE")
    .SetStructuredRetryPolicy(5) // Retry up to 5 times on failure
    .SetDefaultSystemMessage(new Message { Role = MessageRole.System, Content = "You are a drunk assistant with a love for fluffy carpets." }); // Default system message
```

### Examples

#### 1. Synchronous Chat Completion (Non-JSON)

```csharp
var response = await groqClient.CreateChatCompletionAsync(
    new Message { Role = MessageRole.System, Content = "You are a helpful, smart, kind, and efficient AI assistant. You always fulfill the user's requests to the best of your ability." },
    new Message { Role = MessageRole.Assistant, Content = "The user only knows the pain from using commercial models like ChatGPT, and it's the first time they're using Groq." },
    new Message { Role = MessageRole.User, Content = "Explain the importance of fast language models." }
);
```

#### 2. Synchronous Chat Completion (JSON)

You can customize the default retry of 3 using the fluent `SetRetryPolicy`. Since models can inherently not return JSON, be sure to handle error conditions. Be aware that extra costs can be incurred with excessive retries. The client will automatically add JSON instructions to the system message, but feel free to add more.

```csharp
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
        new Message { Role = MessageRole.User, Content = "Give me a few Pokémon characters." }
    );

    Console.WriteLine(response);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
```

#### 3. Streaming Chat Completion

```csharp
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
```

### Conclusion

GroqSharp offers a flexible and streamlined way to work with the Groq service in your C# projects. You can easily configure its settings, handle structured responses, and manage different interaction types, making it an invaluable tool for any C# developer looking to integrate Groq's capabilities.