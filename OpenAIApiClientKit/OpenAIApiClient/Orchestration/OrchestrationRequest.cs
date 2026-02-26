// <copyright file="OrchestrationRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Orchestration.Dispatch;

    public sealed class OrchestrationRequest
    {
        /// <summary>
        /// Gets a value indicating whether to use ensemble routing or single model routing.
        /// </summary>
        public bool UseEnsemble
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the input prompt for the model(s).
        /// </summary>
        public string Prompt
        {
            get;
            init;
        } = string.Empty;

        /// <summary>
        /// Gets the desired output format (e.g., JSON, Text).
        /// </summary>
        public OutputFormat OutputFormat
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the single model request details if <see cref="UseEnsemble"/> is false..
        /// </summary>
        public SingleAiModelDispatchRequest? SingleModelRequest
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the ensemble model request details if <see cref="UseEnsemble"/> is true).
        /// </summary>
        public EnsembleDispatchRequest? EnsembleRequest
        {
            get;
            init;
        }
    }
}
