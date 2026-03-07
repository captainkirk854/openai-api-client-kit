// <copyright file="OpenAIModelsFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories
{
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels.Factories.Components;
    using OpenAIApiClient.Registries.AiModels.Sources;

    public static class OpenAIModelsFactory
    {
        public static OpenAIModelsNEW Create()
        {
            // Load and merge embedded capability registry JSON
            IAiModelCapabilityRegistrySource embeddedSource = new EmbeddedAiModelPropertyRegistrySource();
            AiModelPropertyRegistryLoader loader = new(embeddedSource);
            AiModelPropertyRegistryData mergedData = loader.LoadMerged();

            // Create lookup and evaluator
            AiModelPropertyRegistryLookup lookup = new(mergedData);
            AiModelCapabilityEvaluator evaluator = new();

            // Build descriptor dictionary
            Dictionary<string, AiModelPropertyRegistryModel> descriptors = mergedData.ToUpperNameDictionary();

            // Construct and return registry
            return new OpenAIModelsNEW(descriptors);
        }

        /// <summary>
        /// Builds a dictionary keyed by upper-case model name from the supplied registry data.
        /// </summary>
        /// <param name="registryData">The registry data containing models.</param>
        /// <returns>
        /// A dictionary whose keys are upper-case model names and whose values are the corresponding
        /// <see cref="AiModelPropertyRegistryModel"/> instances.
        /// </returns>
        public static Dictionary<string, AiModelPropertyRegistryModel> ToUpperNameDictionary(this AiModelPropertyRegistryData registryData)
        {
            ArgumentNullException.ThrowIfNull(registryData);

            if (registryData.Models is null)
            {
                throw new InvalidOperationException("Registry data must contain a non-null Models collection.");
            }

            Dictionary<string, AiModelPropertyRegistryModel> dictionary = registryData.Models.ToDictionary(keySelector: model => model.UpperName, elementSelector: model => model, comparer: StringComparer.OrdinalIgnoreCase);

            return dictionary;
        }
    }
}