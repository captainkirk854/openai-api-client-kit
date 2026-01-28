// <copyright file="IEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Models.Registries;

    public interface IEnsembleExecutor
    {
        Task<IReadOnlyList<string>> ExecuteAsync(IReadOnlyList<ModelDescriptor> models, string prompt);
    }
}