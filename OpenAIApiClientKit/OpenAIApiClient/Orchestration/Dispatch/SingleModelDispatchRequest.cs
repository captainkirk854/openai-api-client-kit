// <copyright file="SingleModelDispatchRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;

    /// <summary>
    /// Defines a request to the single model router for selecting an appropriate model based on the specified criteria.
    /// </summary>
    public sealed class SingleModelDispatchRequest
    {
        /// <summary>
        /// Gets the strategy to use (e.g., BestReasoning, LowestCost, Explicit).
        /// </summary>
        public SingleModelStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional explicit model override (used only for <see cref="SingleModelStrategy.Explicit"/> strategy).
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