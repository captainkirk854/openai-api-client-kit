// <copyright file="SingleModelExecutionContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    public sealed class SingleModelExecutionContext(string prompt, OutputFormat outputFormat, ModelDescriptor model) : IExecutionContext
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
        public ModelDescriptor Model
        {
            get;
        } = model;

        /// <summary>
        /// Gets the list of models to be executed.
        /// </summary>
        public IReadOnlyList<ModelDescriptor> Models => [this.Model];
    }
}
