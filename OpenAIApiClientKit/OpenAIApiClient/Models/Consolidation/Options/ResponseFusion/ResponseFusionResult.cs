// <copyright file="ResponseFusionResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.ResponseFusion
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Orchestration;

    /// <summary>
    /// Represents the result of response fusion consolidation strategy.
    /// </summary>
    public class ResponseFusionResult
    {
        /// <summary>
        /// Gets or sets the judge model used for synthesis.
        /// </summary>
        public OpenAIModel JudgeModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the synthesized response combining all inputs.
        /// </summary>
        public string SynthesizedResponse
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the list of source responses used for synthesis.
        /// </summary>
        public List<AiModelResponse> SourceResponses
        {
            get;
            set;
        } = [];

        /// <summary>
        /// Gets or sets the raw output from the fusion model.
        /// </summary>
        public string RawFusionOutput
        {
            get;
            set;
        } = string.Empty;
    }
}