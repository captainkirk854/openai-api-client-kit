// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Defines a contract for executing ensemble AI model requests asynchronously and returning responses.
    /// </summary>
    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ChatClientRequestBuilder requestBuilder, IExecutionContext context, AiCallOptions execution, CancellationToken cancelToken);
    }
}
