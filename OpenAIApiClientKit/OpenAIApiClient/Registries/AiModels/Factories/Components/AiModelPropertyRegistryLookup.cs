// <copyright file="AiModelPropertyRegistryLookup.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories.Components
{
    using OpenAIApiClient.Models.Registries.AiModels;

    internal sealed class AiModelPropertyRegistryLookup(AiModelPropertyRegistryData mergedRegistry)
    {
        /// <summary>
        /// Maps model names to their corresponding capability registry models, using case-insensitive comparison.
        /// </summary>
        /// <remarks>If multiple models share the same name, the last one encountered is used.</remarks>
        private readonly Dictionary<string, AiModelPropertyRegistryModel> byName = mergedRegistry
                .Models
                .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                .GroupBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Last(),   // last wins if duplicates
                    StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Attempts to retrieve a model entry from the registry by its API model name.
        /// </summary>
        /// <param name="apiModelName">The API model name to search for.</param>
        /// <param name="modelEntry">When this method returns, contains the model entry associated with the specified name, if found; otherwise,
        /// null.</param>
        /// <returns>true if a model entry with the specified name is found; otherwise, false.</returns>
        public bool TryGetByName(string apiModelName, out AiModelPropertyRegistryModel? modelEntry)
        {
            if (this.byName.TryGetValue(apiModelName, out AiModelPropertyRegistryModel? value))
            {
                modelEntry = value;
                return true;
            }

            modelEntry = null;
            return false;
        }
    }
}