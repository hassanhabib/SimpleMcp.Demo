using MCPSharp;
using MCPSharp.Model;

namespace SimpleMcp.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MCPClient(
                name: "McpClient",
                version: "v1.0.0",
                server: "D:\\source\\repos\\SimpleMcp\\SimpleMcp.Server\\bin\\Debug\\net9.0\\SimpleMcp.Server.exe");

            var result = await client.CallToolAsync(
                name: "addition",
                parameters: new Dictionary<string, object>
                {
                    {"firstNumber", 5 },
                    {"secondNumber", 10 }
                });

            Console.WriteLine(result.Content[0].Text);
        }
    }
}
