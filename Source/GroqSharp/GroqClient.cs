using GroqSharp;
using GroqSharp.Models;
using GroqSharp.Tools;
using GroqSharp.Utilities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class GroqClient :
    IGroqClient
{
    #region Class Fields

    private const string DefaultBaseUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string ContentTypeJson = "application/json";
    private const string BearerTokenPrefix = "Bearer";
    private const string DataPrefix = "data: ";
    private const string StreamDoneSignal = "[DONE]";
    private const string ChoicesKey = "choices";
    private const string DeltaKey = "delta";
    private const string ContentKey = "content";
    private const string FunctionTypeKey = "function";

    private static readonly Dictionary<Type, string> _typeCache = new Dictionary<Type, string>();

    #endregion

    #region Instance Fields

    private readonly HttpClient _client = new HttpClient();
    private readonly Dictionary<string, IGroqTool> _tools = new Dictionary<string, IGroqTool>();

    private string _baseUrl = DefaultBaseUrl;
    private string _model;
    private double? _temperature;
    private int? _maxTokens;
    private double? _topP;
    private string? _stop;
    private Message _defaultSystemMessage;
    private int _maxStructuredRetryAttempts = 3;
    private int _maxToolInvocationDepth = 3;

    #endregion

    #region Constructors

    public GroqClient(
        string apiKey,
        string model,
        HttpClient? client = null)
    {
        _model = model;
        _client = client ?? new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerTokenPrefix, apiKey);
    }

    #endregion

    #region Instance Methods

    #region Fluent Methods

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

    public IGroqClient SetMaxToolInvocationDepth(
        int maxDepth)
    {
        _maxToolInvocationDepth = maxDepth;
        return this;
    }

    public IGroqClient RegisterTools(
        params IGroqTool[] tools)
    {
        foreach (var tool in tools)
            _tools[tool.Name] = tool;
        return this;
    }

    public IGroqClient UnregisterTools(
        params string[] toolNames)
    {
        foreach (var toolName in toolNames)
        {
            _tools.Remove(toolName);
        }
        return this;
    }

    #endregion

    #region Helper Methods

    public object BuildToolSpecifications()
    {
        var toolsList = new List<object>();
        foreach (var tool in _tools.Values)
        {
            var toolSpec = new
            {
                type = FunctionTypeKey,
                function = new
                {
                    name = tool.Name,
                    description = tool.Description,
                    parameters = tool.Parameters.ToDictionary(
                        param => param.Key,
                        param => param.Value.ToJsonSerializableObject()
                    )
                }
            };
            toolsList.Add(toolSpec);
        }

        return toolsList;
    }

    private async Task<string> HandleToolResponsesAndReinvokeAsync(
        List<Message> messages,
        GroqClientResponse response,
        int depth)
    {
        // First check if max depth is exceeded
        if (depth >= _maxToolInvocationDepth)
        {
            throw new InvalidOperationException("Maximum tool invocation depth exceeded, possible loop detected.");
        }

        // Add the assistant's original response to the conversation before handling tool calls
        if (response.Contents.Any())
        {
            messages.Add(new Message
            {
                Role = MessageRoleType.Assistant,
                Content = response.Contents.FirstOrDefault()
            });
        }

        // Handle any tool calls
        if (response.ToolCalls != null && response.ToolCalls.Any())
        {
            foreach (var call in response.ToolCalls)
            {
                if (_tools.TryGetValue(call.ToolName, out var tool))
                {
                    var toolResult = await tool.ExecuteAsync(call.Parameters);
                    messages.Add(new MessageTool
                    {
                        Role = MessageRoleType.Tool,
                        Content = toolResult,
                        ToolCallId = call.Id
                    });
                }
            }

            // Reinvoke the API with the updated messages
            return await CreateChatCompletionWithToolsAsync(messages, depth);
        }

        // If there were no tool calls, just return the original response
        return response.Contents.FirstOrDefault();
    }



    #endregion

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

    public async Task<string> CreateChatCompletionWithToolsAsync(
        List<Message> messages,
        int depth = 0)
    {
        if (depth >= _maxToolInvocationDepth)
        {
            throw new InvalidOperationException("Maximum tool invocation depth exceeded, possible loop detected.");
        }

        if (messages == null || messages.Count == 0)
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

        // Build request with potential tools included
        var toolSpecs = BuildToolSpecifications();

        var request = new GroqClientRequest
        {
            Model = _model,
            Messages = messages.ToArray(),
            Tools = toolSpecs,
            ToolChoice = "auto"
        };

        string requestJson = request.ToJson();
        var content = new StringContent(requestJson, Encoding.UTF8, ContentTypeJson);
        var response = await _client.PostAsync(_baseUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {await response.Content.ReadAsStringAsync()}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var chatResponse = GroqClientResponse.TryCreateFromJson(responseJson);
        return await HandleToolResponsesAndReinvokeAsync(messages, chatResponse, depth + 1);
    }


    public async Task<string?> GetStructuredChatCompletionAsync(
        string jsonStructure,
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

        // Check if a system message is present
        var systemMessageIndex = messages.ToList().FindIndex(m => m.Role == MessageRoleType.System);
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
                Role = MessageRoleType.System,
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

    public async Task<TResponse?> GetStructuredChatCompletionAsync<TResponse>(
        params Message[] messages) 
        where TResponse : class, new()
    {
        if (messages == null || messages.Length == 0)
        {
            throw new ArgumentException("Messages cannot be null or empty");
        }

        // Generate the JSON structure from the response type
        string jsonStructure = JsonStructureUtility.CreateJsonStructureFromType(typeof(TResponse), _typeCache);

        // Extend the system message to include the JSON structure
        var systemMessageIndex = messages.ToList().FindIndex(m => m.Role == MessageRoleType.System);
        if (systemMessageIndex != -1)
        {
            messages[systemMessageIndex].Content += $" IMPORTANT: Please respond ONLY in JSON format as follows: {jsonStructure}";
        }
        else
        {
            // Add a new system message if none exists
            var newSystemMessage = new Message
            {
                Role = MessageRoleType.System,
                Content = $"IMPORTANT: Please respond ONLY in JSON format as follows:\n{jsonStructure}"
            };
            var messageList = new List<Message>(messages) { newSystemMessage };
            messages = messageList.ToArray();
        }

        // Call the existing method to get the JSON response
        string jsonResponse = await GetStructuredChatCompletionAsync(jsonStructure, messages);

        if (string.IsNullOrEmpty(jsonResponse))
        {
            throw new InvalidOperationException("Received an empty response from the API.");
        }

        // Deserialize the JSON response back into the expected type
        TResponse responsePoco = JsonSerializer.Deserialize<TResponse>(jsonResponse) ??
                                 throw new InvalidOperationException("Failed to deserialize the response.");

        return responsePoco;
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

    #endregion
}