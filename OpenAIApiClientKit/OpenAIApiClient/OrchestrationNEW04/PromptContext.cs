// <copyright file="PromptContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Request;

    /// <summary>
    /// Defines the context of a prompt being sent to the AI model.
    /// </summary>
    public sealed class PromptContext
    {
        /// <summary>
        /// Gets the prompt to be sent to the model.
        /// </summary>
        public string Prompt
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the desired output format.
        /// </summary>
        public OutputFormat? OutputFormat
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the chat completion request details.
        /// </summary>
        public ChatCompletionRequest Request
        {
            get;
            init;
        } = default!;
    }
}
