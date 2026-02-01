// <copyright file="IEnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Routing.Ensemble;

    public interface IEnsembleRouter
    {
        /// <summary>
        /// Routes an ensemble request to the appropriate set of model descriptors
        /// based on the ensemble routing strategy and registry configuration.
        /// </summary>
        /// <param name="request">The ensemble routing request.</param>
        /// <returns>A result containing the selected model descriptors.</returns>
        EnsembleRouterResult Route(EnsembleRouterRequest request);
    }
}