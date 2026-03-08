// <copyright file="EnsembleDispatchStrategyHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Delegates
{
    using System.Collections.Generic;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Represents a strategy function that selects models from the available registry.
    /// </summary>
    /// <param name="availableModels">
    /// The collection of available models to select from.
    /// </param>
    /// <returns>
    /// An <see cref="EnsembleDispatchResult"/> describing the selected models.
    /// </returns>
    public delegate EnsembleDispatchResult EnsembleDispatchStrategyHandler(IEnumerable<AiModelDescriptor> availableModels);
}