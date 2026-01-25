// <copyright file="EnsembleRouterResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Ensemble
{
    using OpenAIApiClient.Models.Registries;

    public sealed class EnsembleRouterResult(IEnumerable<ModelDescriptor> models)
    {
        public IReadOnlyList<ModelDescriptor> Models { get; } = [.. models];
    }
}