// <copyright file="SingleAiModelExecutionContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Defines the execution context for single model requests.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="outputFormat"></param>
    /// <param name="model"></param>
    public sealed class SingleAiModelExecutionContext(string prompt, OutputFormat outputFormat, AiModelDescriptor model) : IExecutionContext
    {
        /// <summary>
        /// Gets the prompt to be sent to the model.
        /// </summary>
        public string Prompt
        {
            get;
        } = prompt;

        /// <summary>
        /// Gets the desired output format.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get;
        } = outputFormat;

        /// <summary>
        /// Gets the model to be executed.
        /// </summary>
        public AiModelDescriptor Model
        {
            get;
        } = model;

        /// <summary>
        /// Gets the list of models to be executed.
        /// </summary>
        public IReadOnlyList<AiModelDescriptor> Models => [this.Model];
    }
}
