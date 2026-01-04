// <copyright file="PromptTokensDetails.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Completion
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Gets details about prompt tokens.
    /// </summary>
    public sealed class PromptTokensDetails
    {
        /// <summary>
        /// Gets or sets the number of cached tokens.
        /// </summary>
        [JsonPropertyName("cached_tokens")]
        public int CachedTokens
        {
            get;
            set;
        }
    }
}