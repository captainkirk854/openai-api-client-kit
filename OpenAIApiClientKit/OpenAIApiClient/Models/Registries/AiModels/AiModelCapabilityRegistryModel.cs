// <copyright file="AiModelCapabilityRegistryModel.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    /// <summary>
    /// Represents a registry entry for an OpenAI model and its capability scores.
    /// </summary>
    public sealed class AiModelCapabilityRegistryModel
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
        /// Gets the model family (e.g., gpt-4, gpt-4o, o-series).
        /// </summary>
        public string Family
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the model tier (e.g., standard, pro, enterprise).
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
