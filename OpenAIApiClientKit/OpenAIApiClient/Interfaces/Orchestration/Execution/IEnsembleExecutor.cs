// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Orchestration.Response;

    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ClientRequestBuilder requestBuilder, IExecutionContext context, CancellationToken cancelToken);
    }
}
