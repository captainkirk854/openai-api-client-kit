// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;

    public sealed class EnsembleExecutor(ISingleAiModelExecutor singleModelExecutor) : IEnsembleExecutor
    {
        private readonly ISingleAiModelExecutor singleModelExecutor = singleModelExecutor;

        public async Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ClientRequestBuilder requestBuilder, IExecutionContext context, CancellationToken cancelToken)
        {
            IEnumerable<Task<AiModelResponse>> tasks = context.Models.Select(model =>
            {
                // Build the request by overriding the model in the request builder with the current model for this iteration of the loop ..
                ChatCompletionRequest chatRequest = requestBuilder
                    .WithModel(model.Name)
                    .Build();

                // Execute the request on this model ..
                return this.singleModelExecutor.ExecuteAsync(request: chatRequest, cancelToken: cancelToken);
            });

            AiModelResponse[] responses = await Task.WhenAll(tasks);
            return responses;
        }
    }
}
