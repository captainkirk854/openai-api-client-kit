// <copyright file="EnsembleRoutingStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Routing;

    /// <summary>
    /// Ensemble routing strategy handler delegate which, when implemented, builds an ensemble of models.
    /// </summary>
    /// <param name="modelRegistry"></param>
    /// <returns cref="EnsembleRouterResult">RouterResult.</returns>
    public delegate EnsembleRouterResult EnsembleRoutingStrategyHandler(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry);
}