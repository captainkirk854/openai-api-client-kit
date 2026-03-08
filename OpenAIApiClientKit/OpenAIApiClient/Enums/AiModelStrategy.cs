// <copyright file="AiModelStrategy.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public class AiModelStrategy
    {
        /// <summary>
        /// Specifies the types of ensemble configurations available for model selection.
        /// </summary>
        public enum Ensemble
        {
            Custom,
            None,
            TwoModel,
            ThreeModel,
            Reasoning,
            Vision,
            CostOptimized,
        }

        /// <summary>
        /// Specifies available single AI model types for various tasks such as cost, performance, reasoning, vision,
        /// audio, embedding, moderation, and content handling.
        /// </summary>
        public enum SingleAiModel
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
}
