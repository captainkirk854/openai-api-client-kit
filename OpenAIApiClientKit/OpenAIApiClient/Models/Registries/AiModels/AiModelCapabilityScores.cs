// <copyright file="AiModelCapabilityScores.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    using OpenAIApiClient.Models.Registries.AiModels.Capabilities;

    /// <summary>
    /// Represents capability scores for an OpenAI model, including core, advanced, performance, and operational
    /// aspects.
    /// </summary>
    public sealed class AiModelCapabilityScores
    {
        /// <summary>
        /// Gets the core capability scores for the model, which include fundamental abilities such as language understanding, reasoning, and contextual awareness.
        /// </summary>
        public OpenAIModelCore Core
        {
            get;
            init;
        } = new OpenAIModelCore();

        /// <summary>
        /// Gets the advanced capability scores for the model, which include higher-level abilities such as creativity, problem-solving, and multi-modal understanding.
        /// </summary>
        public OpenAIModelAdvanced Advanced
        {
            get;
            init;
        } = new OpenAIModelAdvanced();

        /// <summary>
        /// Gets the performance capability scores for the model, which include metrics related to speed, efficiency, and resource utilization.
        /// </summary>
        public OpenAIModelPerformance Performance
        {
            get;
            init;
        } = new OpenAIModelPerformance();

        /// <summary>
        /// Gets the operational capability scores for the model, which include factors such as reliability, scalability, and ease of integration.
        /// </summary>
        public OpenAIModelOperational Operational
        {
            get;
            init;
        } = new OpenAIModelOperational();
    }
}