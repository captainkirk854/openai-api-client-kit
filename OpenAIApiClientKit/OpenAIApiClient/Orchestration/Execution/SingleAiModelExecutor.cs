// <copyright file="SingleAiModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Executes chat completion requests using a single AI model, supporting non-streaming, buffered streaming, and
    /// push streaming modes with optional callbacks and latency measurement.
    /// </summary>
    /// <param name="client">Chat client used to communicate with the AI model.</param>
    public sealed class SingleAiModelExecutor(ChatClient client) : ISingleAiModelExecutor
    {
        /// <summary>
        /// Executes the given model with the provided prompt context.
        /// </summary>
        /// <param name="request">Chat Completion request.</param>
        /// <param name="options">Execution options, including mode and callbacks.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the model response.</returns>
        public async Task<AiModelResponse> ExecuteAsync(ChatCompletionRequest request, AiCallOptions options, CancellationToken cancelToken)
        {
            // Start timing ..
            Stopwatch sw = Stopwatch.StartNew();

            // Execute request ..
            try
            {
                return options.Mode switch
                {
                    AiCallMode.NonStreaming => await this.ExecuteNonStreamingAsync(request: request, sw: sw, cancelToken: cancelToken),
                    AiCallMode.BufferedStreaming => await this.ExecuteBufferedStreamingAsync(request: request, sw: sw, options: options, cancelToken: cancelToken),
                    AiCallMode.PushStreaming => await this.ExecutePushStreamingAsync(request: request, sw: sw, options: options, cancelToken: cancelToken),
                    _ => throw new ArgumentOutOfRangeException(paramName: nameof(options), actualValue: options.Mode, message: "Unknown call option mode."),
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new AiModelResponse
                {
                    Model = request.ModelInfo,
                    RawOutput = string.Empty,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Latency = sw.Elapsed,
                    EstimatedCost = 0m,
                    TotalTokens = 0m,
                };
            }
        }

        /// <summary>
        /// Executes a chat completion request asynchronously and returns the full response without streaming.
        /// </summary>
        /// <param name="request">The chat completion request to execute.</param>
        /// <param name="sw">A stopwatch used to measure the latency of the operation.</param>
        /// <param name="cancelToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>An <see cref="AiModelResponse"/> containing the result of the chat completion request.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the response from the model is null or contains no choices.</exception>
        private async Task<AiModelResponse> ExecuteNonStreamingAsync(ChatCompletionRequest request, Stopwatch sw, CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelInfo;

            // Execute request and wait for full response ..
            ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cancelToken);

            sw.Stop();

            if (response == null || response.Choices.Count == 0)
            {
                throw new InvalidOperationException("The response from the model was null or contained no choices.");
            }

            return new AiModelResponse
            {
                Model = model,
                RawOutput = response.Choices[0].Message.Content!,
                IsSuccessful = true,
                Latency = sw.Elapsed,
                EstimatedCost = response.Usage?.CalculateCost(pricing: model.Pricing!) ?? 0m,
                TotalTokens = response.Usage?.TotalTokens ?? 0m,
            };
        }

        /// <summary>
        /// Executes a buffered streaming chat completion request, aggregating all response chunks before returning the final result
        /// and optionally invoking callbacks for each token or chunk as they arrive. This mode allows you to get the full response at
        /// the end of the stream, while still having the option to process tokens/chunks in real-time via callbacks if desired.
        /// </summary>
        /// <remarks>
        /// Token usage and cost information are not available from streaming responses and are set to zero unless a follow-up request is made.
        /// </remarks>
        /// <param name="request">The chat completion request to process.</param>
        /// <param name="sw">A stopwatch used to measure the operation's latency.</param>
        /// <param name="options">Execution options, including optional callbacks for chunk content token and chunk processing.</param>
        /// <param name="cancelToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation, containing the aggregated chat completion response as part of the <see cref="AiModelResponse"/>.</returns>
        private async Task<AiModelResponse> ExecuteBufferedStreamingAsync(ChatCompletionRequest request, Stopwatch sw, AiCallOptions options, CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelInfo;
            StringBuilder outputBuilder = new();
            int chunkCount = 0;

            // Process streaming chunks as they arrive and invoke OnChunkDeltaContentToken and OnChunk on every chunk as it arrives ..
            await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cancelToken))
            {
                ChatDelta chunkDelta = chunk.Choices[0].Delta;
                chunkCount++;

                if (!string.IsNullOrEmpty(chunkDelta.Content))
                {
                    // Aggregate the full response as the chunks arrive, and optionally also push those chunks through callbacks in buffered streaming mode if desired (e.g. for real-time UI updates, etc.) ..
                    outputBuilder.Append(chunkDelta.Content);

                    // Optional: still allow per-token callback in buffered mode
                    if (options.OnChunkDeltaContentToken is not null)
                    {
                        await options.OnChunkDeltaContentToken(model, chunkDelta.Content);
                    }
                }

                // Optional: per-chunk callback in buffered streaming mode (e.g. for metadata extraction, tool-calling, etc.)
                if (options.OnChunk is not null)
                {
                    await options.OnChunk(model, chunk, chunkCount);
                }
            }

            sw.Stop();

            // Note: token/cost usage is not returned by streaming chunks.
            //   - Leave them at 0, or
            //   - Perform a follow-up non-streaming call just as a usage probe (expensive).
            return new AiModelResponse
            {
                Model = model,
                RawOutput = outputBuilder.ToString(),
                IsSuccessful = true,
                Latency = sw.Elapsed,
                EstimatedCost = 0m,    // or some heuristic
                TotalTokens = 0m,      // unknown without additional call
                ChunkCount = chunkCount,
            };
        }

        /// <summary>
        /// Executes a streaming chat completion request, processing response chunks as they arrive and optionally invoking
        /// callbacks for each token or chunk.
        /// </summary>
        /// <remarks>
        /// Token usage and cost information are not available from streaming responses and are set to zero unless a follow-up request is made.
        /// </remarks>
        /// <param name="request">The chat completion request to send to the AI model.</param>
        /// <param name="sw">A stopwatch used to measure the latency of the operation.</param>
        /// <param name="options">Execution options, including optional callbacks for chunk content token and chunk processing.</param>
        /// <param name="cancelToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>An <see cref="AiModelResponse"/> containing the aggregated response, latency, and metadata.</returns>
        private async Task<AiModelResponse> ExecutePushStreamingAsync(ChatCompletionRequest request, Stopwatch sw, AiCallOptions options, CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelInfo;
            StringBuilder outputBuilder = new();
            int chunkCount = 0;

            // Process streaming chunks as they arrive, pushing updates via callbacks without waiting for full response ..
            await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cancelToken))
            {
                ChatDelta chunkDelta = chunk.Choices[0].Delta;
                chunkCount++;

                if (!string.IsNullOrEmpty(chunkDelta.Content))
                {
                    // In this mode, choose to aggregate the full response as the chunks arrive, and/or just push each chunk through callbacks without aggregation ..
                    if (options.AggregateChunkContent)
                    {
                        outputBuilder.Append(chunkDelta.Content);
                    }

                    // Optional: per-token callback in push streaming mode
                    if (options.OnChunkDeltaContentToken is not null)
                    {
                        await options.OnChunkDeltaContentToken(model, chunkDelta.Content);
                    }
                }

                // Optional: per-chunk callback in push streaming mode (e.g. for metadata extraction, tool-calling, etc.)
                if (options.OnChunk is not null)
                {
                    await options.OnChunk(model, chunk, chunkCount);
                }
            }

            sw.Stop();

            return new AiModelResponse
            {
                Model = model,
                RawOutput = outputBuilder.ToString(),
                IsSuccessful = true,
                Latency = sw.Elapsed,
                EstimatedCost = 0m,    // or some heuristic
                TotalTokens = 0m,      // unknown without additional call
                ChunkCount = chunkCount,
            };
        }
    }
}