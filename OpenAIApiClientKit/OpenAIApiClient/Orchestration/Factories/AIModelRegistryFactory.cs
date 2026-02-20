// <copyright file="AIModelRegistryFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Registries.Models;

    /// <summary>
    /// Provides factory methods for creating ai model registry instances.
    /// </summary>
    public static class AIModelRegistryFactory
    {
        /// <summary>
        /// Creates a new instance of the default model registry.
        /// </summary>
        /// <returns>An <see cref="IAIModelRegistry"/> representing the default model registry.</returns>
        public static IAIModelRegistry Create() => new OpenAIModels();
    }
}