using GroqSharp.Tools;

namespace GroqSharp.Samples.ToolIntegration
{
    public class CurrencyConversionTool : 
        IGroqTool
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

        public async Task<string> ExecuteAsync(
            IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue("amount", out var amountObj) &&
                parameters.TryGetValue("from_currency", out var fromCurrencyObj) &&
                parameters.TryGetValue("to_currency", out var toCurrencyObj))
            {
                string fromCurrency = fromCurrencyObj.ToString();
                string toCurrency = toCurrencyObj.ToString();
                decimal amount = Convert.ToDecimal(amountObj);

                // Retrieve the exchange rate for the given currency pair
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
