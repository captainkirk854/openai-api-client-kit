// <copyright file="FanoutModelResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation
{
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Represents a single model's response in the fan-out process.
    /// </summary>
    public class FanoutModelResponse
    {
        /// <summary>
        /// Gets or sets the model descriptor for this response.
        /// </summary>
        public AiModelPropertyRegistryModel? Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the model's response.
        /// </summary>
        public string Content
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the model call was successful.
        /// </summary>
        public bool IsSuccessful
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the latency of the model call.
        /// </summary>
        public TimeSpan Latency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error message if the model call failed.
        /// </summary>
        public string? ErrorMessage
        {
            get;
            set;
        }
    }
}
