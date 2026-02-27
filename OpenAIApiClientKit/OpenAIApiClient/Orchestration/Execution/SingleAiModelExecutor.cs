// <copyright file="SingleAiModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Executes OpenAI models using the provided ChatClient.
    /// </summary>
    /// <param name="client"></param>
    public sealed class SingleAiModelExecutor(ChatClient client) : ISingleAiModelExecutor
    {
        /// <summary>
        /// Executes the given model with the provided prompt context.
        /// </summary>
        /// <param name="request">Chat Completion request.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the model response.</returns>
        public async Task<AiModelResponse> ExecuteAsync(ChatCompletionRequest request, CancellationToken cancelToken)
        {
            // Initialise ..
            AiExecutionOptions execution = new()
            {
                // if Stream mode is false or null (i.e. non-stream)
                Mode = request.Stream.GetValueOrDefault() ? AiExecutionMode.BufferedStreaming : AiExecutionMode.NonStreaming,
                OnChunkDeltaContentToken = async (model, chunkDeltaContent) =>
                {
                    Console.Write(chunkDeltaContent);   // stream chunk delta(s) to console
                    await Task.Yield();                 // keep it async
                },
                OnChunk = null,   // can be set by caller if they want chunk-level callbacks in streaming modes (e.g. for metadata extraction, tool-calling, etc.)
                AggregateChunkContent = true,  // whether to aggregate output in streaming modes; can be set to false if caller only wants callbacks and doesn't care about final full output
            };

            // Start timing ..
            Stopwatch sw = Stopwatch.StartNew();

            // Execute request ..
            try
            {
                return execution.Mode switch
                {
                    AiExecutionMode.NonStreaming => await this.ExecuteNonStreamingAsync(request, sw, cancelToken),
                    AiExecutionMode.BufferedStreaming => await this.ExecuteBufferedStreamingAsync(request, sw, execution, cancelToken),
                    AiExecutionMode.PushStreaming => await this.ExecutePushStreamingAsync(request, sw, execution, cancelToken),
                    _ => throw new ArgumentOutOfRangeException(paramName: nameof(request), actualValue: execution.Mode, message: "Unknown execution mode."),
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new AiModelResponse
                {
                    Model = request.ModelDescriptor,
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
        private async Task<AiModelResponse> ExecuteNonStreamingAsync(ChatCompletionRequest request,
                                                                     Stopwatch sw,
                                                                     CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelDescriptor;

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
        /// <param name="execution">Execution options, including optional callbacks for chunk content token and chunk processing.</param>
        /// <param name="cancelToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation, containing the aggregated chat completion response as part of the <see cref="AiModelResponse"/>.</returns>
        private async Task<AiModelResponse> ExecuteBufferedStreamingAsync(ChatCompletionRequest request,
                                                                          Stopwatch sw,
                                                                          AiExecutionOptions execution,
                                                                          CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelDescriptor;
            string response = string.Empty;
            int chunkCount = 0;

            // Process streaming chunks as they arrive, but buffer them and only push updates via callbacks once the full response is received at the end of the stream ..
            await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cancelToken))
            {
                ChatDelta chunkDelta = chunk.Choices[0].Delta;
                chunkCount++;

                if (!string.IsNullOrEmpty(chunkDelta.Content))
                {
                    // Aggregate the full response as the chunks arrive, and optionally also push those chunks through callbacks in buffered streaming mode if desired (e.g. for real-time UI updates, etc.) ..
                    response += chunkDelta.Content;

                    // Optional: still allow per-token callback in buffered mode
                    if (execution.OnChunkDeltaContentToken is not null)
                    {
                        await execution.OnChunkDeltaContentToken(model, chunkDelta.Content);
                    }

                    // Optional: per-chunk callback in buffered streaming mode (e.g. for metadata extraction, tool-calling, etc.)
                    if (execution.OnChunk is not null)
                    {
                        await execution.OnChunk(model, chunk);
                    }
                }
            }

            sw.Stop();

            // Note: token/cost usage is not returned by streaming chunks.
            //   - Leave them at 0, or
            //   - Perform a follow-up non-streaming call just as a usage probe (expensive).
            return new AiModelResponse
            {
                Model = model,
                RawOutput = response,
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
        /// <param name="execution">Execution options, including optional callbacks for chunk content token and chunk processing.</param>
        /// <param name="cancelToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>An <see cref="AiModelResponse"/> containing the aggregated response, latency, and metadata.</returns>
        private async Task<AiModelResponse> ExecutePushStreamingAsync(ChatCompletionRequest request,
                                                                      Stopwatch sw,
                                                                      AiExecutionOptions execution,
                                                                      CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelDescriptor;
            string response = string.Empty;
            int chunkCount = 0;

            // Process streaming chunks as they arrive, pushing updates via callbacks without waiting for full response ..
            await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cancelToken))
            {
                ChatDelta chunkDelta = chunk.Choices[0].Delta;
                chunkCount++;

                if (!string.IsNullOrEmpty(chunkDelta.Content))
                {
                    // In this mode, choose to aggregate the full response as the chunks arrive, and/or just push each chunk through callbacks without aggregation ..
                    if (execution.AggregateChunkContent)
                    {
                        response += chunkDelta.Content;
                    }

                    // Optional: per-token callback in push streaming mode
                    if (execution.OnChunkDeltaContentToken is not null)
                    {
                        await execution.OnChunkDeltaContentToken(model, chunkDelta.Content);
                    }
                }

                // Optional: per-chunk callback in push streaming mode (e.g. for metadata extraction, tool-calling, etc.)
                if (execution.OnChunk is not null)
                {
                    await execution.OnChunk(model, chunk);
                }
            }

            sw.Stop();

            return new AiModelResponse
            {
                Model = model,
                RawOutput = response,
                IsSuccessful = true,
                Latency = sw.Elapsed,
                EstimatedCost = 0m,    // or some heuristic
                TotalTokens = 0m,      // unknown without additional call
                ChunkCount = chunkCount,
            };
        }
    }
}