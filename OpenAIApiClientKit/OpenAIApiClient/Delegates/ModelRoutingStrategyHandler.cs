// <copyright file="ModelRoutingStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Individual;

    public delegate ModelRouterResult ModelRoutingStrategyHandler(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, ModelRouterRequest request);
}