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
    /// Loads <see cref="AiModelPropertyRegistryModel"/> instances from embedded JSON registry streams.
    /// </summary>
    public sealed class EmbeddedAiModelPropertyRegistryLoader
    {
        private readonly IAiModelCapabilityRegistrySource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedAiModelPropertyRegistryLoader"/> class.
        /// </summary>
        /// <param name="source">The source that provides embedded JSON registry streams.</param>
        public EmbeddedAiModelPropertyRegistryLoader(IAiModelCapabilityRegistrySource source)
        {
            this.source = source;
        }

        /// <summary>
        /// Loads all <see cref="AiModelPropertyRegistryModel"/> instances from the embedded registry streams.
        /// </summary>
        /// <returns>
        /// A read-only collection of <see cref="AiModelPropertyRegistryModel"/> instances.
        /// </returns>
        public IReadOnlyCollection<AiModelPropertyRegistryModel> Load()
        {
            List<AiModelPropertyRegistryModel> models = new List<AiModelPropertyRegistryModel>();

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
            };

            IEnumerable<Stream> streams = this.source.GetRegistryStreams();

            foreach (Stream stream in streams)
            {
                using (stream)
                using (StreamReader reader = new StreamReader(stream, leaveOpen: false))
                {
                    string json = reader.ReadToEnd();

                    // Basic guard against empty embedded resources ..
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new InvalidDataException("Embedded capability JSON must not be empty.");
                    }

                    // Deserialize into the DTO wrapper.
                    AiModelPropertyRegistryData? registry = JsonSerializer.Deserialize<AiModelPropertyRegistryData>(json: json, options: options);

                    if (registry is null)
                    {
                        throw new InvalidDataException("Capability registry deserialization returned null.");
                    }

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