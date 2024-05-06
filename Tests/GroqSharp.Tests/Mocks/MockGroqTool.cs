using GroqSharp.Tools;

namespace GroqSharp.Tests.Mocks
{
    internal class MockGroqTool : 
        GroqTool
    {
        public override string Name => "MockTool";
        public override string Description => "A mock tool for testing.";
        public override Dictionary<string, IGroqToolParameter> Parameters => new Dictionary<string, IGroqToolParameter>
        {
            { "testParam", new GroqToolParameter(GroqToolParameterType.String, "A test parameter") }
        };

        public override Task<string> ExecuteAsync(IDictionary<string, object> parameters)
        {
            return Task.FromResult("Executed");
        }
    }
}
