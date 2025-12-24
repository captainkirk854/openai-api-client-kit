// <copyright file="ChatChunkChoice.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Streaming
{
    using System.Text.Json.Serialization;

    public class ChatChunkChoice
    {
        /// <summary>
        /// Gets or sets the index of this choice in the streamed response.
        /// </summary>
        [JsonPropertyName("index")]
        required public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the incremental delta update for this chunk.
        /// </summary>
        [JsonPropertyName("delta")]
        required public ChatDelta Delta
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reason the model stopped generating, if this chunk ends the stream.
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string? FinishReason
        {
            get;
            set;
        }
    }
}
