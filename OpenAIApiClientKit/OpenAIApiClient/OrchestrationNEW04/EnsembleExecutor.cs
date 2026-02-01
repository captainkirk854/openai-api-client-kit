// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    public sealed class EnsembleExecutor(SingleModelExecutor singleModelExecutor) : IEnsembleExecutor
    {
        private readonly SingleModelExecutor singleModelExecutor = singleModelExecutor;

        public async Task<IReadOnlyList<ModelResponse>> ExecuteAsync(OrchestrationContext context, CancellationToken cancelToken)
        {
            Task<ModelResponse>[] tasks = [.. context.ExecutionContext.Models.Select(model => this.singleModelExecutor.ExecuteAsync(model, context.PromptContext, cancelToken))];

            return await Task.WhenAll(tasks);
        }
    }
}
