// <copyright file="ChatToolDefinition.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Request
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a tool definition for chat function calling, including the tool type and function details.
    /// </summary>
    public class ChatToolDefinition
    {
        /// <summary>
        /// Gets or sets the type of tool. For function calling, this is always "function".
        /// </summary>
        [JsonPropertyName("type")]
        required public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function definition describing the callable function.
        /// </summary>
        [JsonPropertyName("function")]
        required public ChatToolFunctionDefinition Function
        {
            get;
            set;
        }
    }
}
