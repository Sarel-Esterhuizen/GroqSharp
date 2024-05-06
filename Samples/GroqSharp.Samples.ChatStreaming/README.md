# Chat Streaming Sample

This sample demonstrates how to use the GroqSharp library to handle streaming chat completions.

## Overview

The Chat Streaming project showcases how to set up a streaming chat session using GroqSharp, enabling real-time response processing.

## Getting Started

### Prerequisites

- .NET SDK
- An API key for GroqCloud

### Running the Example

1. Store your Groq API key in the environment variables.
2. Run the project:

```bash
dotnet run
```

### Example Code

```csharp
using GroqSharp.Models;
using System;
using System.Threading.Tasks;

namespace GroqSharp.Samples.ChatStreaming
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
            var apiModel = "llama3-70b-8192";

            IGroqClient groqClient = new GroqClient(apiKey, apiModel)
                .SetTemperature(0.5)
                .SetMaxTokens(256)
                .SetTopP(1)
                .SetStop("NONE");

            try
            {
                var messages = new[]
                {
                    new Message { Role = MessageRoleType.User, Content = "Give some lyrics to a song." },
                    new Message { Role = MessageRoleType.System, Content = "You are the love child of Britney Spears and Eminem." }
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

            Console.WriteLine("\n[Enter to continue]");
            Console.ReadLine(); 
        }
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
