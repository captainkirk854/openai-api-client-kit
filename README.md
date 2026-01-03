🚀 ChatGPT C# Client
A lightweight, production‑ready C# wrapper for accessing OpenAI’s ChatGPT models — no Azure deployment required.
Supports:
- ✔️ Simple chat completions
- ✔️ Streaming responses (token‑by‑token)
- ✔️ Automatic retries
- ✔️ Exponential backoff with jitter
- ✔️ Smart handling of HTTP 429 Rate Limit + Retry-After header
- ✔️ Clean, reusable API surface

📘 Overview
This project provides a robust, developer‑friendly C# client for interacting with OpenAI’s Chat Completion API.
It’s designed for:
- Backend services
- Desktop apps
- Tools and utilities
- High‑reliability integrations
No Azure OpenAI deployment is required — this client talks directly to OpenAI’s public API.

🔧 Features
🧵 Streaming Support
Consume responses as they are generated using IAsyncEnumerable<string>.
🔁 Retry Logic
Automatic retries on:
- Network failures
- Transient errors
- HTTP 429 (rate limit)
⏳ Rate‑Limit Handling
Honors the server’s Retry-After header when present.
🧱 Minimal Dependencies
Only uses System.Net.Http and System.Text.Json.

📦 Installation
Add the required package:
dotnet add package System.Net.Http.Json

🔑 Getting an OpenAI API Key
- Visit https://platform.openai.com
- Log in or create an account
- Go to API Keys
- Click Create new secret key
- Store it securely (never commit it to Git)

Set it as an environment variable:
Windows (PowerShell):
setx OPENAI_API_KEY "sk-yourkeyhere"

🧠 ChatGptClient
Place the full wrapper class in:
/src/ChatGptClient.cs

This class includes:
- Normal completions
- Streaming completions
- Retry logic
- Rate‑limit handling
- Retry-After support

🖥️ Usage Example — Standard Response
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new ChatGptClient(apiKey);

string reply = await client.SendMessageAsync("Write a haiku about winter mornings.");
Console.WriteLine(reply);

🖥️ Usage Example — Streaming Response
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new ChatGptClient(apiKey);

await foreach (var chunk in client.StreamMessageAsync("Write a haiku about winter mornings."))
{
    Console.Write(chunk);
}
Console.WriteLine();

🛡️ Error Handling
The client automatically:
- Retries on transient network errors
- Detects HTTP 429
- Reads and honors Retry-After
- Falls back to exponential backoff with jitter
You can configure retry behavior:
var client = new ChatGptClient(apiKey, maxRetries: 5, baseDelayMs: 300);

🧪 Testing Your Setup
Run:
dotnet run


If everything is configured correctly, you’ll see ChatGPT’s response in your console.

📄 License
MIT License — free to use, modify, and distribute.
