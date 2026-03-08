// <copyright file="OpenAIModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels.Sources;

    /// <summary>
    /// Provides a registry of OpenAI model descriptors loaded from embedded resources.
    /// </summary>
    /// <remarks>
    /// The registry is constructed once and then treated as immutable. Models are keyed by their
    /// <see cref="AiModelDescriptor.Name"/> using a case-insensitive comparer. The data is loaded from the embedded
    /// JSON/PowerShell-generated registry via <see cref="EmbeddedAiModelPropertyRegistryResource"/>.
    /// </remarks>
    public sealed class OpenAIModelRegistry : IAiModelRegistry
    {
        private readonly IReadOnlyDictionary<string, AiModelDescriptor> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModelRegistry"/> class.
        /// </summary>
        /// <remarks>
        /// The registry is constructed once and then treated as immutable. Models are keyed
        /// by their <see cref="AiModelDescriptor.Name"/> using a case-insensitive
        /// comparer. The data is loaded from the embedded JSON/PowerShell-generated registry
        /// via <see cref="EmbeddedAiModelPropertyRegistryResource"/>.
        /// </remarks>
        public OpenAIModelRegistry()
        {
            EmbeddedAiModelPropertyRegistryResource resource = new();
            EmbeddedAiModelPropertyRegistryLoader loader = new(resource: resource);

            IReadOnlyCollection<AiModelDescriptor> loadedModels = loader.Load()
                ?? throw new InvalidOperationException("EmbeddedAiModelPropertyRegistryLoader.Load() returned null.");

            Dictionary<string, AiModelDescriptor> dictionary =
                loadedModels.ToDictionary(keySelector: m => m.UpperName,
                                          elementSelector: m => m,
                                          comparer: StringComparer.OrdinalIgnoreCase);

            this.models = new ReadOnlyDictionary<string, AiModelDescriptor>(dictionary);
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, AiModelDescriptor> GetRegistry()
        {
            // Return a defensive copy to ensure callers cannot mutate internal state.
            return this.models;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<AiModelDescriptor> GetAll()
        {
            // Return a defensive copy to ensure callers cannot mutate internal state.
            return [.. this.models.Values];
        }

        /// <inheritdoc/>
        public AiModelDescriptor? TryGetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (this.models.TryGetValue(name, out AiModelDescriptor? model))
            {
                return model;
            }

            return null;
        }

        /// <summary>
        /// Determines whether a model with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the model to check for presence.</param>
        /// <returns>true if the model exists; otherwise, false.</returns>
        public bool IsExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            return this.models.ContainsKey(name);
        }
    }
}