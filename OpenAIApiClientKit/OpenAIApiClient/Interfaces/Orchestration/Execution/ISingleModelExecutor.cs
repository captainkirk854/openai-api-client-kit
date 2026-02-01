// <copyright file="ISingleModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration;

    public interface ISingleModelExecutor
    {
        Task<ModelResponse> ExecuteAsync(ModelDescriptor model, PromptContext context, CancellationToken cancelToken);
    }
}