// <copyright file="ChatCompletionResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Completion
{
    using System.Text.Json.Serialization;

    public class ChatCompletionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for this chat completion response.
        /// </summary>
        [JsonPropertyName("id")]
        required public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the object type, typically "chat.completion".
        /// </summary>
        [JsonPropertyName("object")]
        required public string Object
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Unix timestamp (in seconds) when the completion was created.
        /// </summary>
        [JsonPropertyName("created")]
        required public long Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the model used to generate the completion.
        /// </summary>
        [JsonPropertyName("model")]
        required public string Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of choices returned by the model.
        /// </summary>
        [JsonPropertyName("choices")]
        required public List<ChatChoice> Choices
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the token usage statistics for this request.
        /// </summary>
        [JsonPropertyName("usage")]
        public ChatUsage? Usage
        {
            get;
            set;
        }
    }
}
