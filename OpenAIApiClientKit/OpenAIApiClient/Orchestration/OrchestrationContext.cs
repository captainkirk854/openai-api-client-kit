// <copyright file="OrchestrationContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;

    /// <summary>
    /// Defines the orchestration context for model requests.
    /// </summary>
    public sealed class OrchestrationContext
    {
        public OrchestrationContext(string prompt, OutputFormat outputFormat, IExecutionContext executionContext, ClientRequestBuilder builder)
        {
            this.Prompt = prompt;
            this.OutputFormat = outputFormat;
            this.ExecutionContext = executionContext;

            // Build the request
            ChatCompletionRequest request = builder
                .WithModel(input: executionContext.Models[0].Name)
                .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                .AddUserMessage(input: prompt)
                .UsingMaxTokens(input: 1000)
                .SetOutputFormat(input: outputFormat)
                .Build();

            // Create the prompt context
            this.PromptContext = new PromptContext
            {
                Prompt = prompt,
                OutputFormat = outputFormat,
                Request = request,
            };
        }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        public string Prompt
        {
            get;
        }

        /// <summary>
        /// Gets the output format.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get;
        }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        public IExecutionContext ExecutionContext
        {
            get;
        }

        /// <summary>
        /// Gets the prompt context.
        /// </summary>
        public PromptContext PromptContext
        {
            get;
        }

        public bool IsEnsemble => this.ExecutionContext.Models.Count > 1;
    }
}
