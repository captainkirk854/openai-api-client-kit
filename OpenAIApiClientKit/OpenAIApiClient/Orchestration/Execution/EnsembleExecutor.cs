// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;

    public sealed class EnsembleExecutor(SingleModelExecutor singleModelExecutor) : IEnsembleExecutor
    {
        private readonly SingleModelExecutor singleModelExecutor = singleModelExecutor;

        public async Task<IReadOnlyList<ModelResponse>> ExecuteAsync(ClientRequestBuilder requestBuilder, IExecutionContext context, CancellationToken cancelToken)
        {
            IEnumerable<Task<ModelResponse>> tasks = context.Models.Select(model =>
            {
                // Override request with ensemble model ..
                ChatCompletionRequest chatRequest = requestBuilder
                    .WithModel(model.Name)
                    .Build();

                // Execute the request on this model ..
                return this.singleModelExecutor.ExecuteAsync(request: chatRequest, cancelToken: cancelToken);
            });

            ModelResponse[] responses = await Task.WhenAll(tasks);
            return responses;
        }
    }
}
