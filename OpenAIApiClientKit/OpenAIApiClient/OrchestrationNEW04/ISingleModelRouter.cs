// <copyright file="ISingleModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Routing.SingleModel;

    public interface ISingleModelRouter
    {
        /// <summary>
        /// Routes a single-model request to the appropriate model descriptor
        /// based on the routing strategy and registry configuration.
        /// </summary>
        /// <param name="request">The routing request containing strategy and constraints.</param>
        /// <returns>A result containing the selected model descriptor.</returns>
        SingleModelRouterResult Route(SingleModelRouterRequest request);
    }
}
