// <copyright file="ChatCompletionChunk.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Streaming
{
    using System.Text.Json.Serialization;

    public class ChatCompletionChunk
    {
        /// <summary>
        /// Gets or sets the unique identifier for this streamed chunk.
        /// </summary>
        [JsonPropertyName("id")]
        required public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the object type, typically "chat.completion.chunk".
        /// </summary>
        [JsonPropertyName("object")]
        required public string Object
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Unix timestamp (in seconds) when this chunk was generated.
        /// </summary>
        [JsonPropertyName("created")]
        required public long Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the model used to generate the streamed response.
        /// </summary>
        [JsonPropertyName("model")]
        required public string Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of streamed choices included in this chunk.
        /// </summary>
        [JsonPropertyName("choices")]
        required public List<ChatChunkChoice> Choices
        {
            get;
            set;
        }
    }
}
