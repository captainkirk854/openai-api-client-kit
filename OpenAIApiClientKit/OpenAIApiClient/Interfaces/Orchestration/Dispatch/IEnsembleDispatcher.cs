// <copyright file="IEnsembleDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Dispatch
{
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// <see cref="IEnsembleDispatcher"/> dispatches an ensemble request to evaluate the set of model descriptors based on the ensemble strategy and registry configuration.
    /// </summary>
    public interface IEnsembleDispatcher
    {
        /// <summary>
        /// Evaluates a request to select the appropriate set of model descriptor(s).
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing strategy and constraints.</param>
        /// <returns see cref="EnsembleDispatchResult">Selected model descriptor(s).</returns>
        EnsembleDispatchResult Evaluate(EnsembleDispatchRequest request);
    }
}