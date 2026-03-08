// <copyright file="ChatChoice.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Completion
{
    using System.Text.Json.Serialization;
    using OpenAIApiClient.Models.Chat.Common;
    using OpenAIApiClient.Models.Chat.Response.Common;

    /// <summary>
    /// Represents a single choice returned by a chat model, including its index, message, finish reason, and optional
    /// tool calls.
    /// </summary>
    public class ChatChoice
    {
        /// <summary>
        /// Gets or sets the index of this choice in the returned list.
        /// </summary>
        [JsonPropertyName("index")]
        required public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message content returned by the model.
        /// </summary>
        [JsonPropertyName("message")]
        required public ChatMessage Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reason the model stopped generating (e.g., "stop", "length").
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string? FinishReason
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Optional list of tool calls returned by the model.
        /// </summary>
        [JsonPropertyName("tool_calls")]
        public List<ChatToolCall>? ToolCalls
        {
            get;
            set;
        }
    }
}
