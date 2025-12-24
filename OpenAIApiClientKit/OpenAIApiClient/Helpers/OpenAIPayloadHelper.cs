// <copyright file="OpenAIPayloadHelper.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers
{
    using OpenAIApiClient.Models.Chat.Common;
    using OpenAIApiClient.Models.Chat.Request;

    /// <summary>
    /// Provides factory methods for constructing payload objects used with OpenAI API requests.
    /// </summary>
    public static class OpenAIPayloadHelper
    {
        /// <summary>
        /// Builds a ChatCompletionRequest object for the specified model and user prompt.
        /// </summary>
        /// <param name="model">The OpenAI model to be used for the request.</param>
        /// <param name="userPrompt">The user input prompt to include in the chat message sequence. Cannot be null.</param>
        /// <param name="stream">A value indicating whether the response should be streamed. Set to <see langword="true"/> to enable streaming; otherwise, <see langword="false"/>.</param>
        /// <param name="temperature">Valid values range from 0 - 2.0.</param>
        /// <param name="maxTokens"></param>
        /// <returns>ChatCompletionRequest.</returns>
        public static ChatCompletionRequest BuildChatCompletionRequestObject(OpenAIModels model, string userPrompt, bool stream, double temperature = 0.1, int maxTokens = 1000)
        {
            // Check that temperature is within valid range 0 - 2.0 ..
            temperature = temperature < 0 ? 0 : temperature > 2.0 ? 2.0 : temperature;

            ChatCompletionRequest request = new()
            {
                Model = OpenAIModelHelper.ToApiString(model),
                Messages =
                [
                    new ChatMessage
                    {
                        Role = OpenAIRole.System.ToString().ToLower(),
                        Content = "You are a helpful assistant that answers concisely",
                    },
                    new ChatMessage
                    {
                        Role = OpenAIRole.User.ToString().ToLower(),
                        Content = userPrompt,
                    }
                ],
                Stream = stream,
                Temperature = temperature,
                MaxTokens = maxTokens,

                // Tools = new List<ChatToolDefinition> { /* Define tools here if needed */ }
            };

            return request;
        }
    }
}