// <copyright file="SingleModelRoutingStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Routing;

    public delegate SingleModelRouterResult SingleModelRoutingStrategyHandler(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelRouterRequest request);
}