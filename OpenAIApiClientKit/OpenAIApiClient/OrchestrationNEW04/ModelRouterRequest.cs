// <copyright file="ModelRouterRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Enums;

    public sealed class ModelRouterRequest
    {
        /// <summary>
        /// Gets the routing strategy to use (e.g., BestReasoning, LowestCost, Explicit).
        /// </summary>
        public ModelRoutingStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional explicit model override (used only when Strategy = Explicit).
        /// </summary>
        public OpenAIModel? ExplicitModel
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional required capabilities (e.g., Vision, AudioIn, Reasoning).
        /// </summary>
        public IReadOnlyList<ModelCapability>? RequiredCapabilities
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the maximum acceptable latency for processing this prompt.
        /// </summary>
        public TimeSpan? MaxLatency
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the maximum acceptable cost for processing this prompt.
        /// </summary>
        public decimal? MaxCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional metadata for future extensibility (temperature, max tokens, etc.).
        /// </summary>
        public Dictionary<string, object>? Metadata
        {
            get;
            init;
        }
    }
}