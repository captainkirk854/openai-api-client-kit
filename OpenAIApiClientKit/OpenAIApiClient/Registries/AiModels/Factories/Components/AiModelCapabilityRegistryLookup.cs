// <copyright file="AiModelCapabilityRegistryLookup.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories.Components
{
    using OpenAIApiClient.Models.Registries.AiModels;

    internal sealed class AiModelCapabilityRegistryLookup(AiModelCapabilityRegistryData mergedRegistry)
    {
        private readonly Dictionary<string, AiModelCapabilityRegistryModel> byName = mergedRegistry
                .Models
                .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                .GroupBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Last(),   // last wins if duplicates
                    StringComparer.OrdinalIgnoreCase);

        public bool TryGetByName(string apiModelName, out AiModelCapabilityRegistryModel? modelEntry)
        {
            if (this.byName.TryGetValue(apiModelName, out AiModelCapabilityRegistryModel? value))
            {
                modelEntry = value;
                return true;
            }

            modelEntry = null;
            return false;
        }
    }
}
