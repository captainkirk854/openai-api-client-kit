// <copyright file="OpenAIModelsFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories
{
    using OpenAIApiClient.Enums;
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
            //foreach (string model in Enum.GetValues(typeof(string)))
            //{
            //    // Initialise with empty capabilities ..
            //    string apiName = model;
            //    IReadOnlySet<AiModelCapability> capabilities = new HashSet<AiModelCapability>();
            //    AiModelPricing pricing = new(0m, 0m);
            //    ModelDomain domain = ModelDomain.Other;
            //    OpenAIModelGeneration generation = OpenAIModelGeneration.Other;

            //    // .. then try to populate from registry if we have an entry for this model
            //    if (lookup.TryGetByName(apiName, out AiModelPropertyRegistryModel? entry) && entry is not null)
            //    {
            //        capabilities = evaluator.GetCapabilities(modelEntry: entry);
            //        pricing = entry.Pricing;
            //        domain = entry.Tier switch
            //        {
            //            "Chat" => ModelDomain.Chat,
            //            "Embedding" => ModelDomain.Embedding,
            //            "Audio" => ModelDomain.Audio,
            //            "Image" => ModelDomain.Image,
            //            "Moderation" => ModelDomain.Moderation,
            //            _ => ModelDomain.Other
            //        };
            //        generation = entry.Generation switch
            //        {
            //            "GPT-3" => OpenAIModelGeneration.GPT3,
            //            "GPT-3.5" => OpenAIModelGeneration.GPT35,
            //            "GPT-4" => OpenAIModelGeneration.GPT4,
            //            _ => OpenAIModelGeneration.Other
            //        };
            //    }

            //    // Construct descriptor (without Name for now, as we need to set it after the loop to avoid immutability issues)
            //    AiModelPropertyRegistryModel descriptor = new()
            //    {
            //        // Name is set later by registry finalization (or we can set directly)
            //        Capabilities = new HashSet<AiModelCapability>(capabilities),

            //        // For now, you can keep your existing pricing logic.
            //        Pricing = pricing,
            //        Tier = domain, // schema needs extension to support domain data
            //        Generation = generation, // schema needs extension to support generation data
            //    };

            //    descriptors[model] = descriptor;
            //}

            // Inject the Name property as a post-processing step to avoid immutability issues with the record type
            //foreach ((string model, AiModelPropertyRegistryModel descriptor) in descriptors)
            //{
            //    descriptor.Name = model;
            //}

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