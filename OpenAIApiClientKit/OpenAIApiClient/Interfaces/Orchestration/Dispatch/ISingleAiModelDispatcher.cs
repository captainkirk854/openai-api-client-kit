// <copyright file="ISingleAiModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Dispatch
{
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// <see cref="ISingleAiModelDispatcher"/> dispatches a request to select a model descriptor based on the strategy and registry configuration.
    /// </summary>
    public interface ISingleAiModelDispatcher
    {
        /// <summary>
        /// Evaluates request to select an appropriate model descriptor.
        /// </summary>
        /// <param name="request">The single model dispatch request containing strategy and constraints.</param>
        /// <returns see cref="SingleAiModelDispatchResult">Selected model descriptor.</returns>
        SingleAiModelDispatchResult Evaluate(SingleAiModelDispatchRequest request);
    }
}
