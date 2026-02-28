// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ChatClientRequestBuilder requestBuilder, IExecutionContext context, AiCallOptions execution, CancellationToken cancelToken);
    }
}
