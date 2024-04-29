using GroqSharp.Models;

namespace GroqSharp
{
    public interface IGroqClient
    {
        Task<string> CreateChatCompletionAsync(
            params Message[] messages);

        IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
            params Message[] messages);

        Task<string?> GetStructuredChatCompletionAsync(
            string jsonStructure,
            params Message[] messages);

        IGroqClient SetBaseUrl(string baseUrl);

        IGroqClient SetDefaultSystemMessage(Message message);

        IGroqClient SetMaxTokens(int? maxTokens);

        IGroqClient SetModel(string model);

        IGroqClient SetStop(string stop);

        IGroqClient SetTemperature(double? temperature);

        IGroqClient SetTopP(double? topP);

        IGroqClient SetStructuredRetryPolicy(int maxRetryAttempts);
    }
}