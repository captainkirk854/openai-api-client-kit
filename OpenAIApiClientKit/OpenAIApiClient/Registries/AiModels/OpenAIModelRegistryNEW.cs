// <copyright file="OpenAIModelRegistryNEW.cs" company="854 Things (tm)">
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
    /// Provides a string-based registry over OpenAI models using <see cref="AiModelPropertyRegistryModel"/>.
    /// </summary>
    /// <remarks>
    /// This is the new, enum-free model registry used by orchestration. It is keyed by the
    /// <see cref="AiModelPropertyRegistryModel.Name"/> property (for example, gpt-4o-mini).
    /// </remarks>
    public sealed class OpenAIModelRegistryNEW : IAiModelRegistryNEW
    {
        private readonly IReadOnlyDictionary<string, AiModelPropertyRegistryModel> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModelRegistryNEW"/> class.
        /// </summary>
        /// <remarks>
        /// The registry is constructed once and then treated as immutable. Models are keyed
        /// by their <see cref="AiModelPropertyRegistryModel.Name"/> using a case-insensitive
        /// comparer. The data is loaded from the embedded JSON/PowerShell-generated registry
        /// via <see cref="EmbeddedAiModelPropertyRegistrySource"/>.
        /// </remarks>
        public OpenAIModelRegistryNEW()
        {
            EmbeddedAiModelPropertyRegistrySource source = new EmbeddedAiModelPropertyRegistrySource();
            EmbeddedAiModelPropertyRegistryLoader loader = new EmbeddedAiModelPropertyRegistryLoader(source);

            IReadOnlyCollection<AiModelPropertyRegistryModel> loadedModels = loader.Load()
                ?? throw new InvalidOperationException(
                    "EmbeddedAiModelPropertyRegistryLoader.Load() returned null.");

            Dictionary<string, AiModelPropertyRegistryModel> dictionary =
                loadedModels.ToDictionary(
                    keySelector: m => m.UpperName,
                    elementSelector: m => m,
                    comparer: StringComparer.OrdinalIgnoreCase);

            this.models = new ReadOnlyDictionary<string, AiModelPropertyRegistryModel>(dictionary);
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, AiModelPropertyRegistryModel> GetRegistry()
        {
            // Return a defensive copy to ensure callers cannot mutate internal state.
            return this.models;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<AiModelPropertyRegistryModel> GetAll()
        {
            // Return a defensive copy to ensure callers cannot mutate internal state.
            return this.models.Values.ToArray();
        }

        /// <inheritdoc/>
        public AiModelPropertyRegistryModel? TryGetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (this.models.TryGetValue(name, out AiModelPropertyRegistryModel? model))
            {
                return model;
            }

            return null;
        }
    }
}
