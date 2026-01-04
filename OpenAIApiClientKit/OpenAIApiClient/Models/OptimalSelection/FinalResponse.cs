// <copyright file="FinalResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;

    public sealed class FinalResponse
    {
        /// <summary>
        /// Gets the name of the model used to generate the final response.
        /// </summary>
        public OpenAIModels Name
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
        public IReadOnlyList<ModelResponse> SourceResponses
        {
            get;
            init;
        } = [];

        /// <summary>
        /// Gets the strategy used to aggregate multiple model responses, if applicable.
        /// </summary>
        public string? AggregationStrategy
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
