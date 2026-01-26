// <copyright file="SingleRoutingStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Single;

    public delegate SingleRouterResult SingleRoutingStrategyHandler(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleContext request);
}