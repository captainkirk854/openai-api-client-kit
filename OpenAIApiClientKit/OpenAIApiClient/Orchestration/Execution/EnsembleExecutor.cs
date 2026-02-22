// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;

    public sealed class EnsembleExecutor(ISingleModelExecutor singleModelExecutor) : IEnsembleExecutor
    {
        private readonly ISingleModelExecutor singleModelExecutor = singleModelExecutor;

        public async Task<IReadOnlyList<AIModelResponse>> ExecuteAsync(ClientRequestBuilder requestBuilder, IExecutionContext context, CancellationToken cancelToken)
        {
            IEnumerable<Task<AIModelResponse>> tasks = context.Models.Select(model =>
            {
                // Build the request by overriding the model in the request builder with the current model for this iteration of the loop ..
                ChatCompletionRequest chatRequest = requestBuilder
                    .WithModel(model.Name)
                    .Build();

                // Execute the request on this model ..
                return this.singleModelExecutor.ExecuteAsync(request: chatRequest, cancelToken: cancelToken);
            });

            AIModelResponse[] responses = await Task.WhenAll(tasks);
            return responses;
        }
    }
}
