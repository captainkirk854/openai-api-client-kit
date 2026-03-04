// <copyright file="EnsembleDispatchResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// <see cref="EnsembleDispatchRequest"/> represents the result of an ensemble dispatch operation, containing the selected model(s).
    /// </summary>
    /// <param name="models"></param>
    public sealed class EnsembleDispatchResult(IEnumerable<AiModelDescriptor> models)
    {
        /// <summary>
        /// Gets a list of model descriptors selected for the ensemble.
        /// </summary>
        public IReadOnlyList<AiModelDescriptor> Models { get; } = [.. models];
    }
}