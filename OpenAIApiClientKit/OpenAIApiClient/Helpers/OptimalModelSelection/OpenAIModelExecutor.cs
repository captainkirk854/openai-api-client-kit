// <copyright file="OpenAIModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.OptimalModelSelection
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.OptimalModelSelection;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Executes OpenAI models using the provided ChatClient.
    /// </summary>
    /// <param name="client"></param>
    public sealed class OpenAIModelExecutor(ChatClient client)
    {
        /// <summary>
        /// Executes the given model with the provided prompt context.
        /// </summary>
        /// <param name="model">The model descriptor.</param>
        /// <param name="context">The prompt context.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the model response.</returns>
        public async Task<OpenAIModelResponse> ExecuteAsync(OpenAIModelDescriptor model, PromptContext context, CancellationToken cancelToken)
        {
            ChatCompletionRequest request = new ClientRequestBuilder()
                                                .WithModel(model.Name)
                                                .AddUserMessage(context.Prompt)
                                                .Build();

            // Get pricing info for the model ..
            ModelPricing? pricing = OpenAIModelPricingRegistry.Pricing.TryGetValue(model.Name, out ModelPricing? p) ? p : null;

            // Start timing ..
            Stopwatch sw = Stopwatch.StartNew();

            // Execute request ..
            try
            {
                ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cancelToken);

                // Stop timing ..
                sw.Stop();

                // Return successful response ..
                return new OpenAIModelResponse
                {
                    Model = model,
                    RawOutput = response!.Choices[0].Message.Content!,
                    IsSuccessful = true,
                    Latency = sw.Elapsed,
                    EstimatedCost = response!.Usage?.CalculateCost(pricing: pricing!) ?? 0m,
                    TotalTokens = response!.Usage?.TotalTokens ?? 0m,
                };
            }
            catch (Exception ex)
            {
                // Stop timing ..
                sw.Stop();

                // Return failed response ..
                return new OpenAIModelResponse
                {
                    Model = model,
                    RawOutput = string.Empty,
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
