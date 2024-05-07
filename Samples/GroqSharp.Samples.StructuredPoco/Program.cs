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
                        Console.WriteLine($"Name: {item.Name}, Rank: {item.Powers.Rank}, Abilities: {item.Powers.Abilities}");
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
