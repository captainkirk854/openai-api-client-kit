// <copyright file="ISingleModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Models.Registries;

    public interface ISingleModelExecutor
    {
        Task<string> ExecuteAsync(ModelDescriptor model, string prompt);
    }
}