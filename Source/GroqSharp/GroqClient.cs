using GroqSharp;
using GroqSharp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class GroqClient :
    IGroqClient
{
    private readonly HttpClient _client = new HttpClient();

    private const string DefaultBaseUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string ContentTypeJson = "application/json";
    private const string BearerTokenPrefix = "Bearer";
    private const string DataPrefix = "data: ";
    private const string StreamDoneSignal = "[DONE]";
    private const string ChoicesKey = "choices";
    private const string DeltaKey = "delta";
    private const string ContentKey = "content";

    private string _baseUrl = DefaultBaseUrl;
    private string _model;
    private double? _temperature;
    private int? _maxTokens;
    private double? _topP;
    private string? _stop;
    private Message _defaultSystemMessage;
    private int _maxStructuredRetryAttempts = 3;

    public GroqClient(
        string apiKey,
        string model,
        HttpClient? client = null)
    {
        _model = model;
        _client = client ?? new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerTokenPrefix, apiKey);
    }

    public IGroqClient SetBaseUrl(
        string baseUrl)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null.");
        return this;
    }

    public IGroqClient SetModel(
        string model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model), "Model cannot be null.");
        return this;
    }

    public IGroqClient SetTemperature(
        double? temperature)
    {
        _temperature = (temperature == null || (temperature >= 0.0 && temperature <= 2.0)) ? temperature : throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature is a value between 0 and 2.");
        return this;
    }

    public IGroqClient SetMaxTokens(
        int? maxTokens)
    {
        _maxTokens = maxTokens;
        return this;
    }

    public IGroqClient SetTopP(
        double? topP)
    {
        _topP = (topP == null || (topP >= 0.0 && topP <= 1.0)) ? topP : throw new ArgumentOutOfRangeException(nameof(topP), "TopP is a value between 0 and 1.");
        return this;
    }

    public IGroqClient SetStop(
        string stop)
    {
        _stop = stop;
        return this;
    }

    public IGroqClient SetDefaultSystemMessage(
        Message message)
    {
        _defaultSystemMessage = message ?? throw new ArgumentNullException(nameof(message), "Default system message cannot be null.");
        return this;
    }

    public IGroqClient SetStructuredRetryPolicy(
        int maxRetryAttempts)
    {
        _maxStructuredRetryAttempts = maxRetryAttempts;
        return this;
    }

    public async Task<string?> CreateChatCompletionAsync(
        params Message[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            if (_defaultSystemMessage != null)
            {
                messages = [_defaultSystemMessage];
            }
            else
            {
                throw new ArgumentException("Messages cannot be null or empty", nameof(messages));
            }
        }

        var request = new GroqClientRequest
        {
            Model = _model,
            Temperature = _temperature,
            Messages = messages,
            MaxTokens = _maxTokens,
            TopP = _topP,
            Stop = _stop
        };

        try
        {
            string requestJson = request.ToJson();
            var httpContent = new StringContent(requestJson, Encoding.UTF8, ContentTypeJson);
            HttpResponseMessage response = await _client.PostAsync(_baseUrl, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API request failed with status {response.StatusCode}: {errorResponse}");
            }

            var chatResponse = GroqClientResponse.TryCreateFromJson(await response.Content.ReadAsStringAsync());
            if (chatResponse != null &&
                chatResponse.Contents != null)
                return chatResponse.Contents.FirstOrDefault();
            return null;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to create chat completion", ex);
        }
    }

    public async Task<string?> GetStructuredChatCompletionAsync(
        string jsonStructure, 
        params Message[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            if (_defaultSystemMessage != null)
            {
                messages = new Message[] { _defaultSystemMessage };
            }
            else
            {
                throw new ArgumentException("Messages cannot be null or empty", nameof(messages));
            }
        }

        // Check if a system message is present
        var systemMessageIndex = messages.ToList().FindIndex(m => m.Role == MessageRole.System);
        if (systemMessageIndex != -1)
        {
            // Extend the existing system message
            messages[systemMessageIndex].Content += $"\nIMPORTANT: Please respond ONLY in JSON format as follows:\n{jsonStructure}";
        }
        else
        {
            // Create a new system message with explicit instructions
            var newSystemMessage = new Message
            {
                Role = MessageRole.System,
                Content = $"IMPORTANT: Please respond ONLY in JSON format as follows:\n{jsonStructure}"
            };
            messages = new Message[] { newSystemMessage }.Concat(messages).ToArray();
        }

        var request = new GroqClientRequest
        {
            Model = _model,
            Temperature = _temperature,
            Messages = messages,
            MaxTokens = _maxTokens,
            TopP = _topP,
            Stop = _stop
        };

        for (int attempt = 1; attempt <= _maxStructuredRetryAttempts; attempt++)
        {
            try
            {
                string requestJson = request.ToJson();
                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_baseUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API request failed with status {response.StatusCode}: {errorResponse}");
                }

                var chatResponse = GroqClientResponse.TryCreateFromJson(await response.Content.ReadAsStringAsync());
                return chatResponse?.Contents?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (attempt == _maxStructuredRetryAttempts)
                {
                    throw new ApplicationException("Failed to create chat completion", ex);
                }
            }
        }

        return null;
    }

    public async IAsyncEnumerable<string> CreateChatCompletionStreamAsync(
        params Message[] messages)
    {
        if (messages == null || messages.Length == 0)
            throw new ArgumentException("Messages cannot be null or empty", nameof(messages));

        var request = new GroqClientRequest
        {
            Model = _model,
            Temperature = _temperature,
            Messages = messages,
            MaxTokens = _maxTokens,
            TopP = _topP,
            Stop = _stop,
            Stream = true
        };

        string requestJson = request.ToJson();
        var httpContent = new StringContent(requestJson, Encoding.UTF8, ContentTypeJson);

        HttpResponseMessage response = await _client.PostAsync(_baseUrl, httpContent);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed with status {response.StatusCode}: {errorResponse}");
        }

        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith(DataPrefix))
            {
                var data = line.Substring(DataPrefix.Length);
                if (data != StreamDoneSignal)
                {
                    using var doc = JsonDocument.Parse(data);
                    var jsonElement = doc.RootElement;
                    if (jsonElement.TryGetProperty(ChoicesKey, out var choices) && choices.GetArrayLength() > 0)
                    {
                        var firstChoice = choices[0];
                        if (firstChoice.TryGetProperty(DeltaKey, out var delta) && delta.TryGetProperty(ContentKey, out var content))
                        {
                            var text = content.GetString();
                            if (!string.IsNullOrEmpty(text))
                            {
                                yield return text;
                            }
                        }
                    }
                }
            }
        }
    }
}