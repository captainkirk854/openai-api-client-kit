// <copyright file="AiModelConsolidationMode.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum AiModelConsolidationMode
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
        /// Synthesis model synthesizes all response(s) into one
        /// </summary>
        ResponseSynthesis,
    }
}
