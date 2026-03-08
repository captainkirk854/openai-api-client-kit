// <copyright file="ResponseSynthesisResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Consolidation.Options.ResponseSynthesis
{
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Represents the result of response fusion consolidation strategy.
    /// </summary>
    public class ResponseSynthesisResult
    {
        /// <summary>
        /// Gets or sets the judge model used for synthesis.
        /// </summary>
        public string? SynthesisModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the synthesized response combining all inputs.
        /// </summary>
        public string SynthesisedResponse
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