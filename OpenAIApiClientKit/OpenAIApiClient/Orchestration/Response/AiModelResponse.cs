// <copyright file="AiModelResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Response
{
    using System.Collections.Generic;
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Model response details.
    /// </summary>
    public sealed class AiModelResponse
    {
        /// <summary>
        /// Gets the model descriptor.
        /// </summary>
        public AiModelDescriptor Model
        {
            get;
            init;
        } = default!;

        /// <summary>
        /// Gets the raw output from the model.
        /// </summary>
        public string RawOutput
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the model call was successful.
        /// </summary>
        public bool IsSuccessful
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the error message if the model call was not successful.
        /// </summary>
        public string? ErrorMessage
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the latency of the model call.
        /// </summary>
        public TimeSpan Latency
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the total tokens used in the model call.
        /// </summary>
        public decimal TotalTokens
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the estimated cost of the model call.
        /// </summary>
        public decimal EstimatedCost
        {
            get;
            init;
        }

        /// <summary>
        /// Wraps a single ModelResponse into a list.
        /// </summary>
        /// <param name="modelResponse"></param>
        /// <returns>a list containing the single ModelResponse.</returns>
        internal static IReadOnlyList<AiModelResponse> WrapSingleResponseAsList(AiModelResponse modelResponse)
        {
            return [modelResponse];
        }
    }
}