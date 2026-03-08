// <copyright file="ChatToolFunctionDefinition.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Request
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a function definition for a chat tool, including its name and parameter schema.
    /// </summary>
    public class ChatToolFunctionDefinition
    {
        /// <summary>
        /// Gets or sets the name of the function the model may call.
        /// </summary>
        [JsonPropertyName("name")]
        required public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the JSON schema object describing the function parameters.
        /// </summary>
        [JsonPropertyName("parameters")]
        required public object Parameters
        {
            get;
            set;
        }
    }
}