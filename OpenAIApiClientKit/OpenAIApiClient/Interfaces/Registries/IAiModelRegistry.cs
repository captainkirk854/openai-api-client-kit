// <copyright file="IAiModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;

    public interface IAiModelRegistry
    {
        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        /// <returns see cref="Dictionary(string, AiModelDescriptor)">.</returns>
        public Dictionary<string, AiModelDescriptor> GetRegistry();

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        /// <returns see cref="IEnumerable(AiModelDescriptor)">.</returns>
        IEnumerable<AiModelDescriptor> GetAll();

        /// <summary>
        /// Gets the model descriptor for a specified model by its unique name, or null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        AiModelDescriptor? GetByName(string name);

        /// <summary>
        /// Returns all models that satisfy a capability predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns see cref="IEnumerable(AiModelDescriptor)">.</returns>
        IEnumerable<AiModelDescriptor> Find(Func<AiModelDescriptor, bool> predicate);
    }
}
