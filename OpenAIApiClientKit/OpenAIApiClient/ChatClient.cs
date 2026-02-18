// <copyright file="ChatClient.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient
{
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using OpenAIApiClient.Exceptions;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;

    /// <summary>
    /// A unified OpenAI Chat Client supporting both streaming and non-streaming
    /// chat completions, with retry and cancellation support.
    /// </summary>
    public class ChatClient
    {
        // String Constants (centralised for maintainability)
        private const string BaseOpenAIApiUrl = "https://api.openai.com/v1/";
        private const string AuthSchema = "Bearer";
        private const string MediaTypeJson = "application/json";
        private const string ChatCompletionsEndpoint = "chat/completions";
        private const string ServerSentEventDataPrefix = "data:";
        private const string ServerSentEventDoneMarker = "[DONE]";
        private const string RetryAfterResponseHeader = "Retry-After";

        // Json Serialiser options ..
        private static readonly JsonSerializerOptions DefaultJsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        // Client Fields
        private readonly HttpClient httpClient;
        private readonly int maxRetries;
        private readonly TimeSpan baseDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatClient"/> class.
        /// </summary>
        /// <param name="apiKey">The OpenAI API key for authentication.</param>
        /// <param name="httpClient">An optional HttpClient instance for making requests.</param>
        /// <param name="maxRetries">Maximum number of retries for transient failures.</param>
        /// <param name="baseDelayMs">Delay in milliseconds for retry backoff.</param>
        public ChatClient(string apiKey, HttpClient? httpClient = null, int maxRetries = 3, int baseDelayMs = 500)
        {
            // Configure retry parameters ..
            this.maxRetries = maxRetries;
            this.baseDelay = TimeSpan.FromMilliseconds(baseDelayMs);

            // Use provided HttpClient or create a new one
            this.httpClient = httpClient ?? new HttpClient();

            // Configure base API endpoint and authentication
            this.httpClient.BaseAddress = new Uri(BaseOpenAIApiUrl);
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: AuthSchema, parameter: apiKey);
        }

        /// <summary>
        /// Gets or sets HttpContent.
        /// </summary>
        private StringContent? HttpContent
        {
            get;
            set;
        }

        /// <summary>
        /// Sends a non-streaming chat completion request with retry and cancellation support.
        /// </summary>
        /// <param name="request">The chat completion request payload.</param>
        /// <param name="cancelToken">A cancellation token to cancel the operation.</param>
        /// <returns>The chat completion response.</returns>
        public async Task<ChatCompletionResponse?> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancelToken = default)
        {
            // Do not use streaming option ..
            request.Stream = false;

            // Serialise request payload convert it to Http content string ..
            this.HttpContent = ConvertToHttpString(request: request);

            // Execute with retry logic ..
            return await this.ExecuteWithRetryAsync(
                operation: async () =>
                {
                    HttpResponseMessage response = await this.httpClient.PostAsync(requestUri: ChatCompletionsEndpoint, content: this.HttpContent, cancellationToken: cancelToken);
                    response.EnsureSuccessStatusCode();

                    string body = await response.Content.ReadAsStringAsync(cancellationToken: cancelToken);

                    return JsonSerializer.Deserialize<ChatCompletionResponse>(json: body, options: DefaultJsonOptions);
                },
                cancellationToken: cancelToken);
        }

        /// <summary>
        /// Sends a streaming chat completion request with retry and cancellation support.
        /// </summary>
        /// <param name="request">The chat completion request payload.</param>
        /// <param name="cancelToken">A cancellation token to cancel the operation.</param>
        /// <returns>An asynchronous stream of chat completion chunks.</returns>
        public async IAsyncEnumerable<ChatCompletionChunk> CreateChatCompletionStreamAsync(ChatCompletionRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancelToken = default)
        {
            // Use streaming option ..
            request.Stream = true;

            // Serialise request payload convert it to Http content string ..
            this.HttpContent = ConvertToHttpString(request: request);

            // Wrap streaming in retry logic
            await foreach (ChatCompletionChunk chunk in this.ExecuteStreamingWithRetryAsync(operation: () => this.SendStreamingRequestAsync(httpContent: this.HttpContent, cancellationToken: cancelToken), cancellationToken: cancelToken))
            {
                yield return chunk;
            }
        }

        /// <summary>
        /// Convert Request to Json Http String.
        /// </summary>
        /// <param name="request"></param>
        /// <returns see cref="StringContent">.</returns>
        private static StringContent ConvertToHttpString(ChatCompletionRequest request)
        {
            // Prepare and return HTTP content ..
            string json = JsonSerializer.Serialize(value: request, options: DefaultJsonOptions);
            return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: MediaTypeJson);
        }

        /// <summary>
        /// Determines whether an exception is likely transient and safe to retry.
        /// </summary>
        /// <param name="ex">The exception to evaluate.</param>
        /// <returns>True if the exception is transient; otherwise, false.</returns>
        private static bool IsSafeToRetry(Exception ex)
        {
            return ex is HttpRequestException ||
                   ex is TaskCanceledException ||
                   ex is TimeoutException;
        }

        /// <summary>
        /// Gets the retry-after delay from the response headers or uses a fallback.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fallback"></param>
        /// <returns>Timespan.</returns>
        private static TimeSpan GetRetryAfterDelay(HttpResponseMessage response, TimeSpan fallback)
        {
            // Check for "Retry-After" header ..
            if (response.Headers.TryGetValues(RetryAfterResponseHeader, out var values))
            {
                string? retryAfter = values.FirstOrDefault();
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
        /// Distributes jitter around a base delay to avoid clustering.
        /// </summary>
        /// <param name="baseDelay"></param>
        /// <returns>Timespan.</returns>
        private static TimeSpan Jitter(TimeSpan baseDelay)
        {
            double rnd = (Random.Shared.NextDouble() * 0.4) + 0.8; // 0.8–1.2x
            return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * rnd);
        }

        /// <summary>
        /// Executes an operation with retry logic for transient failures.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The result of the operation.</returns>
        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken)
        {
            // Retry loop with exponential backoff ..
            for (int attempt = 1; attempt <= this.maxRetries; attempt++)
            {
                // Check for cancellation ..
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await operation();
                }
                catch (RateLimitException ex) when (attempt < this.maxRetries)
                {
                    TimeSpan retryDelay = GetRetryAfterDelay(response: ex.Response, fallback: this.baseDelay);
                    await Task.Delay(retryDelay, cancellationToken);
                }
                catch (Exception ex) when (IsSafeToRetry(ex) && attempt < this.maxRetries)
                {
                    // Define and apply exponential backoff delay ..
                    TimeSpan delay = TimeSpan.FromMilliseconds(this.baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new InvalidOperationException("Retry limit exceeded.");
        }

        /// <summary>
        /// Executes a streaming operation with retry logic.
        /// </summary>
        /// <param name="operation">The streaming operation to execute.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>An asynchronous stream of items from the operation.</returns>
        private async IAsyncEnumerable<T> ExecuteStreamingWithRetryAsync<T>(Func<IAsyncEnumerable<T>> operation, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            for (int attempt = 1; attempt <= this.maxRetries; attempt++)
            {
                // Check for cancellation ..
                cancellationToken.ThrowIfCancellationRequested();

                bool shouldRetry = false;
                List<T> bufferedItems = [];

                // Buffer items from the streaming operation ..
                try
                {
                    await foreach (var item in operation().WithCancellation(cancellationToken))
                    {
                        bufferedItems.Add(item);
                    }
                }
                catch (RateLimitException ex) when (attempt < this.maxRetries)
                {
                    TimeSpan retryDelay = GetRetryAfterDelay(response: ex.Response, fallback: this.baseDelay);
                    await Task.Delay(retryDelay, cancellationToken);
                    shouldRetry = true;
                }
                catch (Exception ex) when (IsSafeToRetry(ex) && attempt < this.maxRetries)
                {
                    // Define and apply exponential backoff delay ..
                    TimeSpan delay = TimeSpan.FromMilliseconds(this.baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    await Task.Delay(delay, cancellationToken);
                    shouldRetry = true;
                }

                if (!shouldRetry)
                {
                    foreach (var item in bufferedItems)
                    {
                        yield return item;
                    }
                    yield break;
                }
            }

            throw new InvalidOperationException("Streaming retry limit exceeded.");
        }

        /// <summary>
        /// Performs the actual streaming request and yields chunks as they arrive.
        /// </summary>
        /// <param name="httpContent">The HTTP content for the request.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>An asynchronous stream of chat completion chunks.</returns>
        private async IAsyncEnumerable<ChatCompletionChunk> SendStreamingRequestAsync(HttpContent httpContent, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Build HTTP request
            using HttpRequestMessage httpRequest = new(method: HttpMethod.Post, requestUri: ChatCompletionsEndpoint)
            {
                Content = httpContent,
            };

            // Request headers-only completion to enable SSE streaming
            using HttpResponseMessage response = await this.httpClient.SendAsync(request: httpRequest, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken: cancellationToken);

            response.EnsureSuccessStatusCode();

            // Open the response stream
            await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using StreamReader reader = new(stream: stream);

            // Process Server-Sent Events (SSE) line-by-line
            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string? line = await reader.ReadLineAsync(cancellationToken);

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Only process SSE "data:" lines
                if (!line.StartsWith(value: ServerSentEventDataPrefix, comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Extract JSON payload
                string payload = line[ServerSentEventDataPrefix.Length..].Trim();

                // End-of-stream marker
                if (payload == ServerSentEventDoneMarker)
                {
                    yield break;
                }

                // Deserialize streamed chunk
                ChatCompletionChunk? chunk = JsonSerializer.Deserialize<ChatCompletionChunk>(json: payload, options: DefaultJsonOptions);
                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }
    }
}
