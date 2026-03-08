// <copyright file="EmbeddedAiModelPropertyRegistryLoader.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels.Sources
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Loads <see cref="AiModelDescriptor"/> instances from embedded JSON registry streams.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EmbeddedAiModelPropertyRegistryLoader"/> class.
    /// </remarks>
    /// <param name="resource">The source that provides embedded JSON registry streams.</param>
    public sealed class EmbeddedAiModelPropertyRegistryLoader(IAiModelCapabilityRegistryResource resource)
    {
        private readonly IAiModelCapabilityRegistryResource source = resource;

        /// <summary>
        /// Loads all <see cref="AiModelDescriptor"/> instances from the embedded registry streams.
        /// </summary>
        /// <returns>
        /// A read-only collection of <see cref="AiModelDescriptor"/> instances.
        /// </returns>
        public IReadOnlyCollection<AiModelDescriptor> Load()
        {
            List<AiModelDescriptor> models = [];

            JsonSerializerOptions options = new(defaults: JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
            };

            IEnumerable<Stream> streams = this.source.GetAiModelRegistryStreams();

            foreach (Stream stream in streams)
            {
                using (stream)
                using (StreamReader reader = new(stream: stream, leaveOpen: false))
                {
                    string json = reader.ReadToEnd();

                    // Basic guard against empty embedded resources ..
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new InvalidDataException("Embedded capability JSON must not be empty.");
                    }

                    // Deserialize into the DTO wrapper.
                    AiModelPropertyRegistryData? registry = JsonSerializer.Deserialize<AiModelPropertyRegistryData>(json: json, options: options) ?? throw new InvalidDataException("Capability registry deserialization returned null.");
                    if (registry.Models is null || !registry.Models.Any())
                    {
                        throw new InvalidDataException("Capability registry 'models' collection must not be null or empty.");
                    }

                    models.AddRange(registry.Models);
                }
            }

            return models;
        }
    }
}