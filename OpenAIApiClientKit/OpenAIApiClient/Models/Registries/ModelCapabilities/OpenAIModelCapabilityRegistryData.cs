// <copyright file="OpenAIModelCapabilityRegistryData.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.ModelCapabilities
{
    /// <summary>
    /// Represents a registry of OpenAI models and their associated capabilities.
    /// </summary>
    public sealed class OpenAIModelCapabilityRegistryData
    {
        /// <summary>
        /// Gets the list of models and their capabilities.
        /// </summary>
        public IList<OpenAIModelCapabilityRegistryModel> Models
        {
            get;
            init;
        } = [];
    }
}
