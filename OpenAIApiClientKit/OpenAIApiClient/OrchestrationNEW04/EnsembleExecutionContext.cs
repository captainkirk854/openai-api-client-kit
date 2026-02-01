// <copyright file="EnsembleExecutionContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Defines the execution context for ensemble model requests.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="outputFormat"></param>
    /// <param name="models"></param>
    public sealed class EnsembleExecutionContext(string prompt, OutputFormat outputFormat, IReadOnlyList<ModelDescriptor> models) : IExecutionContext
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
        /// Gets the list of models to be executed.
        /// </summary>
        public IReadOnlyList<ModelDescriptor> Models
        {
            get;
        } = models;
    }
}
