// <copyright file="OpenAIModelsFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories
{
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels.Factories.Components;
    using OpenAIApiClient.Registries.AiModels.Sources;

    /// <summary>
    /// Provides factory methods for creating and managing OpenAI model registries and descriptors.
    /// </summary>
    public static class OpenAIModelsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="OpenAIModels"/> by loading and merging model descriptors from embedded JSON resources.
        /// </summary>
        /// <returns>An instance of <see cref="OpenAIModels"/> containing the merged model descriptors.</returns>
        public static OpenAIModels Create()
        {
            // Load and merge embedded capability registry JSON ..
            IAiModelCapabilityRegistryResource embeddedResource = new EmbeddedAiModelPropertyRegistryResource();
            AiModelPropertyRegistryLoader loader = new(embeddedResource);
            AiModelPropertyRegistryData mergedData = loader.LoadMerged();

            // Build model descriptor dictionary ..
            Dictionary<string, AiModelDescriptor> descriptors = mergedData.ToUpperNameDictionary();

            // Construct and return registry ..
            return new OpenAIModels(descriptors);
        }
    }
}