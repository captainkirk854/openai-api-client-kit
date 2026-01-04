// <copyright file="PromptContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    using OpenAIApiClient.Enums;

    public sealed class PromptContext
    {
        /// <summary>
        /// Gets the prompt to be sent to the model.
        /// </summary>
        public string Prompt
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the set of capabilities required from the model to process this prompt.
        /// </summary>
        public IReadOnlySet<ModelCapability> RequiredCapabilities
        {
            get;
            init;
        } = new HashSet<ModelCapability>();

        /// <summary>
        /// Gets the desired output format.
        /// </summary>
        public string? DesiredOutputFormat
        {
            get;
            init;
        } // e.g. "text", "json"

        /// <summary>
        /// Gets the maximum acceptable latency for processing this prompt.
        /// </summary>
        public TimeSpan? MaxLatency
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the maximum acceptable cost for processing this prompt.
        /// </summary>
        public decimal? MaxCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets additional metadata associated with the prompt.
        /// </summary>
        public IDictionary<string, object> Metadata
        {
            get;
            init;
        } = new Dictionary<string, object>();
    }
}
