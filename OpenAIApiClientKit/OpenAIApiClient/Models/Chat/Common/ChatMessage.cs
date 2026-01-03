// <copyright file="ChatMessage.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Common
{
    using System.Text.Json.Serialization;
    using OpenAIApiClient.Enums;

    public class ChatMessage
    {
        private string? role;

        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "user", "assistant", "system").
        /// </summary>
        [JsonPropertyName("role")]
        public string Role
        {
            get
            {
                 return this.RoleAsEnum.HasValue ? this.RoleAsEnum.ToString()!.ToLower() : this.role!;
            }

            set
            {
                this.role = value;
            }
        }

        /// <summary>
        /// Gets or sets the role of the message sender as an enumeration.
        /// </summary>
        public OpenAIRole? RoleAsEnum
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

        /// <summary>
        /// Gets or sets the tool call information if the message involves a tool call.
        /// </summary>
        [JsonPropertyName("tool_call")]
        public ToolCall? ToolCall
        {
            get;
            set;
        }
    }
}
