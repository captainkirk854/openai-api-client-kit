# 🚀 ChatGPT C# Client & OpenAI Orchestration Framework

A lightweight, production‑ready C# wrapper and orchestration framework for accessing OpenAI’s ChatGPT and related models — no Azure deployment required.

This repo now includes two major layers:

- **Chat Client Layer** – a clean, resilient `ChatClient` for calling OpenAI’s chat completions (sync + streaming).
- **Orchestration Layer** – a strongly‑typed model registry plus strategy‑driven dispatchers and executors that can pick and run the “right” model(s) for a given task.

---

Supports:

- ✔️ Simple chat completions  
- ✔️ Streaming responses (token‑by‑token)  
- ✔️ Automatic retries  
- ✔️ Exponential backoff with jitter  
- ✔️ Smart handling of `HTTP 429 Rate Limit` + `Retry-After` header  
- ✔ Strongly‑typed **OpenAI Model Registry**  
- ✔️ Strategy‑driven **Single‑Model** & **Ensemble** dispatchers  
- ✔️ **Model‑as‑judge** evaluation flows  
- ✔️ **Decision‑by‑heuristic** model selection and ranking  
- ✔️ **Response synthesis** across multiple model outputs  
- ✔️ Pluggable **response handlers**  
- ✔️ Clean, reusable API surface  

---

# 📘 Overview

This project provides a robust, developer‑friendly C# client for interacting with OpenAI’s Chat Completion API, plus a full orchestration stack for model selection and execution.

It’s designed for:

- Backend services  
- Desktop apps  
- Tools and utilities  
- High‑reliability integrations  
- Model experimentation and cost/performance trade‑off analysis  

No Azure OpenAI deployment is required — this client talks directly to OpenAI’s public API.

---

# 🔧 Features

## 🧵 Streaming Support

Consume responses as they are generated using `IAsyncEnumerable<ChatCompletionChunk>`.

## 🔁 Retry Logic

Automatic retries on:

- Network failures  
- Transient errors  
- HTTP 429 (rate limit)  

## ⏳ Rate‑Limit Handling

Honors the server’s `Retry-After` header when present in the response.

## 🧱 Minimal Dependencies

Only uses:

- `System.Net.Http`  
- `System.Text.Json`  

---

# 📦 Installation

Add any required packages, for example:

```bash
dotnet add package System.Net.Http.Json
```

---

# 🔑 Getting an OpenAI API Key

1. Visit https://platform.openai.com  
2. Log in or create an account  
3. Go to **API Keys**  
4. Click **Create new secret key**  
5. Store it securely (never commit it to Git)  

Set it as an environment variable:

Windows (PowerShell):

```powershell
setx OPENAI_API_KEY "your-api-key"
```

> Note: In the console demo project, the environment variable is read as `OPENAI_API_KEY`.  
> Make sure this matches how you configure your environment.

---

# 🧠 ChatClient

Place the full wrapper class in `ChatClient.cs`.

This class includes:

- Normal completions  
- Streaming completions  
- Retry logic  
- Rate‑limit handling  
- `Retry-After` support  

It is suitable as a drop‑in client for typical backend or desktop usage.

---

# 🖥️ Usage Examples

## Common Setup

```csharp
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

ChatClient client = new(apiKey: apiKey);

ChatCompletionRequest request =
    new ClientRequestBuilder()
        .WithModel(input: OpenAIModels.GPT4o_Mini)
        .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
        .AddUserMessage(input: "Write a haiku about winter mornings.")
        .UsingMaxTokens(input: 1000)
        .EnableStreaming(input: isStreaming)
        .WithTemperature(input: temperature)
        .WithTopP(input: topP)                 // Validated to [0.0, 1.0]
        .WithPresencePenalty(input: presencePenalty)
        .WithFrequencyPenalty(input: frequencyPenalty)
        .Build();

using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
```

## Standard Response

```csharp
ChatCompletionResponse? response =
    await client.CreateChatCompletionAsync(request: request, cancelToken: cts.Token);

return response?.Choices[0].Message.Content;
```

## Streaming Response

```csharp
string? response = string.Empty;

// Stream the response chunk(s) ..
await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(
                   request: request,
                   cancelToken: cts.Token))
{
    // Extract delta content from chunk and concatenate to final response ..
    ChatDelta chunkDelta = chunk.Choices[0].Delta;
    response += chunkDelta.Content;
}

return response;
```

