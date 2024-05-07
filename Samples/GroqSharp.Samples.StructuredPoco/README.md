# Structured Poco Sample

This sample demonstrates the use of plain POCO objects for structured JSON requests with the GroqSharp library.

## Overview

The StructuredPoco project illustrates how to configure the GroqClient to receive and process structured JSON responses, automatically mapping them to predefined POCO (Plain Old CLR Object) structures. This approach facilitates the integration of AI-driven data parsing in applications that require structured outputs, making it ideal for scenarios where data schema adherence is crucial.

## Getting Started

### Prerequisites

- **.NET SDK**: Ensure you have the latest .NET SDK installed on your machine. This is necessary to compile and run the .NET applications.
- **Groq API Key**: You will need a valid API key from GroqCloud to authenticate and use the Groq APIs.

### Running the Example

1. **Set API Key**: Place your Groq API key in your environment variables under the name `GROQ_API_KEY`.
2. **Compile and Execute**: Navigate to the project directory in your terminal and execute the following commands:

   ```bash
   dotnet build
   dotnet run
   ```

### Example Code

Here is the core snippet of the application demonstrating how the structured data is handled:

```csharp
using GroqSharp;
using GroqSharp.Models;
using GroqSharp.Samples.StructuredPoco.Models;

namespace GroqSharp.Samples.StructuredPoco
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
            var apiModel = "llama3-70b-8192";

            IGroqClient groqClient = new GroqClient(apiKey, apiModel);

            try
            {
                var response = await groqClient.GetStructuredChatCompletionAsync<SuperHeroCollection>(
                    new Message { Role = MessageRoleType.System, Content = "You are a helpful, smart, kind, and efficient AI assistant." },
                    new Message { Role = MessageRoleType.User, Content = "Give me a few Superhero characters." }
                );

                if (response != null)
                {
                    foreach (var item in response.Results)
                    {
                        Console.WriteLine(
                            $"Name:        {item.Name}\n" +
                            $"Description: {item.Description}\n" +
                            $"Real Name:   {item.RealName}\n" +
                            $"Age:         {item.Age}\n" +
                            $"Rate:        {item.HiringRatePerHour}\n" +
                            $"Rank:        {item.Powers.Rank}\n" +
                            $"Abilities:   {item.Powers.Abilities}\n");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine("No response received.");
                }
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

## Context Attributes

In this sample, we enhance our POCO models using `GroqSharp.Annotations` to provide more explicit context to each property. These attributes allow developers to define additional metadata for properties, which can be used to enforce data validation and improve readability and maintenance of the code. Here's how you can apply these annotations:

```csharp
using GroqSharp.Annotations;

namespace GroqSharp.Samples.StructuredPoco.Models
{
    public class SuperHero
    {
        [ContextDescription("Name of the superhero - make it up not a common name")]
        public string Name { get; set; }

        [ContextDescription("Description of the superhero as a pirate")]
        [ContextLength(250)]
        public string Description { get; set; }

        public string RealName { get; set; }

        [ContextDescription("The age in human years")]
        [ContextRange(min:0, max:1000)]
        public int Age { get; set; }

        [ContextDescription("The freelance hiring rate in dollars and cents")]
        [ContextRange(min: 10.00, max: 999.99)]
        public double HiringRatePerHour { get; set; }

        public PowerDetail Powers { get; set; }
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
