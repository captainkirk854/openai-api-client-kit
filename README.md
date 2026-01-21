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
See the ```OpenAIApiClient.ConsoleApp``` project for a simple console application demonstrating usage.
- The ```ModelPromptDemo.cs``` file provides an example of how to prompt different models and handle their responses (non-streaming and streaming).)
- The ```OptimalModelSelectionDemo.cs``` file provides an example as to how to select the best model based on capabilities and cost.

# 🧪 Testing Your Setup
Run:
```dotnet run```
or use the simple console app provided in the ```OpenAIApiClient.ConsoleApp``` project.

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
A flexible ```ModelCapability``` enum has been defined to represent what each model can do:
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
```Dictionary<OpenAIModel, ModelDescriptor>```

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


---



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





# 📄 License
MIT License — free to use, modify, and distribute.
