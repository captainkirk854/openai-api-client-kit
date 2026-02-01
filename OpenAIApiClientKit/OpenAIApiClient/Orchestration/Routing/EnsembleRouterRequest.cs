// <copyright file="EnsembleRouterRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Enums;

    /// <summary>
    /// Defines a request to the ensemble router for selecting appropriate models based on the specified criteria.
    /// </summary>
    public sealed class EnsembleRouterRequest
    {
        /// <summary>
        /// Gets the ensemble routing strategy (e.g., Reasoning, Vision, CostOptimized).
        /// </summary>
        public EnsembleRoutingStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the Optional explicit list of models (used only for ExplicitEnsemble strategy).
        /// </summary>
        public IReadOnlyList<OpenAIModel>? ExplicitModels
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
        /// Gets the Optional per-model weights (used for weighted ensembles).
        /// </summary>
        public IReadOnlyDictionary<OpenAIModel, double>? Weights
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
