// <copyright file="IAiModelCapabilityRegistryResource.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    public interface IAiModelCapabilityRegistryResource
    {
        /// <summary>
        /// Gets a sequence of streams, each containing a JSON AI model registry document.
        /// Implementations of this interface should provide access to the necessary registry data through these streams,
        /// which can be read and processed to build a comprehensive AI model registry.
        /// </summary>
        /// <returns see cref="IEnumerable{Stream}"> A sequence of streams, each containing a JSON AI model registry document.</returns>
        IEnumerable<Stream> GetAiModelRegistryStreams();
    }
}