using LLama.Common;
using LLama.Sampling;
using LLama;

namespace SimpleMcp.LLM
{
    public class LLMChat : IDisposable
    {
        // hold onto everything so we can dispose properly
        private readonly LLamaWeights modelWeights;
        private readonly LLamaContext llamaContext;
        private readonly InteractiveExecutor executor;
        private readonly InferenceParams inferenceParams;
        private readonly ChatSession session;
        private readonly ChatHistory history;

        public LLMChat(string modelPath, uint? ctxSize = 2048, int gpuLayers = 128)
        {
            var mp = new ModelParams(modelPath)
            {
                ContextSize = ctxSize,
                GpuLayerCount = gpuLayers
            };

            modelWeights = LLamaWeights.LoadFromFile(mp);
            llamaContext = modelWeights.CreateContext(mp);
            executor = new InteractiveExecutor(llamaContext);
            history = new ChatHistory();

            history.AddMessage(
                authorRole: AuthorRole.System,
                content: @"

You are a function router. Use ONLY the functions and parameters listed in the CATALOG. 
Task:
1) Pick exactly one function that best matches the USER REQUEST.
2) Produce parameter values from the request.

Output constraints (MUST follow exactly):
- Output ONLY a single JSON object.
- Do NOT echo the catalog or any prose.
- JSON shape:
{
  ""mcp"": {
    ""function"": {
      ""name"": ""<one function name from the catalog>"",
      ""parameters"": [
        { ""name"": ""<paramNameFromCatalog>"", ""value"": <JSON value or null> }
      ]
    }
  }
}

Rules:
- Use only parameters that appear in the chosen function’s catalog entry.
- If a required parameter value is missing, set ""value"": null.
- Parse numbers/booleans from text when obvious (""true"", ""3.14"").
- If nothing matches, return {""mcp"":{""function"":{""name"":""none"",""parameters"":[]}}}
- NEVER include keys other than: mcp, function, name, parameters, name, value.
- NEVER include the word ""Catalog"" or repeat the catalog content in your output.
JSON_ONLY_START

");


            session = new ChatSession(executor, history);

            inferenceParams = new InferenceParams
            {
                MaxTokens = 256,

                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.6f,
                    TopK = 40
                },

                AntiPrompts = new List<string> { "()" }, // Cut off early
            };
        }

        public async IAsyncEnumerable<string> SendAsync(string userMessage)
        {
            var message = new ChatHistory
                .Message(AuthorRole.User, userMessage);

            IAsyncEnumerable<string> response =
                session.ChatAsync(message, inferenceParams);

            await foreach (var tok in response)
                yield return tok;
        }

        public void Dispose()
        {
            llamaContext.Dispose();
            modelWeights.Dispose();
        }
    }
}
