// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW03
{
    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<ModelResponse>> ExecuteAsync(IExecutionContext context, CancellationToken cancelToken);
    }
}
