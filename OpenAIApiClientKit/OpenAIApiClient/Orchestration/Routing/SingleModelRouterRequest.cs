// <copyright file="SingleModelRouterRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Enums.Routing;

    /// <summary>
    /// Defines a request to the single model router for selecting an appropriate model based on the specified criteria.
    /// </summary>
    public sealed class SingleModelRouterRequest
    {
        /// <summary>
        /// Gets the routing strategy to use (e.g., BestReasoning, LowestCost, Explicit).
        /// </summary>
        public SingleModelStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional explicit model override (used only when <cref="ModelRoutingStrategy"/> = Explicit).
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