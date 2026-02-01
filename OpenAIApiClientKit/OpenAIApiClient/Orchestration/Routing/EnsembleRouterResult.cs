// <copyright file="EnsembleRouterResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Models.Registries;

    public sealed class EnsembleRouterResult(IEnumerable<ModelDescriptor> models)
    {
        /// <summary>
        /// Gets a unique list of model descriptors selected for the ensemble.
        /// </summary>
        public IReadOnlyList<ModelDescriptor> Models { get; } = [.. models];
    }
}