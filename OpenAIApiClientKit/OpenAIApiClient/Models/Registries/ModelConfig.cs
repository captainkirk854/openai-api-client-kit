// <copyright file="ModelConfig.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries
{
    /// <summary>
    /// Default configuration settings for an AI model.
    /// </summary>
    public sealed class ModelConfig
    {
        /// <summary>
        /// Gets the default temperature for the model.
        /// </summary>
        /// <remarks>
        /// Controls randomness. Higher values produce more creative output.
        /// Valid values range from 0 to 2.
        /// </remarks>
        public float Temperature
        {
            get;
            init;
        } = 0.7f;

        /// <summary>
        /// Gets the default maximum number of tokens the model is allowed to generate.
        /// </summary>
        public int MaxTokens
        {
            get;
            init;
        } = 4096;
    }
}
