// <copyright file="SingleModelStrategy.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums.Routing
{
    public enum SingleModelStrategy
    {
        LowestCost,
        HighestPerformance,
        BestReasoning,
        BestVision,
        BestAudioIn,
        BestAudioOut,
        Embedding,
        Moderation,
        Explicit,
    }
}