// <copyright file="IAiModelCapabilityEvaluator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;

    public interface IAiModelCapabilityEvaluator
    {
        IReadOnlySet<AiModelCapability> GetCapabilities(AiModelCapabilityRegistryModel modelEntry);
    }
}