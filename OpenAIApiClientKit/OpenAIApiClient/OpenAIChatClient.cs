// <copyright file="OpenAIChatClient.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using OpenAIApiClient.Exceptions;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;

    public class OpenAIChatClient
    {
        private const string DefaultBaseUrl = "https://api.openai.com/v1/";
        private const string ChatCompletionsEndpoint = "chat/completions";
        private readonly OpenAIModels model;
        private readonly string apiKey;
        private readonly string baseUrl;
        private readonly int maxRetries;
        private readonly TimeSpan baseDelay;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIChatClient"/> class.
        /// </summary>
        /// <remarks>This constructor configures the client with default HTTP headers and retry logic. The
        /// client is ready to send requests immediately after instantiation.</remarks>
        /// <param name="apiKey">The API key used to authenticate requests to the OpenAI API. Cannot be null.</param>
        /// <param name="model">The OpenAI model to use for chat completions.</param>
        /// <param name="baseUrl">The base URL for the OpenAI API endpoint. If not specified, the default base URL is used.</param>
        /// <param name="maxRetries">The maximum number of times to retry failed requests due to transient errors. Must be zero or greater.</param>
        /// <param name="baseDelayMs">The base delay, in milliseconds, between retry attempts. Used to calculate exponential backoff for retries.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiKey is null.</exception>
        public OpenAIChatClient(string apiKey, OpenAIModels model, string baseUrl = DefaultBaseUrl, int maxRetries = 3, int baseDelayMs = 500)
        {
            this.apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            this.model = model;
            this.baseUrl = baseUrl;
            this.maxRetries = maxRetries;
            this.baseDelay = TimeSpan.FromMilliseconds(baseDelayMs);

            // Initialize HttpClient with default headers ..
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.baseUrl),
            };
            this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.apiKey}");
        }

        /// <summary>
        /// Sends a user prompt to the OpenAI chat completion API asynchronously and returns the assistant's response.
        /// </summary>
        /// <param name="prompt">The user input or question to send to the assistant. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the assistant's response as a string. Returns an empty string if the response content is missing.</returns>
        /// <exception cref="RateLimitException">Thrown if the OpenAI API rate limit is exceeded.</exception>
        public async Task<string> CreateChatCompletionAsync(string prompt)
        {
            // Build the request payload ..
            ChatCompletionRequest payload = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: this.model, userPrompt: prompt, stream: false);

            return await this.ExecuteWithRetry(async () =>
            {
                HttpResponseMessage? response = await this.httpClient.PostAsJsonAsync(requestUri: ChatCompletionsEndpoint, value: payload);
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new RateLimitException("Rate limit exceeded!", response);
                }

                response.EnsureSuccessStatusCode();

                JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            });
        }

        /// <summary>
        /// Sends a chat completion request (with optional tools) and returns the typed response.
        /// </summary>
        /// <param name="prompt">The user prompt to send to the chat completion model. This text will be included as the user's message in the conversation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ChatCompletionResponse"/> from the API, or null if deserialization fails.</returns>
        public async Task<ChatCompletionResponse?> CreateChatCompletionAsyncV2(string prompt)
        {
            // Build the request payload ..
            ChatCompletionRequest request = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: this.model, userPrompt: prompt, stream: false);

            return await this.ExecuteWithRetry(async () =>
            {
                HttpResponseMessage? response = await this.httpClient.PostAsJsonAsync(requestUri: ChatCompletionsEndpoint, value: request, options: this.jsonOptions);
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new RateLimitException("Rate limit exceeded!", response);
                }

                response.EnsureSuccessStatusCode();

                string body = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ChatCompletionResponse>(body, this.jsonOptions);
            });
        }

        /// <summary>
        /// Streams the response from a chat completion API as a sequence of text chunks, enabling real-time processing
        /// of the generated content.
        /// </summary>
        /// <remarks>The method yields each chunk of the response as it is received, allowing callers to
        /// process or display output incrementally. If a transient network error or rate limit is encountered, the
        /// method automatically retries the request up to a configured maximum number of attempts before propagating
        /// the exception. The returned sequence may be empty if the model does not generate any content.
        /// </remarks>
        /// <param name="prompt">The user prompt to send to the chat completion model. This text will be included as the user's message in the conversation.</param>
        /// <returns>An asynchronous sequence of strings, each representing a chunk of the generated response text. The sequence completes when the full response has been streamed.</returns>
        /// <exception cref="RateLimitException">Thrown if the API rate limit is exceeded and the maximum number of retry attempts has been reached.</exception>
        public async IAsyncEnumerable<string> CallOpenAIStreamResponseAsync(string prompt)
        {
            // Build the request payload ..
            ChatCompletionRequest payload = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: this.model, userPrompt: prompt, stream: true);

            // Begin the retry loop ..
            int attempt = 0;
            TimeSpan delay = this.baseDelay;

            while (true)
            {
                bool shouldRetry = false;
                List<string> chunks = [];

                try
                {
                    using var request = new HttpRequestMessage(method: HttpMethod.Post, requestUri: ChatCompletionsEndpoint)
                    {
                        Content = JsonContent.Create(payload),
                    };

                    using var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        throw new RateLimitException("Rate limit exceeded!", response);
                    }

                    response.EnsureSuccessStatusCode();

                    using Stream stream = await response.Content.ReadAsStreamAsync();
                    using StreamReader reader = new(stream);

                    while (!reader.EndOfStream)
                    {
                        // Read line by line
                        string? line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        if (!line.StartsWith("data:"))
                        {
                            continue;
                        }

                        // Extract the JSON payload ..
                        string? data = line["data:".Length..].Trim();
                        if (data == "[DONE]")
                        {
                            continue;
                        }

                        JsonElement json = JsonSerializer.Deserialize<JsonElement>(data);
                        JsonElement delta = json.GetProperty("choices")[0].GetProperty("delta");

                        // Extract the content chunk ..
                        if (delta.TryGetProperty("content", out var contentProp))
                        {
                            string? chunk = contentProp.GetString();
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                chunks.Add(chunk);
                            }
                        }
                    }
                }
                catch (RateLimitException ex) when (attempt < this.maxRetries)
                {
                    TimeSpan retryDelay = GetRetryDelay(ex.Response, delay);
                    await Task.Delay(retryDelay);
                    delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, 8000));
                    attempt++;
                    shouldRetry = true;
                }
                catch (HttpRequestException) when (attempt < this.maxRetries)
                {
                    await Task.Delay(Jitter(delay));
                    shouldRetry = true;
                }

                // Yield any collected chunks outside the try/catch block
                foreach (string? chunk in chunks)
                {
                    yield return chunk;
                }

                if (!shouldRetry)
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Sends a streaming chat completion request and yields chunks as they arrive.
        /// </summary>
        /// <param name="prompt">The user prompt to send to the chat completion model. This text will be included as the user's message in the conversation.</param>
        /// <returns>An asynchronous sequence of <see cref="ChatCompletionChunk"/> objects representing the streamed response from the API.</returns>
        public async IAsyncEnumerable<ChatCompletionChunk> CreateChatCompletionStreamAsyncV2(string prompt)
        {
            // Build the request payload ..
            ChatCompletionRequest request = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: this.model, userPrompt: prompt, stream: true);

            string json = JsonSerializer.Serialize(request, this.jsonOptions);
            StringContent httpContent = new(json, Encoding.UTF8, "application/json");

            using HttpRequestMessage httpRequest = new(HttpMethod.Post, ChatCompletionsEndpoint)
            {
                Content = httpContent,
            };

            using HttpResponseMessage response = await this.httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            await using Stream stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new(stream);

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string payload = line["data:".Length..].Trim();

                if (payload == "[DONE]")
                {
                    yield break;
                }

                ChatCompletionChunk? chunk = JsonSerializer.Deserialize<ChatCompletionChunk>(payload, this.jsonOptions);

                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }

        /// <summary>
        /// Determines the delay interval before retrying a request based on the 'Retry-After' header in the HTTP
        /// response, or returns a fallback delay if the header is not present or invalid.
        /// </summary>
        /// <remarks>The method supports both integer and HTTP-date formats for the 'Retry-After' header,
        /// as specified by RFC 7231. If the header is not present or cannot be parsed, the fallback delay may be
        /// adjusted with jitter to help avoid synchronized retries.
        /// </remarks>
        /// <param name="response">The HTTP response message from which to extract the 'Retry-After' header value.</param>
        /// <param name="fallback">The fallback delay to use if the 'Retry-After' header is missing or cannot be parsed.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the delay to wait before retrying the request. If the 'Retry-After' header is present and valid, the delay is based on its value; otherwise, the fallback delay is used.</returns>
        private static TimeSpan GetRetryDelay(HttpResponseMessage response, TimeSpan fallback)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                var retryAfter = values.FirstOrDefault();
                if (int.TryParse(retryAfter, out var seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }

                if (DateTimeOffset.TryParse(retryAfter, out var date))
                {
                    return date - DateTimeOffset.UtcNow;
                }
            }
            return Jitter(fallback);
        }

        /// <summary>
        /// Calculates a jittered delay based on the specified base delay, introducing randomness.
        /// </summary>
        /// <param name="baseDelay"></param>
        /// <returns>A <see cref="TimeSpan"/> representing the jittered delay.</returns>
        private static TimeSpan Jitter(TimeSpan baseDelay)
        {
            double rnd = (Random.Shared.NextDouble() * 0.4) + 0.8; // 0.8–1.2x
            return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * rnd);
        }

        /// <summary>
        /// Executes the specified asynchronous action with automatic retries in the event of rate limiting or transient
        /// HTTP errors.
        /// </summary>
        /// <remarks>The method retries the action when a rate limit or transient HTTP error occurs, using
        /// an exponential backoff strategy with jitter. The number of retry attempts and delay behavior are determined
        /// by the configuration of the containing class. If the maximum number of retries is exceeded, the last
        /// encountered exception is propagated to the caller.
        /// </remarks>
        /// <typeparam name="T">The type of the result returned by the asynchronous action.</typeparam>
        /// <param name="action">A function that represents the asynchronous operation to execute. The function should return a task that yields a result of type <typeparamref name="T"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the value returned by the action after successful execution.</returns>
        private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action)
        {
            int attempt = 0;
            TimeSpan delay = this.baseDelay;

            while (true)
            {
                try
                {
                    return await action();
                }
                catch (RateLimitException ex) when (attempt < this.maxRetries)
                {
                    var retryDelay = GetRetryDelay(ex.Response, delay);
                    await Task.Delay(retryDelay);
                    delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, 8000));
                    attempt++;
                    continue;
                }
                catch (HttpRequestException) when (attempt < this.maxRetries)
                {
                    await Task.Delay(Jitter(delay));
                    delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, 8000));
                    attempt++;
                    continue;
                }
            }
        }
    }
}
