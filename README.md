# 🧠 Simple MCP Server & Client in C#

Welcome to the **Simple MCP Server & Client** demo built using [`MCPSharp`](https://www.nuget.org/packages/MCPSharp/), a .NET implementation of the **Model-Context-Protocol (MCP)** framework for building AI-powered tools.

> 🎯 This repo shows how to define tools on the server side and call them from a local client — all in plain C#!

---

## 📦 What’s Inside

```
SimpleMcp/
├── SimpleMcp.Server/      # MCP Server with one registered tool: addition
│   └── CalculatorTool.cs
├── SimpleMcp.Client/      # MCP Client that calls the addition tool
│   └── Program.cs
└── README.md              # You're reading it :)
```

---

## 🚀 Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/yourusername/SimpleMcp.git
cd SimpleMcp
```

### 2. Build the server

```bash
cd SimpleMcp.Server
dotnet build
```

### 3. Run the client

Make sure the server path in the client is correctly pointing to the server executable:
```csharp
server: "D:\\path\\to\\SimpleMcp.Server.exe"
```

Then run:

```bash
cd ../SimpleMcp.Client
dotnet run
```

### ✅ Expected Output

```bash
115
```

📝 _(Because 5 + 10 + 100 = 115 — see the easter egg in `CalculatorTool`!)_

---

## 🛠️ MCP Concepts in Action

| Concept     | In This Project                       |
|-------------|----------------------------------------|
| **Model**   | `CalculatorTool.Addition`              |
| **Context** | Simple runtime context per call        |
| **Protocol**| MCPSharp handles tool invocation over local process communication |

---

## 📚 MCPSharp Usage

- Decorate tools with `[McpTool]`
- Define parameters using `[McpParameter]`
- Register tools on the server with `MCPServer.Register<T>()`
- Use `MCPClient` to call tools by name with parameters

---

## 💡 Why MCP?

MCP lets you define structured AI tools that can be embedded into agentic systems, LLM orchestration, and more. This example keeps it minimal, but real-world apps can scale up with context, memory, chaining, and local/remote AI models.

---

## 🧪 Example Tool

```csharp
[McpTool(name: "addition", Description = "This tool will add two numbers.")]
public static int Addition(
    [McpParameter(required: true)] int firstNumber,
    [McpParameter(required: true)] int secondNumber)
{
    return firstNumber + secondNumber + 100;
}
```

---

## 🙌 Credits

Inspired by the [Anthropic Model-Context-Protocol](https://github.com/anthropics/model-context-protocol) specification and the `MCPSharp` .NET implementation.

---

## 🧵 Want to Go Deeper?

- Add multiple tools
- Use session or memory for persistent context
- Connect to local AI models (e.g. Llama.cpp, Ollama) via MCP
- Build UI/agent shells around this infrastructure

---

## 📽️Watch YouTube video about this repo


[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/dG9nAAPLeVU/0.jpg)](https://youtu.be/dG9nAAPLeVU)



---

## 📜 License

MIT — use this freely for learning, demos, or real projects!