---

# 🛡️ Error Handling

The client automatically:

- Retries on transient network errors  
- Detects HTTP 429  
- Reads and honors `Retry-After`  
- Falls back to exponential backoff with jitter  

You can configure retry behavior:

```csharp
ChatClient client = new(apiKey: apiKey, maxRetries: 5, baseDelayMs: 1000);
```

---

# 🧠 Implementation Examples

See the `OpenAIApiClient.ConsoleApp` project for a collection of demos:

- `AiModelChatClientDemo` – basic `ChatClient` usage to send a prompt and receive a response.  
- `AIModelDispatchDemo` – uses the dispatchers to select models based on strategies  
  (e.g., best reasoning model, lowest cost model).  
- `AIModelBestResponseDemo` – uses the orchestrator to get a “best” response from a
  combination of low‑cost and fast‑inference models, including **model‑as‑judge** and  
  **response synthesis** patterns.  
- `AIModelOrchestratorDemo` – orchestrator usage with a custom request and response handler,
  including **heuristic‑driven decisions**.  
- `AiModelResponseHandlerDemo` – shows how to implement a custom response handler to
  format and inspect the output (including token counts and estimated costs).  

These demos are aligned with the orchestration/dispatching architecture introduced in this branch.

---

# 🧪 Testing Your Setup

## 1. Set your OpenAI API Key environment variable

```powershell
setx OPENAI_API_KEY "your-api-key"
```

## 2. Option 1: Use PowerShell to list available OpenAI models
```powershell
   Get-Models.ps1
```

## 3. Option 2: Use PowerShell to send a chat completion request (GPT‑5)
```powershell
   Invoke-GPT5.ps1
```

> Note: GPT‑5 access must be enabled for your account. You need a verified organization and billing at  
> https://platform.openai.com/settings/organization/general.  
> Otherwise, you’ll receive 403/Forbidden or similar errors.

### Extended PowerShell script with more options
```powershell
   Invoke-Model.ps1
```

## 4. Option 3: Run the console app

From the repo root:

```powershell
dotnet run --project .\OpenAIApiClient.ConsoleApp\OpenAIApiClient.ConsoleApp.csproj
```

If everything is configured correctly, you’ll see ChatGPT’s response in your console.

The console app includes demos for:

- Single‑model prompts  
- Ensemble execution  
- Custom response handling (including token + cost reporting)  
- Model‑as‑judge evaluation and response synthesis  

---

# 📘 OpenAI Model Registry & Orchestration Framework

This branch introduces a unified, strongly typed, future‑proof foundation for working with OpenAI models in .NET. It consolidates model metadata, capabilities, pricing, and dispatch information into a single, extensible architecture designed for reliability, maintainability, and clarity.

## 🚀 1. Strongly Typed Model Enumeration

A comprehensive `OpenAIModel` (or equivalent) enum covers major OpenAI model families:

- GPT‑5 (5.2, 5.2 Pro, 5, Mini, Nano)  
- GPT‑4.1 (Standard, Mini, Reasoning, Critic, Turbo)  
- GPT‑4o (Standard, Mini)  
- GPT‑3.5 Turbo  
- Embedding models  
- TTS / Whisper audio models  
- DALL·E 3  
- Open‑weight models (O1, O1‑Mini, etc.)  
- Moderation models  

This enum forms the **backbone** of the entire registry system.

## 🔗 2. Model Endpoint Mapping

There is a strongly typed mapping from each enum value to its actual OpenAI API endpoint string, ensuring:

- No magic strings  
- Centralized control  
- Easy updates when OpenAI releases new versions  

Example:

```csharp
OpenAIModel.GPT4_1 => "gpt-4.1";
```

## 🧠 3. Capability System

A flexible capability enum (e.g., `ModelCapability` / `AiModelCapability`) represents what each model can do:

- Text / Chat  
- Reasoning  
- Vision  
- Audio In / Out  
- Embedding  
- Image Generation  
- Moderation  
- Low‑Cost  
- High‑Performance  
- Open‑Weight  

This enables intelligent routing and orchestration decisions.

## 🧩 4. Model Descriptor

A unified descriptor object encapsulates:

- Model enum  
- API endpoint  
- Capabilities  
- Pricing (per‑token)  

This is the single source of truth for all model metadata.

## 💰 5. Pricing Model (Per‑Token)

