using MCPSharp;

namespace SimpleMcp.Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MCPServer.Register<CalculatorTool>();

            await MCPServer.StartAsync(
                serverName: "SimpleMcp.Server",
                version: "v1.0.0");
        }
    }

    public class CalculatorTool
    {
        [McpTool(name: "addition", Description = "This tool will add two numbers.")]
        public static int Addition(
            [McpParameter(required: true, description: "First number")] int firstNumber,
            [McpParameter(required: true, description: "Second number")] int secondNumber)
        {
            return firstNumber + secondNumber;
        }
    }
}
