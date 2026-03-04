# OpenAI API Client Kit — API Usage Guide

This guide describes the **public API surface** you’re expected to use:

- **Beginner (Direct Client)** – `ChatClient` + `ClientRequestBuilder`
- **Advanced (Orchestration)** – model registry, dispatchers, executors, response handlers
- **Migration Guide** – moving from direct usage to orchestration

Assume these namespaces:

```csharp
using OpenAIApiClient;
using OpenAIApiClient.Enums;
using OpenAIApiClient.Helpers.General;
using OpenAIApiClient.Orchestration;
using OpenAIApiClient.Orchestration.Dispatch;
using OpenAIApiClient.Orchestration.Execution;
using OpenAIApiClient.Registries;
```

---

## Part 1 — Beginner API Usage (Direct Client)

### 1.1 ChatClient Overview

`ChatClient` is the primary low‑level client for OpenAI chat completions.

Capabilities:

- Non‑streaming and streaming chat completions
- Automatic retries with exponential backoff + jitter
- HTTP 429 handling with `Retry-After`
- Configurable retry parameters

You typically create one `ChatClient` per process / configuration.

### 1.2 Creating a ChatClient

```csharp
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable and try again.");
    return;
}

// Basic client with default retry settings
ChatClient client = new(apiKey: apiKey);

// Or with custom retry configuration
ChatClient resilientClient = new(apiKey: apiKey, maxRetries: 5, baseDelayMs: 1000);
```

### 1.3 Building Requests with ClientRequestBuilder

`ClientRequestBuilder` is a fluent builder around `ChatCompletionRequest`.

Core methods (based on public usage):

- `WithModel(OpenAIModel input)`  
- `AddSystemMessage(string input)`  
- `AddUserMessage(string input)`  
- `UsingMaxTokens(int input)`  
- `EnableStreaming(bool input)`  
- `WithTemperature(double input)`  
- `WithTopP(double input)` – clamps to `[0.0, 1.0]`  

  ```csharp
  public ClientRequestBuilder WithTopP(double input)
  {
      // Clamp to [0.0, 1.0]
      input = input < 0.0 ? 0.0 : input > 1.0 ? 1.0 : input;
      this.topP = input;
      return this;
  }
  ```
- `WithPresencePenalty(double input)`  
- `WithFrequencyPenalty(double input)`  
- `Build()`  

#### Example: Basic Request (Non‑Streaming)

```csharp
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
ChatClient client = new(apiKey: apiKey);

ChatCompletionRequest request =
    new ClientRequestBuilder()
        .WithModel(input: OpenAIModel.GPT4o_Mini)
        .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
        .AddUserMessage(input: "Write a haiku about winter mornings.")
        .UsingMaxTokens(input: 1000)
        .EnableStreaming(input: false)
        .WithTemperature(input: 0.7)
        .WithTopP(input: 0.9)
        .WithPresencePenalty(input: 0.0)
        .WithFrequencyPenalty(input: 0.0)
        .Build();
```

### 1.4 Non‑Streaming Chat Completion

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

ChatCompletionResponse? response =
    await client.CreateChatCompletionAsync(request: request, cancelToken: cts.Token);

string? text = response?.Choices[0].Message.Content;
Console.WriteLine(text);
```

### 1.5 Streaming Chat Completion

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

// Ensure EnableStreaming(true) was set on the builder
string responseText = string.Empty;

await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(
                   request: request,
                   cancelToken: cts.Token))
{
    ChatDelta delta = chunk.Choices[0].Delta;
    responseText += delta.Content;
    Console.Write(delta.Content); // stream to console as it arrives
}

Console.WriteLine();
Console.WriteLine("Final response:");
Console.WriteLine(responseText);
```

### 1.6 Error Handling and Retries

Error handling is built into `ChatClient`:

- Transient network errors → automatic retry
- HTTP 429 → obeys `Retry-After` when present
- Exponential backoff + jitter between retries

You control this via constructor parameters:

```csharp
ChatClient client = new(
    apiKey: apiKey,
    maxRetries: 5,
    baseDelayMs: 1000   // starting delay in milliseconds
);
```

---