Pricing is expressed as cost **per token**, not per‑1K tokens, which simplifies and clarifies cost calculations.

Pricing fields include:

- Input token cost  
- Output token cost  
- Cached input token cost  
- Reasoning token cost  
- Tool‑use token cost  

Values are defined in one place and can be updated whenever OpenAI’s pricing changes:

- https://openai.com/api/pricing/
- https://platform.openai.com/docs/pricing
- https://pricepertoken.com/pricing-page/provider/openai
- https://api.openaipricing.com/openai/text_tokens (Restful API Source - not working currently)
- https://openaipricing.com/pages/openai-platform-api-pricing/ (examples for the API above - not working currently)
- https://zapier.com/blog/openai-models/ (for pricing and good capabilities reference)
- https://learn.microsoft.com/en-us/azure/foundry/foundry-models/how-to/model-choice-guide (capabilities reference for Azure OpenAI models, which often align with OpenAI’s public models)
- https://azure.microsoft.com/en-gb/pricing/details/azure-openai/ (pricing reference for Azure OpenAI models with capabilities information)


## 🗂️ 6. Combined Model Registry

A fully integrated registry like:

```csharp
Dictionary<OpenAIModel, AiModelDescriptor> ModelRegistry;
```

Each entry includes:

- The model enum  
- The API endpoint  
- Its capabilities  
- Its pricing  

The registry is:

- Immutable  
- Strongly typed  
- Extensible  
- Centralized  

It serves as the authoritative source for all model‑related metadata.

## 🧩 7. Combined Prompt and Format Registry

The framework also defines an output format system with:

- Output formats (`PlainText`, `Json`, `Csv`, `Xml`, `Markdown`, `Yaml`, `Html`, `Sql`, …)  
- System prompts required for each format  
- Format‑specific validators to ensure responses conform to the expected shape  
- A unified `FormatDescriptor` that combines prompt and validator:

```csharp
public interface IOutputFormatValidator
{
    bool IsValidFormat(string content, out string? error);
}

public sealed record FormatDescriptor(
    string SystemPrompt,
    IOutputFormatValidator Validator
);
```

These descriptors live in an immutable registry:

```csharp
public static readonly Dictionary<OutputFormat, FormatDescriptor> FormatRegistry =
    new()
    {
        { OutputFormat.PlainText, new FormatDescriptor( ... ) },
        { OutputFormat.Json,      new FormatDescriptor( ... ) },
        { OutputFormat.Csv,       new FormatDescriptor( ... ) },
        { OutputFormat.Xml,       new FormatDescriptor( ... ) },
        { OutputFormat.Markdown,  new FormatDescriptor( ... ) },
        { OutputFormat.Yaml,      new FormatDescriptor( ... ) },
        { OutputFormat.Html,      new FormatDescriptor( ... ) },
        { OutputFormat.Sql,       new FormatDescriptor( ... ) },
    };
```

Validators such as `PlainTextValidator`, `JsonValidator`, `CsvValidator`, `XmlValidator`, `MarkdownValidator`, `YamlValidator`, `HtmlValidator`, and `SqlValidator` enforce output correctness.

---

# 🧭 Architectural Benefits

This design provides:

✔ **Single source of truth**  
- All model metadata lives in one place.

✔ **Zero duplication**  
- Endpoints, capabilities, and pricing are defined once.

✔ **Strong typing**  
- No stringly‑typed model names or ad‑hoc metadata.

✔ **Future‑proofing**  
- Adding GPT‑6 or new audio/vision models is trivial.

✔ **Orchestration‑ready**  
- The system can route based on:
  - Reasoning ability  
  - Cost tier  
  - Vision support  
  - Audio support  
  - Performance tier  

✔ **Pricing‑ready**  
- Cost estimation is a simple lookup + multiplication.

✔ **Judge‑/Heuristic‑/Synthesis‑ready**  
- Supports model‑as‑judge flows, heuristic‑based decision logic, and synthesized responses across multiple models.

---

# 🏗️ Architecture Overview

This project implements a **strategy‑driven orchestration framework** for selecting and executing OpenAI models. The design emphasizes extensibility, determinism, and clean separation of concerns.

At a high level, there are three layers:

- **Dispatching Layer** — selects the appropriate model(s)  
- **Orchestration Layer** — builds execution context and prompt context  
- **Execution Layer** — runs the selected model(s) and aggregates responses  

