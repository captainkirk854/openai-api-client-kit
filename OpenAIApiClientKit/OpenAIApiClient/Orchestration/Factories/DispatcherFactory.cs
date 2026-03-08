// <copyright file="DispatcherFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Provides factory methods for creating dispatcher instances using a model registry.
    /// </summary>
    public static class DispatcherFactory
    {
        /// <summary>
        /// Creates instances of SingleModelDispatcher and EnsembleDispatcher using the provided model registry.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns see cref="(SingleAiModelDispatcher, EnsembleDispatcher)">Tuple containing the created dispatchers.</returns>
        public static (SingleAiModelDispatcher, EnsembleDispatcher) Create(IAiModelRegistry registry) => (new SingleAiModelDispatcher(registry: registry), new EnsembleDispatcher(modelRegistry: registry));
    }
}