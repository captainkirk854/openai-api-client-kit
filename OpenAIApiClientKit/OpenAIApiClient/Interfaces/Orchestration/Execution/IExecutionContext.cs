// <copyright file="IExecutionContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration.Execution
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    public interface IExecutionContext
    {
        /// <summary>
        /// Gets the prompt to be sent to the model.
        /// </summary>
        string Prompt
        {
            get;
        }

        /// <summary>
        /// Gets the desired output format.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get;
        }

        /// <summary>
        /// Gets the list of models to be executed.
        /// </summary>
        IReadOnlyList<ModelDescriptor> Models
        {
            get;
        }
    }
}
