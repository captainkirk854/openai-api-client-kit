// <copyright file="EnsembleStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Ensemble routing strategy handler delegate which, when implemented, builds an ensemble of models.
    /// </summary>
    /// <param name="modelRegistry"></param>
    /// <returns cref="EnsembleDispatchResult">RouterResult.</returns>
    public delegate EnsembleDispatchResult EnsembleStrategyHandler(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry);
}