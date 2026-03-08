// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Executes requests across multiple AI models in an ensemble, aggregating responses from each model.
    /// </summary>
    /// <param name="singleModelExecutor">Executor used to run requests on individual AI models.</param>
    public sealed class EnsembleExecutor(ISingleAiModelExecutor singleModelExecutor) : IEnsembleExecutor
    {
        // Inject the single model executor which will be used to execute requests on individual models as part of the ensemble execution process.
        private readonly ISingleAiModelExecutor singleModelExecutor = singleModelExecutor;

        /// <summary>
        /// Execute the request across all models in the context and return a list of responses, one per model.
        /// The request is built once per model by overriding the model in the request builder with the current model for that iteration of the loop.
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="context"></param>
        /// <param name="options"></param>
        /// <param name="cancelToken"></param>
        /// <returns see cref="IReadOnlyList{AiModelResponse}">Model responses for each model in the context.</returns>
        public async Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ChatClientRequestBuilder requestBuilder, IExecutionContext context, AiCallOptions options, CancellationToken cancelToken)
        {
            IEnumerable<Task<AiModelResponse>> tasks = context.Models.Select(model =>
            {
                // Build the request by overriding the model in the request builder with the current model for this iteration of the loop ..
                ChatCompletionRequest chatRequest = requestBuilder
                    .WithModel(model.Name)
                    .Build();

                // Execute the request on this model ..
                return this.singleModelExecutor.ExecuteAsync(request: chatRequest, options: options, cancelToken: cancelToken);
            });

            // Await all tasks to complete and gather the responses into a list to return ..
            AiModelResponse[] responses = await Task.WhenAll(tasks);
            return responses;
        }
    }
}
