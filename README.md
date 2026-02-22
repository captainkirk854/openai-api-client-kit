# 🚀 ChatGPT C# Client

A lightweight, production‑ready C# wrapper for accessing OpenAI’s ChatGPT models — no Azure deployment required.

Supports:
- ✔️ Simple chat completions
- ✔️ Streaming responses (token‑by‑token)
- ✔️ Automatic retries
- ✔️ Exponential backoff with jitter
- ✔️ Smart handling of ```HTTP 429 Rate Limit``` + ```Retry-After``` header
- ✔️ Clean, reusable API surface

# 📘 Overview

This project provides a robust, developer‑friendly C# client for interacting with OpenAI’s Chat Completion API.

It’s designed for:
- Backend services
- Desktop apps
- Tools and utilities
- High‑reliability integrations

No Azure OpenAI deployment is required — this client talks directly to OpenAI’s public API.

# 🔧 Features

## 🧵 Streaming Support

Consume responses as they are generated using ```IAsyncEnumerable<ChatCompletionChunk>```.

## 🔁 Retry Logic

Automatic retries on:
- Network failures
- Transient errors
- HTTP 429 (rate limit)

## ⏳ Rate‑Limit Handling
Honors the server’s ```Retry-After``` header when present in response.

## 🧱 Minimal Dependencies

Only uses ```System.Net.Http``` and ```System.Text.Json```.

# 📦 Installation

Add the required package(s) if required:
e.g., ```dotnet add package System.Net.Http.Json```

# 🔑 Getting an OpenAI API Key
- Visit https://platform.openai.com
- Log in or create an account
- Go to API Keys
- Click Create new secret key
- Store it securely (never commit it to Git)

Set it as an environment variable:
Windows (PowerShell):
```setx OPENAI_API_KEY "your-api-key"```

# 🧠 ChatClient
Place the full wrapper class in: ```ChatClient.cs```

This class includes:
- Normal completions
- Streaming completions
- Retry logic
- Rate‑limit handling
- Retry-After support

# 🖥️ Usage Examples

## Common Setup
```
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

ChatClient client = new(apiKey: apiKey);

ChatCompletionRequest request = 
new ClientRequestBuilder().WithModel(input: OpenAIModels.GPT4o_Mini)
                          .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                          .AddUserMessage(input: "Write a haiku about winter mornings.")
                          .UsingMaxTokens(input: 1000)
                          .EnableStreaming(input: isStreaming)
                          .WithTemperature(input: temperature)
                          .WithTopP(input: topP)
                          .WithPresencePenalty(input: presencePenalty)
                          .WithFrequencyPenalty(input: frequencyPenalty)
                          .Build();

using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));
```

## Standard Response
```
ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cts.Token);
return response?.Choices[0].Message.Content;
```

## Streaming Response
```
string? response = string.Empty;

// Stream the response chunk(s) ..
await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cts.Token))
{
    // Extract delta content from chunk and concatenate to final response ..
    ChatDelta chunkDelta = chunk.Choices[0].Delta;
    response += chunkDelta.Content;
}

return response;
```

# 🛡️ Error Handling

The client automatically:
- Retries on transient network errors
- Detects HTTP 429
- Reads and honors Retry-After
- Falls back to exponential backoff with jitter

You can configure retry behavior:
```ChatClient client = new(apiKey: apiKey, maxRetries: 5, baseDelayMs: 1000);```

