// <copyright file="ParsedJudgeResponse.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.LLMJudge
{
    /// <summary>
    /// Represents a parsed response from the judge model.
    /// </summary>
    public class ParsedJudgeResponse
    {
        /// <summary>
        /// Gets or sets the index of the selected response.
        /// </summary>
        public int SelectedIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the selected response.
        /// </summary>
        public string SelectedResponse
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the reasoning for the selection.
        /// </summary>
        public string Reasoning
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the evaluation scores for the response.
        /// </summary>
        public Dictionary<string, int> Scores
        {
            get;
            set;
        } = [];
    }
}
