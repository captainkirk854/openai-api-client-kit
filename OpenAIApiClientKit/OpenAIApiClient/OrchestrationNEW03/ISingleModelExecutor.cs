// <copyright file="ISingleModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW03
{
    using OpenAIApiClient.Models.Registries;

    public interface ISingleModelExecutor
    {
        Task<ModelResponse> ExecuteAsync(ModelDescriptor model, PromptContext promptContext, CancellationToken cancelToken);
    }
}