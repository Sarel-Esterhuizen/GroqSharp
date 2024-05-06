# Chat Basics Sample

This sample demonstrates the basic usage of the GroqSharp library to create chat completions.

## Overview

The Chat Basics project shows how to configure the GroqClient with basic parameters and make a simple API call to generate a chat completion.

## Getting Started

### Prerequisites

- .NET SDK
- An API key for GroqCloud

### Running the Example

1. Ensure that your Groq API key is stored in your environment variables.
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
```

## License
This project is licensed under the MIT License - see the LICENSE file for details.