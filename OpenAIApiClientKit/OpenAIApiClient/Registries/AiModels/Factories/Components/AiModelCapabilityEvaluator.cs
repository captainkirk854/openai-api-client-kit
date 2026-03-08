// <copyright file="AiModelCapabilityEvaluator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories.Components
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Evaluates and determines the capabilities of AI models based on their descriptor and capability scores.
    /// </summary>
    public sealed class AiModelCapabilityEvaluator : IAiModelCapabilityEvaluator
    {
        public IReadOnlySet<AiModelCapability> GetCapabilities(AiModelDescriptor model)
        {
            // Initalise a set to hold the capabilities of the model ..
            HashSet<AiModelCapability> result = [];
            AiModelCapabilityScores capabilityScores = model.Capabilities;

            // Iterate through all possible enum values
            foreach (AiModelCapability capability in Enum.GetValues(typeof(AiModelCapability)))
            {
                AddCapabilityOnGoodScore(capability: capability, capabilityScores: capabilityScores, result: result);
            }

            return result;
        }

        /// <summary>
        /// Adds the specified capability to the result set if its score meets the required threshold.
        /// </summary>
        /// <param name="capability">The capability to evaluate.</param>
        /// <param name="capabilityScores">The set of scores for each capability.</param>
        /// <param name="result">The collection to which the capability is added if the score is sufficient.</param>
        private static void AddCapabilityOnGoodScore(AiModelCapability capability, AiModelCapabilityScores capabilityScores, HashSet<AiModelCapability> result)
        {
            // Map the capability to its corresponding score using a switch expression.
            int score = capability switch
            {
                // Core
                AiModelCapability.Reasoning => capabilityScores.Core.Reasoning,
                AiModelCapability.Text => capabilityScores.Core.Text,
                AiModelCapability.Chat => capabilityScores.Core.Chat,
                AiModelCapability.Vision => capabilityScores.Core.Vision,
                AiModelCapability.AudioIn => capabilityScores.Core.AudioIn,
                AiModelCapability.AudioOut => capabilityScores.Core.AudioOut,

                // Advanced
                AiModelCapability.Critic => capabilityScores.Advanced.Critic,
                AiModelCapability.Editor => capabilityScores.Advanced.Editor,
                AiModelCapability.JSONMode => capabilityScores.Advanced.JSONMode,
                AiModelCapability.Embedding => capabilityScores.Advanced.Embedding,
                AiModelCapability.ImageGeneration => capabilityScores.Advanced.ImageGeneration,

                // Performance
                AiModelCapability.FastInference => capabilityScores.Performance.FastInference,
                AiModelCapability.HighPerformance => capabilityScores.Performance.HighPerformance,

                // Operational
                AiModelCapability.LowCost => capabilityScores.Operational.LowCost,
                AiModelCapability.Moderation => capabilityScores.Operational.Moderation,
                AiModelCapability.OpenWeight => capabilityScores.Operational.OpenWeight,

                _ => 0,
            };

            if (HasPassedScoreThresholdToAddCapability(score))
            {
                result.Add(capability);
            }
        }

        /// <summary>
        /// Determines whether the specified score meets or exceeds the threshold required to add a capability.
        /// !!NEED TO HAVE A RANGE!! - WHERE SCORES CAN BE BETWEEN 0 AND 5 IN ORDER TO BE CONSIDERED FOR A CAPABILITY - ALLOWS US TO FILTER OUT HIGH SCORING MODELS IF WE NEED TO.
        /// </summary>
        /// <remarks>
        /// Adjust threshold logic here if you want stricter criteria.
        /// </remarks>
        /// <param name="score">The score to evaluate against the threshold.</param>
        /// <returns>true if the score is greater than or equal to 1; otherwise, false.</returns>
        private static bool HasPassedScoreThresholdToAddCapability(int score) => score >= 3;
    }
}
