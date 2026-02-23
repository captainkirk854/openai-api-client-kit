// <copyright file="OutputFormats.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Output
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.OutputFormat.General;
    using OpenAIApiClient.Models.OutputFormat.Validators;
    using OpenAIApiClient.Registries.Prompts;

    /// <summary>
    /// Provides a registry of output format prompts mapped to their corresponding output format types.
    /// </summary>
    public static class OutputFormats
    {
        /// <summary>
        /// Dictionary which maps output formats to their format descriptors.
        /// </summary>
        public static readonly IReadOnlyDictionary<OutputFormat, FormatDescriptor> FormattingPrompts =
        new Dictionary<OutputFormat, FormatDescriptor>
        {
            [OutputFormat.PlainText] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.PlainText], Validator: new PlainTextValidator()),
            [OutputFormat.Json] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Json], Validator: new JsonValidator()),
            [OutputFormat.Csv] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Csv], Validator: new CsvValidator()),
            [OutputFormat.Xml] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Xml], Validator: new XmlValidator()),
            [OutputFormat.Markdown] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Markdown], Validator: new MarkdownValidator()),
            [OutputFormat.Yaml] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Yaml], Validator: new YamlValidator()),
            [OutputFormat.Html] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Html], Validator: new HtmlValidator()),
            [OutputFormat.Table] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Table], Validator: new MarkdownValidator()),
            [OutputFormat.Sql] = new FormatDescriptor(SystemPrompt: PromptRegistry.Prompts[PromptId.Sql], Validator: new SqlValidator()),
        };
    }
}