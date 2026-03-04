// <copyright file="OpenAIModelAdvanced.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels.Capabilities
{
    /// <summary>
    /// Represents advanced configuration options for OpenAI model features.
    /// </summary>
    public sealed class OpenAIModelAdvanced
    {
        /// <summary>
        /// Gets the critic capability level of the model, indicating its ability to evaluate and critique content.
        /// </summary>
        public int Critic
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the editor capability level of the model, indicating its ability to edit and modify content effectively.
        /// </summary>
        public int Editor
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the JSON mode capability level of the model, indicating its ability to generate and understand JSON-formatted data.
        /// </summary>
        public int JSONMode
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the embedding capability level of the model, indicating its ability to generate and utilize embeddings for various applications.
        /// </summary>
        public int Embedding
        {
            get;
            init;
        }

        public int ImageGeneration
        {
            get;
            init;
        }
    }
}
