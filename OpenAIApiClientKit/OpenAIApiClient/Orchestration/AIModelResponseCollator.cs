// <copyright file="AiModelResponseCollator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Represents a collated response generated from one or more ai model responses.
    /// </summary>
    public sealed class AiModelResponseCollator
    {
        /// <summary>
        /// Gets the name of the model used to generate the final response.
        /// </summary>
        public OpenAIModel Name
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the content of the final response.
        /// </summary>
        public string Content
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the list of all model responses.
        /// </summary>
        public IReadOnlyList<AiModelResponse> SourceResponses
        {
            get;
            init;
        } = [];

        /// <summary>
        /// Gets the strategy used to collate multiple model responses, if applicable.
        /// </summary>
        public string? CollationStrategy
        {
            get;
            init;
        }

        /// <summary>
        /// Overrides the ToString() method to return the model name as a string.
        /// </summary>
        /// <returns>The model name as a string.</returns>
        public override string ToString() => this.Name.ToApiString();
    }
}
