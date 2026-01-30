// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW03
{
    public sealed class EnsembleExecutor(ISingleModelExecutor executor) : IEnsembleExecutor
    {
        private readonly ISingleModelExecutor executor = executor;

        public async Task<IReadOnlyList<ModelResponse>> ExecuteAsync(IExecutionContext context, CancellationToken cancelToken)
        {
            PromptContext promptContext = context.BuildPromptContext();

            Task<ModelResponse>[] tasks = [.. context.Models.Select(model => this.executor.ExecuteAsync(model, promptContext, cancelToken))];

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