## Part 2 — Advanced API Usage (Orchestration)

The orchestration layer lets you:

- Choose models by capability, cost, reasoning strength, etc.
- Run **ensembles** of models in parallel
- Implement **model‑as‑judge**, **heuristic decisions**, and **response synthesis**
- Centralize model metadata, pricing, and capabilities

### 2.1 Model Registry

#### 2.1.1 OpenAIModel Enum (Conceptual)

`OpenAIModel` enumerates all supported models:

- GPT‑5 family (`GPT5_2`, `GPT5_2_Pro`, `GPT5`, `GPT5_Mini`, `GPT5_Nano`)
- GPT‑4.1 family (`GPT4_1`, `GPT4_1_Mini`, `GPT4_1_Reasoning`, `GPT4_1_Critic`, `GPT4_Turbo`)
- GPT‑4o (`GPT4o`, `GPT4o_Mini`)
- GPT‑3.5 Turbo (`GPT3_5_Turbo`)
- Embeddings, audio, image, moderation, etc.

Example:

```csharp
OpenAIModel model = OpenAIModel.GPT4o_Mini;
```

#### 2.1.2 OpenAIModels Registry

```csharp name=OpenAIModels.cs url=https://github.com/captainkirk854/openai-api-client-kit/blob/ea4a175a64dc9d1c66af8a45f4f82310d8ada8f1/OpenAIApiClientKit/OpenAIApiClient/Registries/OpenAIModels.cs#L7-L465
public sealed class OpenAIModels
{
    // Internal dictionary<OpenAIModel, ModelDescriptor> models;

    /// <summary>Gets the complete model registry dictionary.</summary>
    public Dictionary<OpenAIModel, ModelDescriptor> Registry => this.models;

    /// <summary>Gets all registered model descriptors.</summary>
    public IEnumerable<ModelDescriptor> All => this.models.Values;

    /// <summary>Gets a single model descriptor by enum key.</summary>
    public ModelDescriptor Get(OpenAIModel model) => this.models[model];
}
```

Usage:

```csharp
OpenAIModels registry = new();

ModelDescriptor gpt4oMini = registry.Get(OpenAIModel.GPT4o_Mini);

Console.WriteLine(gpt4oMini.Name);           // GPT4o_Mini
Console.WriteLine(gpt4oMini.Pricing.InputTokenCost);
Console.WriteLine(string.Join(',', gpt4oMini.Capabilities));
```

### 2.2 Orchestration Types

#### 2.2.1 OrchestrationRequest

```csharp name=OrchestrationRequest.cs url=https://github.com/captainkirk854/openai-api-client-kit/blob/ea4a175a64dc9d1c66af8a45f4f82310d8ada8f1/OpenAIApiClientKit/OpenAIApiClient/Orchestration/OrchestrationRequest.cs#L7-L56
public sealed class OrchestrationRequest
{
    public bool UseEnsemble { get; init; }
    public string Prompt { get; init; } = string.Empty;
    public OutputFormat OutputFormat { get; init; }

    public SingleModelDispatchRequest? SingleModelRequest { get; init; }
    public EnsembleDispatchRequest?    EnsembleRequest    { get; init; }
}
```

- `UseEnsemble = false` → use `SingleModelRequest` and the **single‑model dispatcher**
- `UseEnsemble = true` → use `EnsembleRequest` and the **ensemble dispatcher**

#### 2.2.2 AiModelResponse

```csharp name=AiModelResponse.cs url=https://github.com/captainkirk854/openai-api-client-kit/blob/ea4a175a64dc9d1c66af8a45f4f82310d8ada8f1/OpenAIApiClientKit/OpenAIApiClient/Orchestration/AiModelResponse.cs#L1-L88
public sealed class AiModelResponse
{
    public ModelDescriptor Model    { get; init; } = default!;
    public string         RawOutput { get; init; } = string.Empty;
    public bool           IsSuccessful { get; init; }
    public string?        ErrorMessage { get; init; }
    public TimeSpan       Latency { get; init; }
    public decimal        TotalTokens { get; init; }
    public decimal        EstimatedCost { get; init; }
}
```

You get one (single‑model) or many (ensemble) of these from the orchestrator.

