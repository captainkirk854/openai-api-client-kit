// <copyright file="AiExecutionOptions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Streaming;
    using OpenAIApiClient.Models.Registries;

    public sealed class AiExecutionOptions
    {
        /// <summary>
        /// Gets the execution mode to use (non-streaming, buffered streaming, or push streaming).
        /// </summary>
        public AiExecutionMode Mode
        {
            get;
            init;
        } = AiExecutionMode.NonStreaming;

        /// <summary>
        /// Gets the Optional per-token callback used when <see cref="Mode"/> is PushStreaming.
        /// Signature: (<see cref="AiModelDescriptor"/>, chunk content text token) => Task.
        /// Example: OnChunkContentTextToken = (modelDescriptor, chunkDeltaContent) => { Console.Write(chunkDeltaContent); await Task.Yield(); }.
        /// </summary>
        public Func<AiModelDescriptor, string, Task>? OnChunkDeltaContentToken
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional per-chunk callback for push streaming (if the actual full chunks are desired).
        /// Signature: (<see cref="AiModelDescriptor"/>, chunk) => Task.
        /// Example: OnChunk = (modelDescriptor, chunk) => { Console.WriteLine($"Received chunk with content: {chunk}"); await Task.Yield(); }.
        /// </summary>
        public Func<AiModelDescriptor, ChatCompletionChunk, Task>? OnChunk
        {
            get;
            init;
        }

        /// <summary>
        /// Gets a value indicating whether the Optional flag to control whether the executor should also aggregate
        /// all chunk content text tokens into <see cref="AiModelResponse.RawOutput"/> during push streaming.
        /// </summary>
        public bool AggregateChunkContent
        {
            get;
            init;
        } = true;
    }
}
