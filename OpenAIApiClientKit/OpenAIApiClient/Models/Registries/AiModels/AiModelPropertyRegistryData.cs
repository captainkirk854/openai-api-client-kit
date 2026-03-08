// <copyright file="AiModelPropertyRegistryData.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    /// <summary>
    /// Represents a registry of OpenAI models and their associated capabilities.
    /// </summary>
    public sealed class AiModelPropertyRegistryData
    {
        /// <summary>
        /// Gets the list of models and their capabilities.
        /// </summary>
        public IList<AiModelDescriptor> Models
        {
            get;
            init;
        } = [];
    }
}
