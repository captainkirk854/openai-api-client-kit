// <copyright file="AiCallMode.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum AiCallMode
    {
        /// <summary>
        /// Use non-streaming Chat Completions (single response only).
        /// </summary>
        NonStreaming,

        /// <summary>
        /// Use streaming Chat Completions but buffer all chunk content tokens and only return them
        /// as a final concatenated string in <see cref="AiModelResponse.RawOutput"/>.
        /// </summary>
        BufferedStreaming,

        /// <summary>
        /// Use streaming Chat Completions and push chunk content tokens to a callback as they arrive.
        /// The final <see cref="AiModelResponse.RawOutput"/> is still aggregated unless specified otherwise.
        /// </summary>
        PushStreaming,
    }
}
