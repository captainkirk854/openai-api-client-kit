// <copyright file="AiCallOptions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Streaming;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Provides configuration options for AI call execution, including streaming mode, callbacks, and content
    /// aggregation.
    /// </summary>
    public sealed class AiCallOptions
    {
        /// <summary>
        /// Gets the mode of the AI call, which determines how the client will handle streaming responses.
        /// </summary>
        public AiCallMode Mode
        {
            get;
            init;
        } = AiCallMode.NonStreaming;

        /// <summary>
        /// Gets the Optional per-token callback used when <see cref="Mode"/> is PushStreaming.
        /// Signature: (<see cref="AiModelDescriptor"/>, chunk content text token) => Task.
        /// </summary>
        /// <remarks>
        /// Example: OnChunkContentTextToken = (modelDescriptor, chunkDeltaContent) => { Console.Write(chunkDeltaContent); await Task.Yield(); }.
        /// </remarks>
        public Func<AiModelDescriptor, string, Task>? OnChunkDeltaContentToken
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional per-chunk callback for push streaming (if the actual full chunks are desired).
        /// Signature: (<see cref="AiModelDescriptor"/>, chunk) => Task.
        /// </summary>
        /// <remarks>
        /// Example: OnChunk = (modelDescriptor, chunk) => { Console.WriteLine($"Received chunk with content: {chunk}"); await Task.Yield(); }.
        /// Can be set by caller if they want chunk-level callbacks in streaming modes (e.g. for metadata extraction, tool-calling, etc.).
        /// Caution:
        /// This callback will be triggered for every chunk received during push streaming, so it may be called multiple times per response.
        /// Ensure that the implementation is efficient and can handle the potential high frequency of calls without causing performance issues.
        /// </remarks>
        public Func<AiModelDescriptor, ChatCompletionChunk, int, Task>? OnChunk
        {
            get;
            init;
        }

        /// <summary>
        /// Gets a value indicating whether to control if the executor should also aggregate
        /// all chunk content text tokens into <see cref="AiModelResponse.RawOutput"/> during push streaming.
        /// </summary>
        /// <remarks>
        /// Can be set to false if caller only wants callbacks and doesn't care about final full output.
        /// Caution: Setting this to true may have performance implications for very long responses, as it requires concatenating all chunk
        /// content tokens into a single string. Consider the expected response length and performance requirements when configuring this option.
        /// </remarks>
        public bool AggregateChunkContent
        {
            get;
            init;
        } = true;
    }
}
