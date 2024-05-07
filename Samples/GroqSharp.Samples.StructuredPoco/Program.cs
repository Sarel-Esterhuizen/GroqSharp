using GroqSharp.Models;
using GroqSharp.Samples.StructuredPoco.Models;

namespace GroqSharp.Samples.StructuredPoco
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
            //var apiModel = "llama3-70b-8192"; 
            //var apiModel = "llama3-8b-8192";
            var apiModel = "mixtral-8x7b-32768";

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
