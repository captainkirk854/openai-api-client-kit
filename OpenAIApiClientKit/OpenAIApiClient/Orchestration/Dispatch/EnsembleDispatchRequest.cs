// <copyright file="EnsembleDispatchRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;

    /// <summary>
    /// <see cref="EnsembleDispatchRequest"/> Defines a request to the dispatcher for selection of models based on the specified criteria.
    /// </summary>
    public sealed class EnsembleDispatchRequest
    {
        /// <summary>
        /// Gets the ensemble strategy (e.g., Reasoning, Vision, CostOptimized).
        /// </summary>
        public EnsembleStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional explicit list of models (used only for <see cref="EnsembleStrategy.Custom"/> strategy).
        /// </summary>
        public IReadOnlyList<string>? ExplicitModels
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional desired number of model(s) to route to.
        /// </summary>
        public int? ModelCount
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional required capabilities for all models in the ensemble.
        /// </summary>
        public IReadOnlyList<AiModelCapability>? RequiredCapabilities
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
        /// Gets the Optional per-model weights (used for weighted ensembles).
        /// </summary>
        public IReadOnlyDictionary<string, double>? Weights
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
