# GroqSharp

[![NuGet Badge](https://buildstats.info/nuget/GroqSharp)](https://www.nuget.org/packages/GroqSharp)
[![Build and Test](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/build-and-test.yml)
[![Publish to NuGet](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/publish-to-nuget.yml/badge.svg)](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/publish-to-nuget.yml)

GroqSharp is a community maintained C# client library for interacting with [GroqCloud](https://groq.com/). Designed to provide a simple and flexible interface, it enables seamless integration of Groq services into your C# applications.

## Why GroqSharp?

GroqSharp is designed to simplify your interactions with GroqCloud by offering:

- **Fluent API**: Configure client options and parameters fluently for convenient setup.
- **Structured Responses**: Utilize specific JSON response structures for predictable outputs.
- **Retry Mechanism**: Implement configurable retry policies to handle transient errors effectively.
- **Streaming Support**: Support for streaming responses to process data as it becomes available.

## Getting Started

### Installation

Install GroqSharp via NuGet:

```bash
dotnet add package GroqSharp
```

### Environment Setup

For security and flexibility, it is recommended to store your API key in an environment variable. This prevents hardcoding sensitive information in your source code:

```csharp
var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
var apiModel = "llama3-70b-8192"; // Or other supported model

var groqClient = new GroqClient(apiKey, apiModel);
```

### Fluent Configuration

Configure your client using a fluent API:

```csharp
IGroqClient groqClient = new GroqClient(apiKey, apiModel)
    .SetTemperature(0.5)
    .SetMaxTokens(512)
    .SetTopP(1)
    .SetStop("NONE")
    .SetStructuredRetryPolicy(5); // Retry up to 5 times on failure
```

### Examples

Here are a few examples of how to use GroqSharp for different types of interactions:

#### 1. Synchronous Chat Completion (Non-JSON)

Example demonstrating a synchronous chat completion:

```csharp
var response = await groqClient.CreateChatCompletionAsync(
    new Message { Role = MessageRole.System, Content = "You are a helpful, smart, kind, and efficient AI assistant." },
    new Message { Role = MessageRole.Assistant, Content = "This is your first time using Groq, after experiencing other commercial models." },
    new Message { Role = MessageRole.User, Content = "Explain the importance of fast language models." }
);
```

#### 2. Synchronous Chat Completion (JSON)

Handle JSON structured responses and retries:

```csharp
try
{
    var jsonStructure = @"
    {
        ""name"": ""string"",
        ""powers"": {
            ""rank"": ""string"",
            ""abilities"": ""string""
        }
    }
    ";

    var response = await groqClient.GetStructuredChatCompletionAsync(jsonStructure,
        new Message { Role = MessageRole.System, Content = "You are a helpful, smart, kind, and efficient AI assistant." },
        new Message { Role = MessageRole.User, Content = "List a few Pokémon characters." }
    );

    Console.WriteLine(response);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
```

#### 3. Streaming Chat Completion

Stream responses for interactive sessions:

```csharp
try
{
    var messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Provide some song lyrics." },
        new Message { Role = MessageRole.System, Content = "You are the muse for many songwriters." }
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

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This library is licensed under the MIT License. See the LICENSE file for more details.

## Conclusion

GroqSharp offers a flexible and efficient way to incorporate Groq's capabilities into your C# projects, ensuring secure and configurable interaction management.