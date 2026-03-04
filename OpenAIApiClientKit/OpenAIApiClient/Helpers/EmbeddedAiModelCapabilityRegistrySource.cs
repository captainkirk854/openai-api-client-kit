// <copyright file="EmbeddedAiModelCapabilityRegistrySource.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers
{
    using System.Reflection;
    using OpenAIApiClient.Interfaces.Registries;

    /// <summary>
    /// This class provides a registry source that reads AI model capabilities from embedded JSON resources within the assembly.
    /// It implements the <see cref="IAiModelCapabilityRegistrySource"/> interface, allowing it to be used as a source for loading
    /// AI model capabilities in the application.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EmbeddedAiModelCapabilityRegistrySource"/> class using the default assembly.
    /// and resource prefix.
    /// </remarks>
    /// <param name="assembly">The assembly to search for embedded resources. If null, the assembly containing this class will be used.</param>
    /// <param name="resourcePrefix">The prefix to filter embedded resources. If null, a default prefix will be used.</param>
    /// <param name="resourceSuffix">The suffix to filter embedded resources. If null, a default suffix will be used.</param>
    public sealed class EmbeddedAiModelCapabilityRegistrySource(Assembly? assembly = null, string? resourcePrefix = null, string? resourceSuffix = null) : IAiModelCapabilityRegistrySource
    {
        // Default values for resource prefix and suffix if not provided by the caller.
        private const string DefaultResourcePrefix = "OpenAIApiClient._internal.OpenAiModels.capabilities.";
        private const string DefaultFileSuffix = ".json";

        // Initialize the assembly and resource filtering parameters, using defaults if not provided.
        private readonly Assembly assembly = assembly ?? typeof(EmbeddedAiModelCapabilityRegistrySource).Assembly;
        private readonly string resourcePrefix = resourcePrefix ?? DefaultResourcePrefix;
        private readonly string resourceSuffix = resourceSuffix ?? DefaultFileSuffix;

        /// <summary>
        /// Enumerates embedded JSON resource streams from the assembly that match the specified resource prefix.
        /// </summary>
        /// <returns> An enumerable of <see cref="Stream"/> objects representing the matching embedded resources.</returns>
        public IEnumerable<Stream> GetRegistryStreams()
        {
            // e.g., "OpenAIApiClient._internal.OpenAiModels.capabilities.openai-capabilities.base.json"
            foreach (string resourceName in this.assembly.GetManifestResourceNames()
                         .Where(n => n.StartsWith(this.resourcePrefix, StringComparison.OrdinalIgnoreCase)
                                  && n.EndsWith(this.resourceSuffix, StringComparison.OrdinalIgnoreCase)))
            {
                Stream? stream = this.assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    yield return stream;
                }
            }
        }
    }
}