# 🧠 Implementation Examples
See the `OpenAIApiClient.ConsoleApp` project for a simple console application demonstrating usage.
- `AiModelChatClientDemo` - demonstrates basic implementation of the ChatClient to send a prompt and receive a response.
- `AIModelDispatchDemo` - demonstrates using the dispatchers to select models based on strategies (e.g., best reasoning model, lowest cost model).)
- `AIModelBestResponseDemo` - demonstrates using the orchestrator to get the "best" response from lowest-cost and fast-inferencing model(s).
- `AIModelOrchestratorDemo` - demonstrates using the orchestrator with a custom request and response handler.
- `AiModelResponseHandlerDemo` - demonstrates implementing a custom response handler to format the output.`

# 🧪 Testing Your Setup

## Set your OpenAI API Key environment variable
```powershell
setx OPENAI__API_KEY "your-api-key"
```

## Run:
### Option 1: Using PowerShell to list available OpenAI models

#### Simple PowerShell script to list available OpenAI models associated with your API key
```powershell
function Get-OpenAIModels {
    param(
        [string]$ApiKey = $env:OPENAI_API_KEY
    )

    Invoke-RestMethod `
        -Uri "https://api.openai.com/v1/models" `
        -Headers @{ "Authorization" = "Bearer $ApiKey" } `
        -Method Get
}

(Get-OpenAIModels).data | Sort-Object -Property id
```

### Option 2: Using PowerShell to send a chat completion request using GPT-5

Note: OpenAI requires GPT-5 access to be enabled on your account which means verifying 
your organization and billing details at https://platform.openai.com/settings/organization/general.
Lack of access will result in a 403 Forbidden response or similar.

#### Simple PowerShell script to send a chat completion request to GPT-5
```powershell
function Invoke-GPT5Prompt {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Prompt,

        [string]$ApiKey = $env:OPENAI_API_KEY,

        # Optional: adjust temperature if desired
        [double]$Temperature = 1.0 # GPT-5 only supports 1.0
    )

    if (-not $ApiKey) {
        throw "No API key provided. Set OPENAI_API_KEY or pass -ApiKey."
    }

    $url = "https://api.openai.com/v1/chat/completions"

    $headers = @{
        "Authorization" = "Bearer $ApiKey"
        "Content-Type"  = "application/json"
    }

    $body = @{
        model = "gpt-5"
        messages = @(
            @{
                role    = "user"
                content = $Prompt
            }
        )
        temperature = $Temperature
    } | ConvertTo-Json -Depth 5

    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $body

    return $response.choices[0].message.content
}
# Example usage
$response = Invoke-GPT5Prompt -Prompt "List the planets and their diameters in order of size"
$response
```

#### Extended PowerShell script to send a chat completion request to GPT-5 with more options
```powershell

function Invoke-GPT5Prompt {
    [CmdletBinding()]
    param(
        # Required user prompt
        [Parameter(Mandatory = $true)]
        [string]$Prompt,

        # Optional system prompt
        [string]$SystemPrompt,

        # API key (defaults to environment variable)
        [string]$ApiKey = $env:OPENAI_API_KEY,

        # Optional OpenAI parameters
        [double]$Temperature = 1.0,
        [double]$TopP = 1.0,
        [int]$MaxTokens,
        [double]$PresencePenalty = 0.0,
        [double]$FrequencyPenalty = 0.0,
        [string[]]$Stop,
        [string]$User,
        [int]$Seed,
        [int]$N = 1,

        # Optional: JSON object for logit bias
        [hashtable]$LogitBias,

        # Optional: response format (e.g., "json_object")
        [string]$ResponseFormat
    )

    if (-not $ApiKey) {
        throw "No API key provided. Set OPENAI_API_KEY or pass -ApiKey."
    }

    $url = "https://api.openai.com/v1/chat/completions"

    $headers = @{
        "Authorization" = "Bearer $ApiKey"
        "Content-Type"  = "application/json"
    }

    # Build messages array
    $messages = @()

    if ($SystemPrompt) {
        $messages += @{
            role    = "system"
            content = $SystemPrompt
        }
    }

    $messages += @{
        role    = "user"
        content = $Prompt
    }

    # Build request body
    $body = @{
        model = "gpt-5"
        messages = $messages
        temperature = $Temperature # GPT-5 only supports 1.0
        top_p = $TopP # Not supported by GPT-5 but included for completeness
        n = $N
        presence_penalty = $PresencePenalty
        frequency_penalty = $FrequencyPenalty
    }

    if ($MaxTokens)      { $body.max_completion_tokens = $MaxTokens }
    if ($Stop)           { $body.stop = $Stop }
    if ($User)           { $body.user = $User }
    if ($Seed)           { $body.seed = $Seed }
    if ($LogitBias)      { $body.logit_bias = $LogitBias } # Not supported by GPT-5 but included for completeness
    if ($ResponseFormat) { $body.response_format = @{ type = $ResponseFormat } }

    $json = $body | ConvertTo-Json -Depth 10

    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $json

    # Return the first completion
    return $response.choices[0].message.content
}

# Example usage

# Basic
$response = Invoke-GPT5Prompt -Prompt "Explain the theory of relativity in simple terms."
$response

# With system prompt and additional parameters
$response = Invoke-GPT5Prompt -Prompt "Summarize the key points of the article." `
                              -SystemPrompt "You are a concise summarizer." `
                              -MaxTokens 150 `
                              -Temperature 0.7 `
                              -TopP 0.9
$response

# With logit bias and response format
$response = Invoke-GPT5Prompt -Prompt "Generate a JSON object with user details." `
                              -LogitBias @{ "50256" = -100 } ` # Example token bias
                              -ResponseFormat "json_object"
```


### Option 3: Use the simple console app provided in the ```OpenAIApiClient.ConsoleApp``` project.

```powershell
dotnet run --project .\OpenAIApiClient.ConsoleApp\OpenAIApiClient.ConsoleApp.csproj
```

*(Tip: Remember to set your OPENAI_API_KEY environment variable first (see above))*

If everything is configured correctly, you’ll see ChatGPT’s response in your console.


# 📘 OpenAI Model Registry & Orchestration Framework
This project establishes a unified, strongly typed, future‑proof foundation for working with OpenAI models in .NET. The work completed in this discussion consolidates model metadata, capabilities, pricing, and routing information into a single, extensible architecture designed for reliability, maintainability, and clarity.

## 🚀 1. Strongly Typed Model Enumeration
A comprehensive ```OpenAIModel``` enum has been defined covering all relevant OpenAI model families:
- GPT‑5 (5.2, 5.2 Pro, 5, Mini, Nano)
- GPT‑4.1 (Standard, Mini, Reasoning, Critic, Turbo)
- GPT‑4o (Standard, Mini)
- GPT‑3.5 Turbo
- Embedding models
- TTS / Whisper audio models
- DALL·E 3
- Open‑weight models (O1, O1‑Mini)
- Moderation models

This enum forms the **backbone** of the entire registry system.

## 🔗 2. Model Endpoint Mapping
There is a strongly typed mapping from each enum value to its actual OpenAI API endpoint string, ensuring:
- No magic strings
- Centralized control
- Easy updates when OpenAI releases new versions

Example:
OpenAIModel.GPT4_1 → "gpt-4.1"

## 🧠 3. Capability System
A flexible ```AiModelCapability``` enum has been defined to represent what each model can do:
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

This enables intelligent routing, filtering, and orchestration decisions.

## 🧩 4. ModelDescriptor Class
A unified descriptor object encapsulates:
- Model enum
- API endpoint
- Capabilities
- Pricing (per‑token)

This becomes the single source of truth for all model metadata.

## 💰 5. Pricing Model (Per‑Token)
The ModelPricing class uses cost per token, not per‑1K, making cost calculations simpler and more accurate.

Pricing fields include:
- Input token cost
- Output token cost
- Cached input token cost
- Reasoning token cost
- Tool‑use token cost

All values were left as placeholders to populate with real pricing when necessary.

## 🗂️ 6. Combined Model Registry
We end up with a fully integrated registry:
```Dictionary<OpenAIModel, AiModelDescriptor>```

Each entry includes:
- The model’s enum
- The API endpoint
- Its capabilities
- Its pricing

This registry is:
- Immutable
- Strongly typed
- Extensible
- Centralized
- Perfect for orchestration, routing, and cost estimation

It is the authoritative source for all model-related metadata in this system.


## 🧩 7. Combined Prompt and Format Registry

We have a complete, extendable framework that unifies:

- **Output formats** (TXT, JSON, CSV, XML, Markdown, YAML, HTML, SQL)
- **System prompts** required for each format
- **Format‑specific validators** to ensure compliance
- **A unified `FormatDescriptor` registry** to hold it all together

The result is a clean, deterministic, strongly‑typed architecture with zero duplication and a single source of truth.

### 🧪 Format Validators

Validators have been implemented for the following format:

- **PlainTextValidator**  
- **JsonValidator**  
- **CsvValidator**  
- **XmlValidator**  
- **MarkdownValidator**  
- **YamlValidator**  
- **HtmlValidator**  
- **SqlValidator** (dialect‑agnostic)

Each validator implements a shared interface:

```csharp
public interface IOutputFormatValidator
{
    bool IsValidFormat(string content, out string? error);
}
```
Prompts and validators are merged into a single atomic unit:

```csharp
public sealed record FormatDescriptor(
    string SystemPrompt,
    IOutputFormatValidator Validator
);
```

These are then stored in a strongly typed, immutable registry as a dictionary eliminating duplication and creating
a single source of truth for all formatting metadata:

```csharp
public static readonly Dictionary<OutputFormat, FormatDescriptor> FormatRegistry = 
    new()
    {
        { OutputFormat.PlainText, new FormatDescriptor( ... ) },
        { OutputFormat.Json, new FormatDescriptor( ... ) },
        { OutputFormat.Csv, new FormatDescriptor( ... ) },
        { OutputFormat.Xml, new FormatDescriptor( ... ) },
        { OutputFormat.Markdown, new FormatDescriptor( ... ) },
        { OutputFormat.Yaml, new FormatDescriptor( ... ) },
        { OutputFormat.Html, new FormatDescriptor( ... ) },
        { OutputFormat.Sql, new FormatDescriptor( ... ) },
    };
```


# 🧭 Architectural Benefits
This design provides:

✔ Single‑source‑of‑truth
- All model metadata lives in one place.

✔ Zero duplication
- Endpoints, capabilities, and pricing are defined once.

✔ Strong typing everywhere
- No stringly‑typed model names or ad‑hoc metadata.

✔ Future‑proofing
- Adding GPT‑6 or new TTS models is trivial.

✔ Orchestration‑ready
- The system can route based on:
    - Reasoning ability
    - Cost tier
    - Vision support
    - Audio support
    - Performance tier

✔ Pricing‑ready
Cost estimation becomes a simple lookup + multiplication.


# 🏗️ Architecture Overview
This project implements a strategy‑driven orchestration framework for selecting and executing OpenAI models. The design emphasizes extensibility, determinism, and clean separation of concerns. At its core, the system is built around three major layers:
- Dispatching Layer — selects the appropriate model(s)
- Orchestration Layer — builds execution context and prompt context
- Execution Layer — runs the selected model(s) and aggregates responses

- The architecture is fully declarative: adding new models or strategies requires no changes to the orchestrator or dispatchers.

## 🔍 Dispatching Layer
The dispatching layer is responsible for deciding which OpenAI model(s) to use for a given request.
It contains two symmetrical components:

### Single Model Dispatcher
Evaluates a ModelDispatcherRequest and selects one model using a strategy.

Responsibilities
- Consult the Single Model Strategy Registry
- Apply the selected strategy handler
- Return a single ModelDescriptor

Example Strategies
- Best reasoning model
- Lowest cost model
- Vision‑capable model
- Explicit model selection

### Ensemble Dispatcher
Evaluates an EnsembleDispatcherRequest and selects multiple models.

Responsibilities
- Consult the Ensemble Strategy Registry
- Apply the selected ensemble strategy handler
- Return a list of ModelDescriptor objects

Example Strategies
- Reasoning ensemble
- Cost‑optimized ensemble
- Vision ensemble
- Weighted ensemble
- Explicit model list

## 🧠 Strategy Registries
Both dispatchers rely on strategy registries, which map:

```
Strategy → StrategyHandler
```

A strategy handler is a pure function:
```
(registry, request) → AiModelDescriptor
(registry, request) → List<AiModelDescriptor>
```
This design allows new strategies to be added without modifying dispatcher code.

Benefits
- Declarative and extensible
- Zero duplication
- Deterministic model selection
- Easy to test and mock

## 🔧 Orchestration Layer
The Orchestrator coordinates the entire flow:
- Determines whether the request is single‑model or ensemble
- Calls the appropriate dispatcher’s Evaluate method
- Wraps the selected model(s) in an ExecutionContext
- Builds a PromptContext containing a ChatCompletionRequest
- Passes everything to the execution layer

This layer is intentionally thin — it delegates all decision‑making to dispatchers and all execution to executors.

## ⚙️ Execution Layer
The execution layer runs the selected model(s):
- `ISingleAiModelExecutor`
- `IEnsembleExecutor`

Single‑model execution is straightforward.
Ensemble execution runs models in parallel, then aggregates results.

All responses flow through an IResponseHandler, which formats or merges them into a final output

## 🎯 Key Architectural Advantages
- Strategy‑driven: All model selection logic is declarative and pluggable
- Symmetrical design: Single and ensemble paths share the same structure
- Extensible: Add new strategies or models without modifying core classes
- Deterministic: Same request + registry → same model selection
- Testable: Dispatchers, executors, and handlers are fully mockable
- Future‑proof: Supports new model generations, capabilities, and pricing


 
# 🚀 Strategy‑Driven Model Dispatching

## The Single‑Model and Ensemble Dispatchers for OpenAI Model Selection
This architecture has a robust, extensible, and deterministic model dispatching architecture. The system revolves around two key components:
- Single Ai Model Dispatcher
- Ensemble Dispatcher

- Both dispatchers use strategy registries to evaluate incoming requests and select the most appropriate OpenAI model(s) for execution.
The result is a clean, declarative, and future‑proof orchestration pipeline.

### 🧩 Core Architectural Concepts
1. Dispatchers

    Dispatchers have these responsibilities:
    - They evaluate a request
    - They select the appropriate model(s)
    - They do not execute the models
    - They rely on strategy handlers to make decisions

2. Strategy Registries

    Both dispatchers use a registry of strategies, where each strategy is a function that:
    - Receives the model registry
    - Receives the dispatcher request
    - Returns:
        - A single `AiModelDescriptor`, or
        - A list of `AiModelDescriptor` objects
    
        - This enables a wide range of selection behaviors without modifying dispatcher code.

#### Example Strategy Pattern
```csharp
SingleAiModelStrategies.Register(
    AiModelDispatchingStrategy.BestReasoning,
    (registry, request) =>
        registry.Values
            .Where(m => m.Capabilities.Contains(AiModelCapability.Reasoning))
            .OrderByDescending(m => m.Generation)
            .First());
```


The dispatcher simply calls the registered handler.

### 🧠 Single Model Dispatcher

Purpose
- Select one OpenAI model based on a strategy.

Interface:

    public interface ISingleAiModelDispatcher
    {
        SingleAiModelDispatchResult Evaluate(SingleAiModelDispatchRequest request);
    }

Implementation Highlights
- Accepts a `SingleAiModelDispatchRequest`
- Looks up the correct strategy handler from the registry
- Returns a `SingleAiModelDispatchResult` containing the chosen `AiModelDescriptor`

Benefits
- Clean separation of concerns
- Deterministic model selection
- Easy to mock in unit tests
- Extensible without modifying dispatcher code

### 🧠 Ensemble Dispatcher

Purpose
- Select multiple OpenAI models for ensemble execution.

Interface:

    public interface IEnsembleDispatcher
    {
        EnsembleDispatcherResult Evaluate(EnsembleDispatcherRequest request);
    }

Implementation Highlights
- Accepts an EnsembleDispatcherRequest
- Uses the EnsembleDispatchingStrategyRegistry
- Returns a list of ModelDescriptor objects

Supported Ensemble Strategies
- Reasoning ensembles
- Vision ensembles
- Cost‑optimized ensembles
- Explicit model lists
- Weighted ensembles
- Capability‑filtered ensembles
Benefits
- Parallel model execution
- Flexible ensemble composition
- Strategy‑driven, not hard‑coded
- Perfect symmetry with the single‑model dispatcher

## 🧱 Orchestrator Integration

The Orchestrator coordinates the entire flow:
- Determines whether the request is single‑model or ensemble
- Calls the appropriate dispatcher’s Evaluate method
- Wraps the result in an ExecutionContext
- Builds a PromptContext containing a ChatCompletionRequest
- Executes via:
    - `ISingleAiModelExecutor`
    - `IEnsembleExecutor`
- Passes results to an `IAiModelResponseHandler`

This creates a clean pipeline:
Dispatcher → ExecutionContext → OrchestrationContext → Executor → ResponseHandler

🧪 Unit Testing Enhancements
 The test suite cotains:
- Mocks dispatchers
- Mocks executors
- Verifies correct dispatcher selection
- Ensures correct model selection
- Confirms correct response handling
- Uses reflection to construct ModelDescriptor safely

This ensures the system is:
- Deterministic
- Predictable
- Extensible
- Fully testable

# 🎯 Summary

We have an extensible orchestration framework for OpenAI models, centered around:
- Single Model Dispatcher
- Ensemble Dispatcher
- Strategy registries
- Execution contexts
- PromptContext with embedded ChatCompletionRequest
- Clean orchestration pipeline
- Comprehensive unit tests

The dispatchers now serve as the decision‑making layer, selecting the best model(s) based on declarative strategies, while the orchestrator and executors handle execution and response processing.
The result is a system that is:
- Declarative
- Strategy‑driven
- Extensible
- Testable
- Architecturally symmetrical
- Ready for production and future expansion


# 📊 Orchestration Flowchart

```
+--------------------------------------------------------------------+
|                        OrchestrationRequest                        |
+--------------------------------------------------------------------+
                                |
                                v
                       +----------------------+
                       |     UseEnsemble?     |
                       +----------------------+
                         |                |
                        No               Yes
                         |                |
                         v                v
+----------------------------------+   +--------------------------------+
| SingleAiModelDispatcher.Evaluate |   | EnsembleDispatcher.Evaluate    |
+----------------------------------+   +--------------------------------+
                         |                |
                         v                v
+-------------------------------+      +--------------------------------+
| SingleAiModelExecutionContext |      |  EnsembleExecutionContext      |
+-------------------------------+      +--------------------------------+
                         |                |
                         v                v

====================================================================
||                        ORCHESTRATION LAYER                     ||
||                                                                ||
||   +--------------------------------------------------------+   ||
||   |                OrchestrationContext                    |   ||
||   |                + PromptContext                         |   ||
||   +--------------------------------------------------------+   ||
====================================================================

                         |                |
                         v                v
+--------------------------------+   +--------------------------------+
|   ISingleAiModelExecutor.Exec  |   |   IEnsembleExecutor.Exec       |
+--------------------------------+   +--------------------------------+
                         |                |
                         v                v
+--------------------------------+   +--------------------------------+
|         AiModelResponse        |   |      List<AiModelResponse>     |
+--------------------------------+   +--------------------------------+
                         \                /
                          \              /
                           v            v
                 +---------------------------------+
                 | IAiModelResponseHandler.Respond |
                 +---------------------------------+
                                 |
                                 v
                  +------------------------------+
                  |          Final Output        |
                  +------------------------------+
```


# Fluent Orchestrator Builder
The Fluent Orchestrator Builder provides a clean, expressive, and extensible way to construct an AI orchestration pipeline. It combines a fluent configuration API with modular sub‑factories to keep the system flexible, testable, and easy to reason about.

## Purpose
The builder exists to:
- Offer a single, fluent entry point for configuring orchestration
- Allow easy overrides of internal components (registries, builders, dispatchers, executors)
- Keep construction logic modular through small, focused sub‑factories
- Maintain deterministic, registry‑driven orchestration
- Support test isolation and dependency injection

## Key Features
### Fluent API
The builder exposes a chainable configuration surface, the fullest possible example of which 
looks like this, where every component is overridden:

```
var orchestrator = new OrchestratorBuilder()
                       .WithClient(<ChatClient> client)
                       .WithResponseHandler(<IResponseHandler> handler)
                       .WithModelRegistry(<IAIModelRegistry> customRegistry)
                       .WithRequestBuilder(<ClientRequestBuilder> customRequestBuilder)
                       .WithSingleModelDispatcher(<ISingleModelDispatcher> customSingleModelDispatcher)
                       .WithSingleModelExecutor(<ISingleModelExecutor> customSingleModelExecutor);
                       .WithEnsembleDispatcher(<IEnsembleDispatcher> customEnsembleDispatcher)
                       .WithEnsembleExecutor(<IEnsembleExecutor> customEnsembleExecutor);
                       .Build();
```

The minimum configuration is as simple as the following, with the other options being defaulted internally:

```
var orchestrator = new OrchestratorBuilder()
                       .WithClient(client)
                       .WithResponseHandler(handler)
                       .Build();
```

This makes orchestration setup readable and intuitive.

### Sub‑Factories
The builder delegates construction of internal components to dedicated factories:
- ModelRegistryFactory
- DispatcherFactory
- ExecutorFactory
- RequestBuilderFactory
Each factory encapsulates a single responsibility, keeping the builder clean and the system modular.

### Default‑with‑Override Pattern
The builder provides sensible defaults for all components:
- Default model registry
- Default request builder
- Default dispatchers
- Default executors

Users can override any component without affecting the rest of the pipeline.

## Build Process
The builder orchestrates the following steps:
- Resolve the model registry
- Use caller‑provided registry or default factory
- Resolve the request builder
- Use caller‑provided builder or default factory
- Create dispatchers
- Single‑model dispatcher
- Ensemble dispatcher
- Create executors
- Single‑model executor
- Ensemble executor
- Assemble the orchestrator
- Inject all components
- Return a fully configured instance

## Benefits
### Readable and expressive
The fluent API makes orchestration configuration easy to understand at a glance.

### Modular and maintainable
Sub‑factories isolate construction logic and reduce duplication.

### Highly testable
Each subsystem can be mocked or replaced independently.

### Override‑friendly
Users can plug in custom registries, request builders, dispatchers, or executors.

### Deterministic
The builder ensures consistent wiring of all components.

### Extensible
New factories or configuration steps can be added without breaking the API.

## When to Use
The Fluent Orchestrator Builder is ideal when:
- You want a single, expressive entry point for orchestration setup
- You need customizable orchestration profiles
- You value architectural symmetry and clarity
- You want to support multiple model registries or request builders
- You require clean test isolation


# 📄 License
MIT License — free to use, modify, and distribute.