#### 2.2.3 Orchestrator

```csharp name=Orchestrator.cs url=https://github.com/captainkirk854/openai-api-client-kit/blob/ea4a175a64dc9d1c66af8a45f4f82310d8ada8f1/OpenAIApiClientKit/OpenAIApiClient/Orchestration/Orchestrator.cs#L1-L72
public sealed class Orchestrator
{
    public Orchestrator(
        ISingleModelDispatcher singleModelDispatcher,
        IEnsembleDispatcher    ensembleDispatcher,
        ISingleModelExecutor   singleModelExecutor,
        IEnsembleExecutor      ensembleExecutor,
        ClientRequestBuilder   requestBuilder,
        IAiModelResponseHandler responseHandler)
    { /* ... */ }

    public Task<IReadOnlyList<AiModelResponse>> ProcessAsync(
        OrchestrationRequest request,
        CancellationToken cancelToken);
}
```

> In typical usage you won’t wire all of these manually; you’ll use a **Fluent Orchestrator Builder** that composes default components and lets you override specific ones.

### 2.3 Simple Orchestrator Creation

Conceptually (exact builder API may vary slightly):

```csharp
// Assuming a Fluent Orchestrator Builder as described in README
var orchestrator = new OrchestratorBuilder()
    .WithClient(client)               // ChatClient
    .WithResponseHandler(handler)     // your IAiModelResponseHandler
    .Build();
```

Or fully customized:

```csharp
var orchestrator = new OrchestratorBuilder()
    .WithClient(client)
    .WithResponseHandler(customHandler)
    .WithModelRegistry(customRegistry)
    .WithRequestBuilder(customRequestBuilder)
    .WithSingleModelDispatcher(customSingleDispatcher)
    .WithSingleModelExecutor(customSingleExecutor)
    .WithEnsembleDispatcher(customEnsembleDispatcher)
    .WithEnsembleExecutor(customEnsembleExecutor)
    .Build();
```

### 2.4 Example: Single Model — Best Reasoning Strategy

```csharp
Orchestrator orchestrator = CreateDefaultOrchestrator(client);

var request = new OrchestrationRequest
{
    UseEnsemble = false,
    Prompt = "Explain why the sky is blue.",
    OutputFormat = OutputFormat.PlainText,
    SingleModelRequest = new SingleModelDispatchRequest
    {
        Strategy = SingleModelStrategy.BestReasoning,
        RequiredCapabilities =
        [
            ModelCapability.Text,
            ModelCapability.Chat,
            ModelCapability.Reasoning
        ]
    }
};

IReadOnlyList<AiModelResponse> responses =
    await orchestrator.ProcessAsync(request, CancellationToken.None);

AiModelResponse best = responses.Single();

Console.WriteLine(best.RawOutput);
Console.WriteLine($"Tokens: {best.TotalTokens}, Cost: {best.EstimatedCost:C}, Latency: {best.Latency}");
```

### 2.5 Example: Ensemble — Model‑as‑Judge & Response Synthesis

In an ensemble flow, your `IAiModelResponseHandler` is where **model‑as‑judge**, **heuristics**, and **response synthesis** live:

1. Dispatcher selects multiple candidate models.
2. Ensemble executor runs them in parallel and returns several `AiModelResponse`s.
3. Response handler:
   - Optionally calls a “judge” model
   - Applies heuristics (cost, tokens, judge scores, etc.)
   - Returns either:
     - The “winning” response, or
     - A synthesized answer combining multiple responses.

```csharp
Orchestrator orchestrator = CreateDefaultOrchestrator(client);

var request = new OrchestrationRequest
{
    UseEnsemble = true,
    Prompt = "Give three different architectures for a plugin system in C#.",
    OutputFormat = OutputFormat.Markdown,
    EnsembleRequest = new EnsembleDispatchRequest
    {
        Strategy = EnsembleStrategy.Custom,
        RequiredCapabilities =
        [
            ModelCapability.Text,
            ModelCapability.Chat,
            ModelCapability.Reasoning
        ]
    }
};

IReadOnlyList<AiModelResponse> results =
    await orchestrator.ProcessAsync(request, CancellationToken.None);

// Depending on IAiModelResponseHandler:
// - results might contain all candidates, or
// - you might already get a single synthesized/best response
foreach (var r in results)
{
    Console.WriteLine($"Model: {r.Model.Name}, Success: {r.IsSuccessful}, Cost: {r.EstimatedCost:C}");
}
```

