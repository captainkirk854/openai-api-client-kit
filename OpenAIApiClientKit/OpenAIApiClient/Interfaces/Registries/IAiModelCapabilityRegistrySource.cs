// <copyright file="IAiModelCapabilityRegistrySource.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Registries
{
    public interface IAiModelCapabilityRegistrySource
    {
        /// <summary>
        /// Returns zero or more readable streams, each containing a JSON
        /// capability registry document that conforms to the
        /// OpenAIModelCapabilityRegistry schema.
        /// </summary>
        /// <returns see cref="IEnumerable{Stream}"> A sequence of streams, each containing a JSON capability registry document.</returns>
        IEnumerable<Stream> GetRegistryStreams();
    }
}