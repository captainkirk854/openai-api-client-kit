// <copyright file="AiModelPropertyRegistryModel.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    /// <summary>
    /// Represents a registry entry for an OpenAI model and its capability scores.
    /// </summary>
    public sealed class AiModelPropertyRegistryModel
    {
        /// <summary>
        /// Gets the model identifier (e.g., gpt-4.1, gpt-4o-mini).
        /// </summary>
        public string Name
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the upper-cased model identifier.
        /// </summary>
        public string UpperName
        {
            get
            {
                return this.Name.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Gets the model generation (e.g., GPT-4.1, GPT-4o, O-series).
        /// </summary>
        public string Generation
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the tags for the model (e.g., standard, pro, enterprise).
        /// </summary>
        public string Tier
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the model tier (e.g., standard, pro, enterprise).
        /// </summary>
        public IReadOnlyList<string> Tags
        {
            get;
            init;
        } = [];

        /// <summary>
        /// Gets the pricing information for the model.
        /// </summary>
        public AiModelPricing Pricing
        {
            get;
            init;
        } = new AiModelPricing(0, 0);

        /// <summary>
        /// Gets Capability scores for this model.
        /// </summary>
        public AiModelCapabilityScores Capabilities
        {
            get;
            init;
        } = new AiModelCapabilityScores();
    }
}
