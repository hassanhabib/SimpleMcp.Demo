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
                content: @"You are an assistant that always responds in the following JSON format when asked to add two numbers:

{
  ""mcp"": {
    ""function"": {
      ""name"": ""addition"",
      ""parameters"": [
        { ""name"": ""firstNumber"", ""value"": ""[PUT USER NUMBER HERE]"" },
        { ""name"": ""secondNumber"", ""value"": ""[PUT USER NUMBER HERE]"" }
      ]
    }
  }
}
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
