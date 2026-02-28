// <copyright file="AdvancedConsolidatedResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Represents the consolidated result from multiple models.
    /// </summary>
    public class AdvancedConsolidatedResponse
    {
        /// <summary>
        /// Gets or sets the original user prompt.
        /// </summary>
        public string UserPrompt
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the final consolidated content.
        /// </summary>
        public string ConsolidatedContent
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the list of responses from all models.
        /// </summary>
        public List<AiModelResponse> ModelResponses
        {
            get;
            set;
        } = [];

        /// <summary>
        /// Gets or sets the consolidation mode used for this response.
        /// </summary>
        public ConsolidationMode ConsolidationMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the metadata from the consolidation process.
        /// </summary>
        public object? ConsolidationMetadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total latency for the entire operation.
        /// </summary>
        public TimeSpan TotalLatency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the count of successful model responses.
        /// </summary>
        public int SuccessCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the count of failed model responses.
        /// </summary>
        public int FailureCount
        {
            get;
            set;
        }
    }
}