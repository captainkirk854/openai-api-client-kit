// <copyright file="EnsembleStrategy.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Ensemble;

    public delegate EnsembleRouterResult EnsembleStrategy(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry);
}