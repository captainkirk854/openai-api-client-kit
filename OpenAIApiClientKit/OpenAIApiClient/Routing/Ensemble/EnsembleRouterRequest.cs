// <copyright file="EnsembleRouterRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Ensemble
{
    using OpenAIApiClient.Enums;

    public sealed class EnsembleRouterRequest
    {
        /// <summary>
        /// Gets the ensemble routing strategy used.
        /// </summary>
        public EnsembleRoutingStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the desired number of models to route to.
        /// </summary>
        public int? ModelCount
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the collection of required model capabilities.
        /// </summary>
        public IReadOnlyCollection<ModelCapability>? RequiredCapabilities
        {
            get;
            init;
        }
    }
}
