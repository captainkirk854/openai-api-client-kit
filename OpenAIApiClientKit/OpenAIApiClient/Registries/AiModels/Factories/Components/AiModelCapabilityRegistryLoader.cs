// <copyright file="AiModelCapabilityRegistryLoader.cs" company="854 Things (tm)">
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
    /// <param name="source">The source providing registry data streams.</param>
    public sealed class AiModelCapabilityRegistryLoader(IAiModelCapabilityRegistrySource source)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly IAiModelCapabilityRegistrySource source = source;

        /// <summary>
        /// Loads and merges AI model capability registry data from multiple sources.
        /// </summary>
        /// <returns>A merged <see cref="AiModelCapabilityRegistryData"/> instance containing combined data from all sources.</returns>
        public AiModelCapabilityRegistryData LoadMerged()
        {
            AiModelCapabilityRegistryData merged = new();

            // Iterate through each registry stream provided by the source, read and deserialize the JSON data, and merge it into the final registry data.
            foreach (Stream stream in this.source.GetRegistryStreams())
            {
                using (stream)
                using (StreamReader reader = new(stream))
                {
                    string json = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        continue;
                    }

                    // Deserialize the JSON data into a partial registry data object. If the deserialization fails or the Models property is null, skip to the next stream.
                    AiModelCapabilityRegistryData? partial = JsonSerializer.Deserialize<AiModelCapabilityRegistryData>(json, JsonOptions);
                    if (partial?.Models is null)
                    {
                        continue;
                    }

                    // Add the models from the partial registry data to the merged registry data.
                    foreach (AiModelCapabilityRegistryModel model in partial.Models)
                    {
                        merged.Models.Add(model);
                    }
                }
            }

            return merged;
        }
    }
}
