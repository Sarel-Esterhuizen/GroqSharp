# Structured Data Sample

This sample demonstrates the use of structured JSON responses with the GroqSharp library.

## Overview

The Structured Data project shows how to set up the GroqClient to handle structured JSON responses based on a predefined schema. This is particularly useful for integrating AI-driven data parsing into applications that require structured output.

## Getting Started

### Prerequisites

- .NET SDK
- An API key for GroqCloud

### Running the Example

1. Store your Groq API key in the environment variables.
2. Compile and run the program:

```bash
dotnet run
```

### Example Code

```csharp
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
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.