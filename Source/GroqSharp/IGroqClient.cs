using GroqSharp.Models;
using GroqSharp.Tools;

namespace GroqSharp
{
    public interface IGroqClient
    {
        Task<string> CreateChatCompletionAsync(
            params Message[] messages);

        Task<string> CreateChatCompletionWithToolsAsync(
            List<Message> messages,
            int depth = 0);

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

        IGroqClient RegisterTools(params IGroqTool[] tools);

        IGroqClient UnregisterTools(params string[] toolNames);
    }
}