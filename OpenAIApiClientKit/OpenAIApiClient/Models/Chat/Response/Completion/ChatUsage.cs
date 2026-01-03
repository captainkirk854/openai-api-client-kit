// <copyright file="ChatUsage.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Completion
{
    using System.Text.Json.Serialization;

    public class ChatUsage
    {
        /// <summary>
        /// Gets or sets the number of tokens used in the prompt.
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int? PromptTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of tokens generated in the completion.
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public int? CompletionTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Total number of tokens consumed by the request.
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int? TotalTokens
        {
            get;
            set;
        }
    }
}
