// <copyright file="ChatMessage.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Common
{
    using System.Text.Json.Serialization;

    public class ChatMessage
    {
        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "user", "assistant", "system").
        /// </summary>
        [JsonPropertyName("role")]
        required public string Role
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content
        {
            get;
            set;
        }
    }
}
