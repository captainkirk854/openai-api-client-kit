// <copyright file="EnsembleDispatchResultNEW.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using System;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Represents the result of applying an ensemble strategy over a set of models.
    /// </summary>
    public sealed class EnsembleDispatchResultNEW
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnsembleDispatchResultNEW"/> class.
        /// </summary>
        /// <param name="models">The models selected by the ensemble strategy.</param>
        public EnsembleDispatchResultNEW(IEnumerable<AiModelPropertyRegistryModel> models)
        {
            ArgumentNullException.ThrowIfNull(models);
            this.Models = [.. models];
        }

        /// <summary>
        /// Gets a list of models selected for the ensemble.
        /// </summary>
        public IReadOnlyList<AiModelPropertyRegistryModel> Models
        {
            get;
        }
    }
}
