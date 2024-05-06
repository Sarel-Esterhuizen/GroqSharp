# Tool Integration Sample

This sample demonstrates how to integrate and utilize custom tools with the GroqSharp library for enhanced functionality.

## Overview

The Tool Integration project showcases the integration of a custom tool, specifically for currency conversion, using the GroqSharp library. This example illustrates how tools can be registered and used within the GroqClient to provide additional functionality beyond basic chat completions.

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
```

### Tool Code Example

```csharp
using GroqSharp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroqSharp.Samples.ToolIntegration
{
    public class CurrencyConversionTool : IGroqTool
    {
        public string Name => "convert_currency";
        public string Description => "Converts an amount from one currency to another.";
        public IDictionary<string, IGroqToolParameter> Parameters => new Dictionary<string, IGroqToolParameter>
        {
            { "amount", new GroqToolParameter(GroqToolParameterType.Integer, "Amount to convert.") },
            { "from_currency", new GroqToolParameter(GroqToolParameterType.String, "Currency code of the source amount, e.g., 'USD'.") },
            { "to_currency", new GroqToolParameter(GroqToolParameterType.String, "Currency code to convert to, e.g., 'EUR'.") }
        };

        private readonly Dictionary<(string, string), decimal> _exchangeRates = new Dictionary<(string, string), decimal>
        {
            { ("USD", "EUR"), 0.85M },
            { ("EUR", "USD"), 1.17M },
            { ("USD", "JPY"), 110.25M },
            { ("GBP", "EUR"), 1.16M },
            { ("EUR", "GBP"), 0.86M }
        };

        public async Task<string> ExecuteAsync(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue("amount", out var amountObj) &&
                parameters.TryGetValue("from_currency", out var fromCurrencyObj) &&
                parameters.TryGetValue("to_currency", out var toCurrencyObj))
            {
                string fromCurrency = fromCurrencyObj.ToString();
                string toCurrency = toCurrencyObj.ToString();
                decimal amount = Convert.ToDecimal(amountObj);

                if (_exchangeRates.TryGetValue((fromCurrency, toCurrency), out var exchangeRate))
                {
                    decimal convertedAmount = amount * exchangeRate;
                    return $"{amount} {fromCurrency} is approximately {convertedAmount:N2} {toCurrency}.";
                }
                else
                {
                    return $"Conversion rate not available from {fromCurrency} to {toCurrency}.";
                }
            }

            return "Invalid parameters provided.";
        }
    }
}
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.