# 🚀 ChatGPT C# Client

A lightweight, production‑ready C# wrapper for accessing OpenAI’s ChatGPT models — no Azure deployment required.

Supports:
- ✔️ Simple chat completions
- ✔️ Streaming responses (token‑by‑token)
- ✔️ Automatic retries
- ✔️ Exponential backoff with jitter
- ✔️ Smart handling of HTTP 429 Rate Limit + Retry-After header
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

Consume responses as they are generated using IAsyncEnumerable<string>.

## 🔁 Retry Logic

Automatic retries on:
- Network failures
- Transient errors
- HTTP 429 (rate limit)

## ⏳ Rate‑Limit Handling
Honors the server’s Retry-After header when present.

## 🧱 Minimal Dependencies

Only uses System.Net.Http and System.Text.Json.

# 📦 Installation

Add the required package:
```dotnet add package System.Net.Http.Json```

# 🔑 Getting an OpenAI API Key
- Visit https://platform.openai.com
- Log in or create an account
- Go to API Keys
- Click Create new secret key
- Store it securely (never commit it to Git)

Set it as an environment variable:
Windows (PowerShell):
```setx OPENAI_API_KEY "your-api-key"```

# 🧠 ChatGptClient
Place the full wrapper class in:
/src/ChatGptClient.cs

This class includes:
- Normal completions
- Streaming completions
- Retry logic
- Rate‑limit handling
- Retry-After support

# 🖥️ Usage Examples

## Common Setup
```
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

ChatClient client = new(apiKey: apiKey);

ChatCompletionRequest request = new ClientRequestBuilder().WithModel(input: OpenAIModels.GPT4o_Mini)
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
    // Extract delta content from chunk ..
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

# 🧪 Testing Your Setup
Run:
```dotnet run```
or use the simple console app provided in the ```OpenAIApiClient.ConsoleApp``` project.

*(Tip: Remember to set your OPENAI_API_KEY environment variable first (see above))*

If everything is configured correctly, you’ll see ChatGPT’s response in your console.

# 📄 License
MIT License — free to use, modify, and distribute.
