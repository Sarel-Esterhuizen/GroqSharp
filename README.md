# GroqSharp

[![NuGet Badge](https://buildstats.info/nuget/GroqSharp)](https://www.nuget.org/packages/GroqSharp)
[![Build and Test](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/build-and-test.yml)
[![Publish to NuGet](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/publish-to-nuget.yml/badge.svg)](https://github.com/Sarel-Esterhuizen/GroqSharp/actions/workflows/publish-to-nuget.yml)

GroqSharp is a community maintained C# client library for interacting with [GroqCloud](https://groq.com/). Designed to provide a simple and flexible interface, it enables seamless integration of Groq services into your C# applications.

## Why GroqSharp?

GroqSharp is designed to simplify your interactions with GroqCloud by offering:

- **Fluent API**: Configure client options and parameters fluently for convenient setup.
- **Structured Responses**: Utilize specific JSON response structures for predictable outputs.
- **Function Integration**: Integrate and manage function calling within your applications.
- **Retry Mechanism**: Implement configurable retry policies to handle transient errors when working with structured data effectively.
- **Streaming Support**: Support for streaming responses to process data as it becomes available, providing real-time interaction capabilities.

GroqSharp's function/tool integration support allows developers to extend the functionality of the Groq platform within their applications, making it easier to implement complex workflows and data manipulations directly within chat interfaces.


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

Explore sample projects that demonstrate various capabilities of GroqSharp:

- [**Chat Basics**](./Samples/GroqSharp.Samples.ChatBasics/README.md) - Basic chat functionalities.
- [**Chat Streaming**](./Samples/GroqSharp.Samples.ChatStreaming/README.md) - Streaming responses for dynamic interaction.
- [**Structured Data**](./Samples/GroqSharp.Samples.StructuredData/README.md) - Handling structured JSON responses.
- [**Structured Poco**](./Samples/GroqSharp.Samples.StructuredPoco/README.md) - Map structured JSON responses to POCOs.
- [**Tool Integration**](./Samples/GroqSharp.Samples.ToolIntegration/README.md) - Using custom tools for enhanced functionalities.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This library is licensed under the MIT License. See the LICENSE file for more details.

## Conclusion

GroqSharp offers a flexible and efficient way to incorporate Groq's capabilities into your C# projects, ensuring secure and configurable interaction management.