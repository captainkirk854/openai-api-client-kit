// <copyright file="ConsolidationMode.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum ConsolidationMode
    {
        /// <summary>
        /// Judge model selects the best response
        /// </summary>
        LLMAsJudge,

        /// <summary>
        /// Deterministic heuristic scoring
        /// </summary>
        HeuristicScoring,

        /// <summary>
        /// Judge model synthesizes all responses into one
        /// </summary>
        ResponseFusion,
    }
}
