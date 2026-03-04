// <copyright file="AiModelCapability.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum AiModelCapability
    {
        // Core cognitive & interaction capabilities ..
        Reasoning,
        Text,
        Chat,
        Vision,
        AudioIn,
        AudioOut,

        // Advanced capabilities ..
        Critic,
        Editor,
        JSONMode,
        Embedding,
        ImageGeneration,

        // Performance capabilities ..
        FastInference,
        HighPerformance,

        // Operational capabilities ..
        LowCost,
        Moderation,
        OpenWeight,
    }
}