See `AIModelBestResponseDemo`, `AIModelOrchestratorDemo`, and `ResponseHandlerDemo` in the console app for concrete orchestration flows.

---

## Part 3 — Migration Guide

This section shows how to **move from direct `ChatClient` usage** to the **orchestration framework** in incremental steps.

### 3.1 Starting Point: Direct ChatClient

A typical starting point (non‑orchestrated) looks like this:

```csharp
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
ChatClient client = new(apiKey: apiKey);

ChatCompletionRequest request =
    new ClientRequestBuilder()
        .WithModel(OpenAIModel.GPT4o_Mini)
        .AddSystemMessage("You are a helpful assistant.")
        .AddUserMessage("Explain the theory of relativity.")
        .UsingMaxTokens(800)
        .Build();

ChatCompletionResponse? response =
    await client.CreateChatCompletionAsync(request, CancellationToken.None);

Console.WriteLine(response?.Choices[0].Message.Content);
```

### 3.2 Step 1 — Switch to Registry‑Backed Models

Use the model registry so your model choices are centralized and typed.

**Before:**

```csharp
.WithModel(OpenAIModel.GPT4o_Mini)
```

**After:**

```csharp
OpenAIModels models = new();
ModelDescriptor descriptor = models.Get(OpenAIModel.GPT4o_Mini);

ChatCompletionRequest request =
    new ClientRequestBuilder()
        .WithModel(descriptor.Name)  // still strongly typed
        .AddSystemMessage("You are a helpful assistant.")
        .AddUserMessage("Explain the theory of relativity.")
        .UsingMaxTokens(800)
        .Build();
```

Benefits:

- Single place to manage model capabilities & pricing
- Easy to switch to other models based on capabilities later

### 3.3 Step 2 — Introduce Orchestrator for Single‑Model Flows

Wrap your direct calls in an `Orchestrator` so you can use strategies, but still get single‑model behavior.

**Before:**

```csharp
ChatCompletionResponse? response =
    await client.CreateChatCompletionAsync(request, CancellationToken.None);
```

**After:**

```csharp
Orchestrator orchestrator = CreateDefaultOrchestrator(client);

var orchestrationRequest = new OrchestrationRequest
{
    UseEnsemble = false,
    Prompt = "Explain the theory of relativity.",
    OutputFormat = OutputFormat.PlainText,
    SingleModelRequest = new SingleModelDispatchRequest
    {
        Strategy = SingleModelStrategy.BestReasoning,
        RequiredCapabilities =
        [
            ModelCapability.Text,
            ModelCapability.Chat,
            ModelCapability.Reasoning
        ]
    }
};

IReadOnlyList<AiModelResponse> responses =
    await orchestrator.ProcessAsync(orchestrationRequest, CancellationToken.None);

AiModelResponse best = responses.Single();
Console.WriteLine(best.RawOutput);
```

Migration notes:

- You no longer specify a **concrete** model in the request; instead, you specify a **strategy**.
- Internally, the dispatcher selects an appropriate model from the registry.

### 3.4 Step 3 — Add Output Formats and Validation

You can start to rely on `OutputFormat` and the `FormatRegistry` to get deterministic shaping of responses.

**Before (unstructured text):**

```csharp
OutputFormat outputFormat = OutputFormat.PlainText;
```

**After (JSON, Markdown, SQL, etc.):**

```csharp
var request = new OrchestrationRequest
{
    UseEnsemble = false,
    Prompt = "Generate a list of planets as JSON.",
    OutputFormat = OutputFormat.Json,
    SingleModelRequest = new SingleModelDispatchRequest
    {
        Strategy = SingleModelStrategy.LowestCost,
        RequiredCapabilities =
        [
            ModelCapability.Text,
            ModelCapability.Chat
        ]
    }
};

IReadOnlyList<AiModelResponse> responses =
    await orchestrator.ProcessAsync(request, CancellationToken.None);

// The underlying FormatRegistry ensures:
// - A format-specific system prompt was used.
// - An IOutputFormatValidator validated that output is valid JSON.
Console.WriteLine(responses.Single().RawOutput);
```

