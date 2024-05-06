using GroqSharp.Models;

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
