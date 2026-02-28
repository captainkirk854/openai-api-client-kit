// <copyright file="ISingleAiModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    public interface ISingleAiModelExecutor
    {
        Task<AiModelResponse> ExecuteAsync(ChatCompletionRequest request, AiCallOptions execution, CancellationToken cancelToken);
    }
}