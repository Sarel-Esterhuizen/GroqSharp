using GroqSharp.Models;
using GroqSharp.Tests.Mocks;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GroqSharp.Tests
{
    public class GroqClientTests
    {
        private GroqClient _client;
        private MockHttpMessageHandler _mockHandler;

        public GroqClientTests()
        {
            _mockHandler = new MockHttpMessageHandler(async request =>
            {
                var responseContent = new
                {
                    choices = new List<dynamic>
                    {
                        new { message = new { content = "Response content" } }
                    }
                };
                var responseJson = JsonSerializer.Serialize(responseContent);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                };
            });

            var httpClient = new HttpClient(_mockHandler);
            _client = new GroqClient("dummy_api_key", "dummy_model", httpClient);
        }

        [Fact]
        public async Task CreateChatCompletionAsync_ReturnsCorrectResponse()
        {
            var result = await _client.CreateChatCompletionAsync(
                new Message { Role = MessageRoleType.User, Content = "Test Request" }
            );

            Assert.Equal("Response content", result);
        }

        [Fact]
        public async Task CreateChatCompletionAsync_ThrowsOnError()
        {
            _mockHandler.SetHandler(async request =>
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Error")
                };
            });

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.CreateChatCompletionAsync(
                new Message { Role = MessageRoleType.User, Content = "Test Request" }
            ));

            Assert.Contains("Failed", ex.Message);
        }

        [Fact]
        public async Task CreateChatCompletionStreamAsync_ReturnsStreamedContent()
        {
            var responseLines = new[]
            {
                "data: {\"choices\": [{\"delta\": {\"content\": \"Streamed content 1\"}}]}",
                "data: {\"choices\": [{\"delta\": {\"content\": \"Streamed content 2\"}}]}",
                "data: [DONE]"
            };

            var responseStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", responseLines)));

            _mockHandler.SetHandler(async request =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(responseStream)
                };
            });

            var result = new List<string>();

            await foreach (var item in _client.CreateChatCompletionStreamAsync(
                new Message { Role = MessageRoleType.User, Content = "Test Request" }
            ))
            {
                result.Add(item);
            }

            Assert.Equal(2, result.Count);
            Assert.Equal("Streamed content 1", result[0]);
            Assert.Equal("Streamed content 2", result[1]);
        }

        [Fact]
        public async Task GetStructuredChatCompletionAsync_ReturnsCorrectResponse()
        {
            var jsonStructure = @"
            {
                ""summary"": ""string"",
                ""data"": {
                    ""speed"": ""string"",
                    ""accuracy"": ""string"",
                    ""scalability"": ""string""
                }
            }
            ";

            var response = await _client.GetStructuredChatCompletionAsync(
                jsonStructure,
                new Message { Role = MessageRoleType.User, Content = "Explain the importance of fast language models" }
            );

            Assert.Equal("Response content", response);
        }

        [Fact]
        public async Task GetStructuredChatCompletionAsync_RetriesOnError()
        {
            _mockHandler.SetHandler(async request =>
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Error")
                };
            });

            var jsonStructure = @"
            {
                ""summary"": ""string"",
                ""data"": {
                    ""speed"": ""string"",
                    ""accuracy"": ""string"",
                    ""scalability"": ""string""
                }
            }
            ";

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _client.GetStructuredChatCompletionAsync(
                jsonStructure,
                new Message { Role = MessageRoleType.User, Content = "Explain the importance of fast language models" }
            ));

            Assert.Contains("Failed", ex.Message);
        }
    }
}
