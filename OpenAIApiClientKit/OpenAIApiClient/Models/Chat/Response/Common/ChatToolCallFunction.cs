// <copyright file="ChatToolCallFunction.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Common
{
    using System.Text.Json.Serialization;

    public class ChatToolCallFunction
    {
        /// <summary>
        /// Gets or sets the name of the function being called.
        /// </summary>
        [JsonPropertyName("name")]
        required public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the JSON string containing the arguments for the function.
        /// </summary>
        [JsonPropertyName("arguments")]
        required public string Arguments
        {
            get;
            set;
        }
    }
}