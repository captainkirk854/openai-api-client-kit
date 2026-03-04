// <copyright file="OpenAIModelOperational.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.ModelCapabilities.Capabilities
{
    /// <summary>
    /// Represents operational characteristics and scores for an OpenAI model, including cost-effectiveness, latency,
    /// and open-ended task handling.
    /// </summary>
    public sealed class OpenAIModelOperational
    {
        /// <summary>
        /// Gets the low cost score for the model. This score is a measure of how cost-effective the model is to use. A higher score indicates that the model is more cost-effective.
        /// </summary>
        public int LowCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the latency score for the model. This score is a measure of how quickly the model can respond to requests. A higher score indicates that the model has lower latency.
        /// </summary>
        public int Moderation
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the open weight score for the model. This score is a measure of how well the model can handle open-ended tasks. A higher score indicates that the model is better at handling open-ended tasks.
        /// </summary>
        public int OpenWeight
        {
            get;
            init;
        }
    }
}