The architecture is fully declarative: adding new models or strategies does **not** require changes to the orchestrator or dispatchers.

## 🔍 Dispatching Layer

The dispatching layer decides which OpenAI model(s) to use for a given request.

It exposes two symmetrical components:

### Single Model Dispatcher

Evaluates a `SingleAiModelDispatchRequest` and selects **one** model using a strategy.

Responsibilities:

- Consult the **Single Model Strategy Registry**  
- Apply the chosen strategy handler  
- Return a single model descriptor  

Typical strategies:

- Best reasoning model  
- Lowest cost model  
- Vision‑capable model  
- Explicit model selection  

### Ensemble Dispatcher

Evaluates an `EnsembleDispatcherRequest` and selects **multiple** models.

Responsibilities:

- Consult the **Ensemble Strategy Registry**  
- Apply the chosen ensemble strategy handler  
- Return a list of model descriptors  

Typical strategies:

- Reasoning ensemble  
- Cost‑optimized ensemble  
- Vision ensemble  
- Weighted ensemble  
- Explicit model list  

## 🧠 Strategy Registries

Both dispatchers rely on registries that map:

```text
Strategy → StrategyHandler
```

A strategy handler is a pure function:

```csharp
(registry, request) => ModelDescriptor
(registry, request) => List<ModelDescriptor>
```

This allows new strategies to be added without modifying dispatcher code.

Benefits:

- Declarative and extensible  
- Zero duplication  
- Deterministic model selection  
- Easy to test and mock  

The current branch includes:

- A complete set of built‑in strategies for single and ensemble dispatch  
- **Fixes for custom strategy handlers** in both `SingleModelStrategies` and `EnsembleStrategies`  
- **Improved unit tests** for dispatcher behavior and strategy registration  

---

## 🧠 Model‑as‑Judge, Heuristic Decisions & Response Synthesis

The orchestration layer supports richer flows on top of basic dispatch:

- **Model‑as‑Judge** – one or more “judge” models evaluate candidate responses from other models
  and score/rank them based on quality, safety, or task‑specific criteria.

- **Decision by Heuristic** – orchestration can select the “winning” model or response using
  pluggable heuristics (e.g., cost, token usage, judge score, length constraints, or domain‑specific rules).

- **Response Synthesis** – multiple candidate outputs can be combined into a single synthesized answer,
  optionally with a synthesis pass handled by a separate model (or the same model with a different prompt).

These behaviors are implemented via:

- Ensemble strategies that return multiple candidate models  
- Executors that run those models in parallel  
- Response handlers that:
  - Inspect all `AiModelResponse` instances  
  - Apply judge prompts and/or heuristics  
  - Produce a single final, synthesized output  

The `AIModelBestResponseDemo` and `AIModelOrchestratorDemo` in the console app provide end‑to‑end usage examples for these patterns.

---

## 🔧 Orchestration Layer

The Orchestrator coordinates the full pipeline:

- Determines whether the request should be **single‑model** or **ensemble**  
- Calls the appropriate dispatcher’s `Evaluate` method  
- Wraps the selected model(s) in an **ExecutionContext**  
- Builds a **PromptContext** containing a `ChatCompletionRequest`  
- Passes everything to the execution layer  

The layer is intentionally thin: it delegates decisions to dispatchers and all actual calls to executors.

## ⚙️ Execution Layer

The execution layer runs the selected model(s):

- `ISingleAiModelExecutor` – single model execution  
- `IEnsembleExecutor` – ensemble execution (parallel models + aggregation)  

Ensemble execution runs models in parallel, then aggregates the results.

All responses flow through an `IResponseHandler` implementation, which can:

- Inspect raw outputs  
- Check token usage  
- Calculate estimated cost  
- Apply judge prompts and/or heuristics  
- Generate a synthesized final answer  

The `ResponseHandlerDemo` in the console app shows how to:

- Log each model’s status  
- Display token counts and estimated costs  
- Print failures vs. successes  
- Return the final list of responses (or a synthesized response).

## 🎯 Key Architectural Advantages

- **Strategy‑driven** – all model selection logic is declarative and pluggable.  
- **Symmetrical design** – single and ensemble paths share the same structure.  
- **Extensible** – new strategies or models can be added without modifying core classes.  
- **Deterministic** – same request + registry → same selection.  
- **Testable** – dispatchers, executors, and handlers are fully mockable; the branch includes expanded unit tests for dispatch and custom strategies.  
- **Future‑proof** – supports new model generations, capabilities, and pricing with minimal changes.  
- **Judge/Heuristic/Synthesis‑aware** – supports advanced patterns like model‑as‑judge, heuristic
  decision‑making, and response synthesis out of the box.

