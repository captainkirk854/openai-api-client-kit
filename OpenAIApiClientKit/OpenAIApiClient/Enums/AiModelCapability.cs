// <copyright file="AiModelCapability.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum AiModelCapability
    {
        // Core cognitive & interaction capabilities ..
        AudioIn,
        AudioOut,
        Chat,
        Reasoning,
        Text,
        Vision,

        // Advanced capabilities ..
        Critic,
        Editor,
        Embedding,
        ImageGeneration,
        JSONMode,

        // Performance capabilities ..
        FastInference,
        HighPerformance,

        // Operational capabilities ..
        LowCost,
        Moderation,
        OpenWeight,
    }
}