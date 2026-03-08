// <copyright file="LLMJudgeResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.LLMJudge
{
    /// <summary>
    /// Represents the result of LLM Judge consolidation strategy.
    /// </summary>
    public class LLMJudgeResult
    {
        /// <summary>
        /// Gets or sets the judge model used for evaluation.
        /// </summary>
        public string? JudgeModel
        {
            get;
            set;
        }

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
        /// Gets or sets the judge's reasoning for the selection.
        /// </summary>
        public string JudgeReasoning
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the scores assigned by the judge to the selected response.
        /// </summary>
        public Dictionary<string, int> JudgeScores
        {
            get;
            set;
        } = [];

        /// <summary>
        /// Gets or sets the raw output from the judge model.
        /// </summary>
        public string RawJudgeOutput
        {
            get;
            set;
        } = string.Empty;
    }
}