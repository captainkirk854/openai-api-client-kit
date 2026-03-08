// <copyright file="OpenAIModelPerformance.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels.Capabilities
{
    /// <summary>
    /// Represents the performance capabilities of an OpenAI model, including FastInference and HighPerformance scores.
    /// These scores provide insights into the model's efficiency and overall performance, helping users understand how
    /// well the model can handle various tasks and workloads.
    /// </summary>
    public sealed class OpenAIModelPerformance
    {
        /// <summary>
        /// Gets the FastInference performance score of the model. This score indicates how well the model performs in
        /// terms of inference speed and efficiency. A higher score suggests that the model can process inputs and generate
        /// outputs more quickly, making it suitable for applications that require real-time or near-real-time responses.
        /// </summary>
        public int FastInference
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the HighPerformance score of the model. This score reflects the overall performance of the model, taking into
        /// account factors such as accuracy, speed, and resource efficiency. A higher score indicates that the model is well-optimized
        /// and can deliver high-quality results while maintaining efficient use of computational resources.
        /// </summary>
        public int HighPerformance
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the total performance score of the model by summing the FastInference and HighPerformance scores providing an overall measure of the model's performance capabilities.
        /// </summary>
        public int Total
        {
            get
            {
                return this.FastInference +
                       this.HighPerformance;
            }
        }
    }
}
