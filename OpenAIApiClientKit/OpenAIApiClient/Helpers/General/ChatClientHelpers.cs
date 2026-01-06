// <copyright file="ChatClientHelpers.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.General
{
    using System.Threading.Tasks;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;

    public static class ChatClientHelpers
    {
        /// <summary>
        /// Asynchronously retrieves the content of the first message from a chat completion response using the specified client and request.
        /// </summary>
        /// <param name="client">The chat client used to send the completion request.</param>
        /// <param name="request">The chat completion request containing the prompt and parameters for the completion.</param>
        /// <param name="cancelTokenSource">A cancellation token source that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the content of the first message
        /// in the chat completion response, or null if the response or message content is unavailable.</returns>
        public static async Task<string?> GetChatCompletionNonStreamingMessageContentAsync(ChatClient client, ChatCompletionRequest request, CancellationTokenSource cancelTokenSource)
        {
            ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cancelTokenSource.Token);
            return response?.Choices[0].Message.Content;
        }

        /// <summary>
        /// Asynchronously streams and aggregates the content of a chat completion message from the specified client using the provided request parameters.
        /// </summary>
        /// <remarks>
        /// The method streams the response in real time and concatenates all content chunks into a single string. If the operation is canceled via the provided
        /// cancellation token, the returned string may contain partial content.
        /// </remarks>
        /// <param name="client">The chat client used to initiate and manage the streaming chat completion request. Cannot be null.</param>
        /// <param name="request">The parameters that define the chat completion request, including messages and model options.</param>
        /// <param name="cancelTokenSource">A cancellation token source that can be used to cancel the streaming operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the aggregated message content
        /// as a string, or null if no content was received.</returns>
        public static async Task<(string?, int)> GetChatCompletionStreamingMessageContentAsync(ChatClient client, ChatCompletionRequest request, CancellationTokenSource cancelTokenSource)
        {
            // Initialise variables ..
            string? response = string.Empty;
            int chunkCount = 0;

            // Stream the response chunk(s) ..
            await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancelToken: cancelTokenSource.Token))
            {
                // Extract delta content from chunk and concatenate to final response ..
                ChatDelta chunkDelta = chunk.Choices[0].Delta;
                response += chunkDelta.Content;
                chunkCount++;
            }

            // Return the finalised response ..
            return (response, chunkCount);
        }
    }
}
