// <copyright file="AiModelPropertyRegistryLoader.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Factories.Components
{
    using System.Text.Json;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Provides functionality to load and merge AI model capability registry data from multiple sources.
    /// </summary>
    /// <param name="resource">The source providing registry data streams.</param>
    public sealed class AiModelPropertyRegistryLoader(IAiModelCapabilityRegistryResource resource)
    {
        // Define JSON deserialization options to allow for case-insensitive property name matching, ensuring that the loader can handle JSON data with varying property name cases without issues.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        // Store the provided resource for later use in loading registry data streams.
        private readonly IAiModelCapabilityRegistryResource resource = resource;

        /// <summary>
        /// Loads and merges AI model capability registry data from embedded resources.
        /// </summary>
        /// <returns>A merged <see cref="AiModelPropertyRegistryData"/> instance containing combined data from embedded resources.</returns>
        public AiModelPropertyRegistryData LoadMerged()
        {
            AiModelPropertyRegistryData merged = new();

            // Iterate through each registry stream provided by the source, read and deserialize the JSON data, and merge it into the final registry data.
            foreach (Stream stream in this.resource.GetAiModelRegistryStreams())
            {
                using (stream)
                using (StreamReader reader = new(stream))
                {
                    string json = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        continue;
                    }

                    // Deserialize JSON data into a partial registry data object. If deserialization fails or the Models property is null, skip to the next stream.
                    AiModelPropertyRegistryData? partial = JsonSerializer.Deserialize<AiModelPropertyRegistryData>(json, JsonOptions);
                    if (partial?.Models is null)
                    {
                        continue;
                    }

                    // Add models from the partial registry data to the merged registry data.
                    foreach (AiModelDescriptor model in partial.Models)
                    {
                        merged.Models.Add(model);
                    }
                }
            }

            return merged;
        }
    }
}