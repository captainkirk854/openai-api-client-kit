// <copyright file="IAiModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Defines a registry for AI model property definitions, allowing retrieval of model information by name and access to all registered models.
    /// </summary>
    public interface IAiModelRegistry
    {
        /// <summary>
        /// Retrieves the registry of AI model property definitions.
        /// </summary>
        /// <returns type="IReadOnlyDictionary{string, AiModelPropertyRegistryModel}">A read-only dictionary mapping model names to their property definitions.</returns>
        IReadOnlyDictionary<string, AiModelDescriptor> GetRegistry();

        /// <summary>
        /// Gets all models in the registry.
        /// </summary>
        /// <returns type="IReadOnlyCollection{AiModelPropertyRegistryModel}">A read-only collection of all registered AI models.</returns>
        IReadOnlyCollection<AiModelDescriptor> GetAll();

        /// <summary>
        /// Attempts to get a model by its canonical name (for example, gpt-4o-mini).
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <returns>
        /// The <see cref="AiModelDescriptor"/> if found; otherwise, <see langword="null"/>.
        /// </returns>
        AiModelDescriptor? TryGetByName(string name);
    }
}
