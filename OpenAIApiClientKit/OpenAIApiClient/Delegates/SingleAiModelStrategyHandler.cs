// <copyright file="SingleAiModelStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;

    public delegate SingleAiModelDispatchResult SingleAiModelStrategyHandler(IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry, SingleAiModelDispatchRequest request);
}