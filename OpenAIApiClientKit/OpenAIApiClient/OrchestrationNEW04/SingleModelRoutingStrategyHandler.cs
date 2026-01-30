// <copyright file="SingleModelRoutingStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.SingleModel;

    public delegate SingleModelRouterResult SingleModelRoutingStrategyHandlerNEW(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, ModelRouterRequest request);
}