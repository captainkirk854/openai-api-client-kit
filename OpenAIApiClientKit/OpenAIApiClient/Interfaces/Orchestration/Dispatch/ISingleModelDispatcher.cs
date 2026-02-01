// <copyright file="ISingleModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Dispatch
{
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// <see cref="ISingleModelDispatcher"/> dispatches a request to select a model descriptor based on the strategy and registry configuration.
    /// </summary>
    public interface ISingleModelDispatcher
    {
        /// <summary>
        /// Evaluates request to select an appropriate model descriptor.
        /// </summary>
        /// <param name="request">The single model dispatch request containing strategy and constraints.</param>
        /// <returns see cref="SingleModelDispatchResult">Selected model descriptor.</returns>
        SingleModelDispatchResult Evaluate(SingleModelDispatchRequest request);
    }
}
