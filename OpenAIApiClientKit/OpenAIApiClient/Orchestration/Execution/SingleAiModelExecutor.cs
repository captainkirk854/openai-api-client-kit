// <copyright file="SingleAiModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Executes OpenAI models using the provided ChatClient.
    /// </summary>
    /// <param name="client"></param>
    public sealed class SingleAiModelExecutor(ChatClient client) : ISingleAiModelExecutor
    {
        /// <summary>
        /// Executes the given model with the provided prompt context.
        /// </summary>
        /// <param name="request">Chat Completion request.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the model response.</returns>
        public async Task<AiModelResponse> ExecuteAsync(ChatCompletionRequest request, CancellationToken cancelToken)
        {
            // Initialise ..
            AiModelDescriptor model = request.ModelDescriptor;

            // Start timing ..
            Stopwatch sw = Stopwatch.StartNew();

            // Execute request ..
            try
            {
                ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cancelToken);

                // Stop timing ..
                sw.Stop();

                // Return successful response ..
                if (response == null || response.Choices.Count == 0)
                {
                    throw new InvalidOperationException("The response from the model was null or contained no response choices.");
                }
                return new AiModelResponse
                {
                    Model = model,
                    RawOutput = response.Choices[0].Message.Content!,
                    IsSuccessful = true,
                    Latency = sw.Elapsed,
                    EstimatedCost = response.Usage?.CalculateCost(pricing: model.Pricing!) ?? 0m,
                    TotalTokens = response.Usage?.TotalTokens ?? 0m,
                };
            }
            catch (Exception ex)
            {
                // Stop timing ..
                sw.Stop();

                // Return failed response ..
                return new AiModelResponse
                {
                    Model = model,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Latency = sw.Elapsed,
                    EstimatedCost = 0m,
                    TotalTokens = 0m,
                };
            }
        }
    }
}
