// <copyright file="ScoredResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation
{
    using OpenAIApiClient.Orchestration;

    /// <summary>
    /// Represents a response that has been scored during heuristic evaluation.
    /// </summary>
    public class ScoredResponse
    {
        /// <summary>
        /// Gets or sets the index of the response in the original list.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the model response being scored.
        /// </summary>
        public AiModelResponse Response
        {
            get;
            set;
        } = null!;

        /// <summary>
        /// Gets or sets the heuristic score for this response.
        /// </summary>
        public int Score
        {
            get;
            set;
        }
    }
}
