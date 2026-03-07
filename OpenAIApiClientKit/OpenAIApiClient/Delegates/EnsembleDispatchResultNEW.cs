// <copyright file="EnsembleDispatchResultNEW.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using System.Collections.Generic;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// Represents a strategy function that selects models from the available registry.
    /// </summary>
    /// <param name="availableModels">
    /// The collection of available models to select from.
    /// </param>
    /// <returns>
    /// An <see cref="EnsembleDispatchResultNEW"/> describing the selected models.
    /// </returns>
    public delegate EnsembleDispatchResultNEW EnsembleStrategyHandlerNEW(IEnumerable<AiModelPropertyRegistryModel> availableModels);
}
