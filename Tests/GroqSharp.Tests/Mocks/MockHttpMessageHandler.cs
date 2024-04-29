namespace GroqSharp.Tests.Mocks
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private Func<HttpRequestMessage, Task<HttpResponseMessage>> _handlerFunc;

        public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        public void SetHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> newHandlerFunc)
        {
            _handlerFunc = newHandlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request);
        }
    }
}
