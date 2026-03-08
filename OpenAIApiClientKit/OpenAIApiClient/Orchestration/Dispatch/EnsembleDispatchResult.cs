// <copyright file="EnsembleDispatchResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using System;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Represents the result of applying an dispatch strategy over an ensemble of models.
    /// </summary>
    public sealed class EnsembleDispatchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnsembleDispatchResult"/> class.
        /// </summary>
        /// <param name="models">The models selected by the ensemble strategy.</param>
        public EnsembleDispatchResult(IEnumerable<AiModelDescriptor> models)
        {
            ArgumentNullException.ThrowIfNull(models);
            this.Models = [.. models];
        }

        /// <summary>
        /// Gets a list of models selected for the ensemble.
        /// </summary>
        public IReadOnlyList<AiModelDescriptor> Models
        {
            get;
        }
    }
}
