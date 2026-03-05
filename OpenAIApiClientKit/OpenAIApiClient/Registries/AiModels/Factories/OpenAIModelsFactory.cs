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
            Dictionary<OpenAIModel, AiModelDescriptor> descriptors = [];
            foreach (OpenAIModel model in Enum.GetValues(typeof(OpenAIModel)))
            {
                // Initialise with empty capabilities ..
                string apiName = model.ToApiString();
                IReadOnlySet<AiModelCapability> capabilities = new HashSet<AiModelCapability>();
                AiModelPricing pricing = new(0m, 0m);

                // .. then try to populate from registry if we have an entry for this model
                if (lookup.TryGetByName(apiName, out AiModelPropertyRegistryModel? entry) && entry is not null)
                {
                    capabilities = evaluator.GetCapabilities(modelEntry: entry);
                    pricing = entry.Pricing;
                }

                // Construct descriptor (without Name for now, as we need to set it after the loop to avoid immutability issues)
                AiModelDescriptor descriptor = new()
                {
                    // Name is set later by registry finalization (or we can set directly)
                    Capabilities = new HashSet<AiModelCapability>(capabilities),

                    // For now, you can keep your existing pricing logic.
                    Pricing = pricing, // schema needs extension to support pricing data
                    Domain = ModelDomain.Other, // schema needs extension to support domain data
                    Generation = OpenAIModelGeneration.Other, // schema needs extension to support generation data
                };

                descriptors[model] = descriptor;
            }

            // Inject the Name property as a post-processing step to avoid immutability issues with the record type
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in descriptors)
            {
                descriptor.Name = model;
            }

            // Construct and return registry
            return new OpenAIModelsNEW(descriptors);
        }
    }
}