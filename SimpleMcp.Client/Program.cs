using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCPSharp;
using MCPSharp.Model;
using SimpleMcp.LLM;

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

            const string llm = @"C:\Users\hassa\OneDrive\Desktop\AI Resources\mistral-7b-instruct-v0.1.Q8_0.gguf";

            var functions = await client.GetFunctionsAsync();

            var catalog = new
            {
                catalog = new
                {
                    functions = functions.Select(function => new
                    {
                        name = function.Name,
                        schema = function.JsonSchema
                    })
                }
            };

            var jsonOpts = new JsonSerializerOptions { WriteIndented = true };
            string functionCatalog = JsonSerializer.Serialize(catalog, jsonOpts);

            string userPrompt = "What do we get when we subtract 5 from ten from 20.";

            string prompt = @$"
CATALOG:
{functionCatalog}

USER REQUEST:
{userPrompt}

Respond with JSON only. Begin your output with ""{{"" and end with ""}}"".
JSON_ONLY_END
";

            var llmChat = new LLMChat(llm);
            string fullResponse = "";

            await foreach (string response in llmChat.SendAsync(prompt))
            {
                Console.Write(response);
                fullResponse += response;
            }

            string json = ExtractFirstJsonObject(fullResponse);
            McpRoot mcpRoot = JsonSerializer.Deserialize<McpRoot>(json);

           

            var result = await client.CallToolAsync(
                name: mcpRoot.Mcp.Function.Name,
                parameters: mcpRoot.Mcp.Function.Parameters.ToDictionary(p => p.Name, p => p.Value));

            Console.WriteLine($"MCP Server Response: {result.Content[0].Text}");
        }

        static string ExtractFirstJsonObject(string text)
        {
            int start = text.IndexOf('{');
            if (start == -1) return null;

            int depth = 0;
            bool inString = false;
            StringBuilder sb = new StringBuilder();

            for (int i = start; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"' && (i == 0 || text[i - 1] != '\\'))
                    inString = !inString;

                if (!inString)
                {
                    if (c == '{') depth++;
                    else if (c == '}') depth--;
                }

                sb.Append(c);

                if (depth == 0 && !inString)
                    break;
            }

            return sb.ToString();
        }
    }

    public class McpRoot
    {
        [JsonPropertyName("mcp")]
        public Mcp Mcp { get; set; }
    }

    public class Mcp
    {
        [JsonPropertyName("function")]
        public McpFunction Function { get; set; }
    }

    public class McpFunction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parameters")]
        public List<McpParameter> Parameters { get; set; }
    }

    public class McpParameter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }  // can also use string or int depending on expected data type
    }
}
