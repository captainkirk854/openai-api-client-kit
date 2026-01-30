// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<ModelResponse>> ExecuteAsync(OrchestrationContext context, CancellationToken cancelToken);
    }
}
