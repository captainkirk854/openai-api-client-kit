// <copyright file="SingleModelExecutionContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW03
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    public sealed class SingleModelExecutionContext(string prompt, ModelDescriptor model, OutputFormat outputFormat) : IExecutionContext
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

        /// <summary>
        /// Builds the prompt context.
        /// </summary>
        /// <param name="model">The model descriptor.</param>
        /// <returns><see cref="PromptContext"/>.</returns>
        public PromptContext BuildPromptContext()
            => new()
            {
                Prompt = this.Prompt,
                OutputFormat = this.OutputFormat,
            };
    }
}