---

# 🚀 Strategy‑Driven Model Dispatching

The **Single Model Dispatcher** and **Ensemble Dispatcher** form the heart of the orchestration system.

### Core Concepts

1. **Dispatchers**

   - Evaluate a request  
   - Select appropriate model(s)  
   - Do *not* execute models themselves  
   - Use strategy handlers registered in the strategy registries  

2. **Strategy Registries**

   Strategies are registered as pure functions that receive:

   - The model registry  
   - The dispatcher request  

   and return:

   - A single model descriptor, or  
   - A list of model descriptors  

   Example pattern (pseudo‑code):

   ```csharp
   SingleAiModelStrategies.Register(
       AiModelDispatchingStrategy.BestReasoning,
       (registry, request) =>
           registry.Values
               .Where(m => m.Capabilities.Contains(AiModelCapability.Reasoning))
               .OrderByDescending(m => m.Generation)
               .First());
   ```

   In this branch, custom strategy handlers for both single and ensemble strategies have been corrected and verified with unit tests.

---

# 📊 Orchestration Flowchart

```text
+--------------------------------------------------------------------+
|                        OrchestrationRequest                        |
+--------------------------------------------------------------------+
                                |
                                v
                       +-----------------------+
                       |     UseEnsemble?      |
                       +-----------------------+
                         |                 |
                        No                Yes
                         |                 |
                         v                 v
+----------------------------------+   +--------------------------------+
| SingleAiModelDispatcher.Evaluate |   | EnsembleDispatcher.Evaluate    |
+----------------------------------+   +--------------------------------+
                         |                 |
                         v                 v
+-------------------------------+      +--------------------------------+
| SingleAiModelExecutionContext |      |  EnsembleExecutionContext      |
+-------------------------------+      +--------------------------------+
                         |                 |
                         v                 v

====================================================================
||                        ORCHESTRATION LAYER                      ||
||                                                                ||
||   +--------------------------------------------------------+   ||
||   |                OrchestrationContext                    |   ||
||   |                + PromptContext                         |   ||
||   +--------------------------------------------------------+   ||
====================================================================

                         |                 |
                         v                 v
+--------------------------------+   +--------------------------------+
|   ISingleAiModelExecutor.Exec  |   |   IEnsembleExecutor.Exec       |
+--------------------------------+   +--------------------------------+
                         |                 |
                         v                 v
+--------------------------------+   +--------------------------------+
|       AiModelResponse          |   |      List<AiModelResponse>     |
+--------------------------------+   +--------------------------------+
                         \                 /
                          \               /
                           v             v
              +-----------------------------------------+
              | IAiModelResponseHandler.HandleResponses |
              +-----------------------------------------+
                                 |
                                 v
                     +------------------------------+
                     |          Final Output        |
                     +------------------------------+
```

---

# Fluent Orchestrator Builder

The **Fluent Orchestrator Builder** provides a clean and expressive way to construct an AI orchestration pipeline.

It offers:

- A fluent configuration surface  
- Modular sub‑factories for registries, dispatchers, executors, and request builders  
- A default‑with‑override pattern so callers can use sensible defaults or plug in their own components  

Example (maximum customization):

```csharp
var orchestrator = new OrchestratorBuilder()
    .WithClient(client)
    .WithResponseHandler(handler)
    .WithModelRegistry(customRegistry)
    .WithRequestBuilderFactory(() => new ChatClientRequestBuilder().WithDefaults())
    .WithSingleModelDispatcher(customSingleModelDispatcher)
    .WithSingleModelExecutor(customSingleModelExecutor)
    .WithEnsembleDispatcher(customEnsembleDispatcher)
    .WithEnsembleExecutor(customEnsembleExecutor)
    .Build();
```

Minimal configuration:

```csharp
var orchestrator = new OrchestratorBuilder()
    .WithClient(client)
    .WithResponseHandler(handler)
    .Build();
```

This builder is ideal when you want:

- A single, expressive entry point  
- Multiple orchestration “profiles”  
- Test isolation through custom components  

---

# 📄 License

MIT License — free to use, modify, and distribute.