// <copyright file="ChatDelta.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Streaming
{
    using System.Text.Json.Serialization;
    using OpenAIApiClient.Models.Chat.Response.Common;

    /// <summary>
    /// Represents a chunk of streamed chat message data, including role, content, and tool calls.
    /// </summary>
    public class ChatDelta
    {
        /// <summary>
        /// Gets or sets the role of the message sender, if provided in this chunk.
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the partial text content generated in this chunk.
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the optional list of tool calls streamed incrementally.
        /// </summary>
        [JsonPropertyName("tool_calls")]
        public List<ChatToolCall>? ToolCalls
        {
            get;
            set;
        }
    }
}
