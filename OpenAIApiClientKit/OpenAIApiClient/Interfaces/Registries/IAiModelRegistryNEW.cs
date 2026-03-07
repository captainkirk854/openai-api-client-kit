// <copyright file="IAiModelRegistryNEW.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Provides lookup and enumeration over AI models by string identifier.
    /// </summary>
    public interface IAiModelRegistryNEW
    {
        /// <summary>
        /// Retrieves the registry of AI model property definitions.
        /// </summary>
        /// <returns type="IReadOnlyDictionary{string, AiModelPropertyRegistryModel}">A read-only dictionary mapping model names to their property definitions.</returns>
        IReadOnlyDictionary<string, AiModelPropertyRegistryModel> GetRegistry();

        /// <summary>
        /// Gets all models in the registry.
        /// </summary>
        /// <returns type="IReadOnlyCollection{AiModelPropertyRegistryModel}">A read-only collection of all registered AI models.</returns>
        IReadOnlyCollection<AiModelPropertyRegistryModel> GetAll();

        /// <summary>
        /// Attempts to get a model by its canonical name (for example, gpt-4o-mini).
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <returns>
        /// The <see cref="AiModelPropertyRegistryModel"/> if found; otherwise, <see langword="null"/>.
        /// </returns>
        AiModelPropertyRegistryModel? TryGetByName(string name);
    }
}
