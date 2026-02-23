// <copyright file="ScoredResponseDetail.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring
{
    /// <summary>
    /// Represents the detailed score breakdown for a single response.
    /// </summary>
    public class ScoredResponseDetail
    {
        /// <summary>
        /// Gets or sets the content of the scored response.
        /// </summary>
        public string Content
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the total score for this response.
        /// </summary>
        public int TotalScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the breakdown of the total score by factor.
        /// </summary>
        public Dictionary<string, int> ScoreBreakdown
        {
            get;
            set;
        } = [];
    }
}
