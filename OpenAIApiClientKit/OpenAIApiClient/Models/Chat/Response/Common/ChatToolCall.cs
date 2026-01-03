// <copyright file="ChatToolCall.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Common
{
    using System.Text.Json.Serialization;

    public class ChatToolCall
    {
        /// <summary>
        /// Gets or sets the Unique identifier for the tool call.
        /// </summary>
        [JsonPropertyName("id")]
        required public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of tool call, usually "function".
        /// </summary>
        [JsonPropertyName("type")]
        required public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the details about the function being invoked.
        /// </summary>
        [JsonPropertyName("function")]
        required public ChatToolCallFunction Function
        {
            get;
            set;
        }
    }
}
