// <copyright file="ModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Orchestration
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Selection;

    /// <summary>
    /// Executes OpenAI models using the provided ChatClient.
    /// </summary>
    /// <param name="client"></param>
    public sealed class ModelExecutor(ChatClient client)
    {
        /// <summary>
        /// Executes the given model with the provided prompt context.
        /// </summary>
        /// <param name="model">The model descriptor.</param>
        /// <param name="context">The prompt context.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the model response.</returns>
        public async Task<ModelResponse> ExecuteAsync(ModelDescriptor model, PromptContext context, CancellationToken cancelToken)
        {
            ChatCompletionRequest request = new ClientRequestBuilder()
                                                .WithModel(model.Model)
                                                .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                                                .AddUserMessage(context.Prompt)
                                                .UsingMaxTokens(input: 1000)
                                                .SetOutputFormat((Enums.OutputFormat)context.OutputFormat!)
                                                .Build();

            // Start timing ..
            Stopwatch sw = Stopwatch.StartNew();

            // Execute request ..
            try
            {
                ChatCompletionResponse? response = await client.CreateChatCompletionAsync(request: request, cancelToken: cancelToken);

                // Stop timing ..
                sw.Stop();

                // Return successful response ..
                return new ModelResponse
                {
                    Model = model,
                    RawOutput = response!.Choices[0].Message.Content!,
                    IsSuccessful = true,
                    Latency = sw.Elapsed,
                    EstimatedCost = response!.Usage?.CalculateCost(pricing: model.Pricing!) ?? 0m,
                    TotalTokens = response!.Usage?.TotalTokens ?? 0m,
                };
            }
            catch (Exception ex)
            {
                // Stop timing ..
                sw.Stop();

                // Return failed response ..
                return new ModelResponse
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
