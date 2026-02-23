// <copyright file="HeuristicScoringResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring
{
    /// <summary>
    /// Represents the result of heuristic scoring consolidation strategy.
    /// </summary>
    public class HeuristicScoringResult
    {
        /// <summary>
        /// Gets or sets the selected response based on heuristic scoring.
        /// </summary>
        public string SelectedResponse
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the index of the selected model.
        /// </summary>
        public int SelectedModelIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the selected model.
        /// </summary>
        public string SelectedModel
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed scoring information for each response.
        /// </summary>
        public Dictionary<string, ScoredResponseDetail> ScoredResponses
        {
            get;
            set;
        } = [];
    }
}
