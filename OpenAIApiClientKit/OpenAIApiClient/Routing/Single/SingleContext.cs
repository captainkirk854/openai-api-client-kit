// <copyright file="SingleContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Single
{
    using OpenAIApiClient.Enums;

    public sealed class SingleContext
    {
        /// <summary>
        /// Gets the model routing strategy.
        /// </summary>
        public ModelRoutingStrategy Strategy
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the explicitly specified model, if any.
        /// </summary>
        public OpenAIModel? ExplicitModel
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the required model capabilities.
        /// </summary>
        public IReadOnlyCollection<ModelCapability>? RequiredCapabilities
        {
            get;
            init;
        }
    }
}