Migration notes:

- You continue to see only a `string` payload (`RawOutput`), but the orchestrator has already nudged/validated format for you.
- You can add your own formats and validators if you need stronger guarantees.

### 3.5 Step 4 — Introduce Ensembles (Best‑of‑N, Model‑as‑Judge, Synthesis)

Once single‑model orchestration is in place, move to **ensembles**:

1. Change `UseEnsemble` to `true`.
2. Provide an `EnsembleDispatchRequest` instead of `SingleModelDispatchRequest`.
3. Use (or configure) a response handler that implements:
   - Model‑as‑judge (a judge model scores each candidate)
   - Heuristic ranking (e.g. cost vs quality)
   - Response synthesis (combine multiple candidates)

**Before (single model):**

```csharp
UseEnsemble = false,
SingleModelRequest = new SingleModelDispatchRequest
{
    Strategy = SingleModelStrategy.BestReasoning
}
```

**After (ensemble):**

```csharp
var ensembleRequest = new OrchestrationRequest
{
    UseEnsemble = true,
    Prompt = "Explain the pros and cons of microservices vs monoliths.",
    OutputFormat = OutputFormat.Markdown,
    EnsembleRequest = new EnsembleDispatchRequest
    {
        Strategy = EnsembleStrategy.Reasoning,
        // or EnsembleStrategy.Custom with RequiredCapabilities, etc.
    }
};

IReadOnlyList<AiModelResponse> responses =
    await orchestrator.ProcessAsync(ensembleRequest, CancellationToken.None);
```

Your configured `IAiModelResponseHandler` can:

- Treat some models as **primary candidates** and another as a **judge** model.
- Ask the judge to score each candidate.
- Use cost/latency/score heuristics to pick the winner or synthesize a combined answer.
- Return either:
  - A single `AiModelResponse` representing the winning/synthesized result, or
  - Multiple `AiModelResponse`s if you want to inspect them yourself.

### 3.6 Step 5 — Replace Ad‑Hoc Logic with Strategies & Registries

Wherever you have manual `if/else` logic like:

```csharp
OpenAIModel selectedModel;

if (needsReasoning)
    selectedModel = OpenAIModel.GPT4_1_Reasoning;
else if (isLowCost)
    selectedModel = OpenAIModel.GPT4o_Mini;
else
    selectedModel = OpenAIModel.GPT4o;
```

You can migrate to **strategy‑driven** selection:

```csharp
var request = new OrchestrationRequest
{
    UseEnsemble = false,
    Prompt       = userPrompt,
    OutputFormat = OutputFormat.PlainText,
    SingleModelRequest = new SingleModelDispatchRequest
    {
        Strategy = needsReasoning
            ? SingleModelStrategy.BestReasoning
            : SingleModelStrategy.LowestCost,
        RequiredCapabilities =
        [
            ModelCapability.Text,
            ModelCapability.Chat
        ]
    }
};
```

Benefits:

- Logic lives in **strategy handlers** backed by the registry, not scattered across your app.
- Adding new models becomes a registry/config change, not a set of `if` branches.

---

## Summary

- **Beginner**: use `ChatClient` + `ClientRequestBuilder` for straightforward chat completions (with or without streaming).
- **Advanced**: use the orchestration layer (`Orchestrator`, model registry, dispatchers, executors, response handlers) for:
  - Strategy‑driven model selection
  - Ensemble runs
  - Model‑as‑judge, heuristic decisions, response synthesis
  - Output format guidance and validation
- **Migration**: you can evolve incrementally:
  - Step 1: registry‑backed models
  - Step 2: single‑model orchestrator
  - Step 3: output formats & validators
  - Step 4: ensembles and judge/synthesis patterns
  - Step 5: strategy‑driven selection instead of ad‑hoc `if/else` logic

This keeps your codebase clean, extensible, and ready for future model families and capabilities.