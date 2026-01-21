// <copyright file="OutputFormatRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.OutputFormat.General;
    using OpenAIApiClient.Models.OutputFormat.Validators;

    /// <summary>
    /// Provides a registry of output format prompts mapped to their corresponding output format types.
    /// </summary>
    public static class OutputFormatRegistry
    {
        /// <summary>
        /// Dictionary mapping output formats to their format descriptors.
        /// </summary>
        public static readonly IReadOnlyDictionary<OutputFormat, FormatDescriptor> Prompts =
        new Dictionary<OutputFormat, FormatDescriptor>
        {
            [OutputFormat.PlainText] = new FormatDescriptor(SystemPrompt: PlainTextPrompt, Validator: new PlainTextValidator()),
            [OutputFormat.Json] = new FormatDescriptor(SystemPrompt: JsonPrompt, Validator: new JsonValidator()),
            [OutputFormat.Csv] = new FormatDescriptor(SystemPrompt: CsvPrompt, Validator: new CsvValidator()),
            [OutputFormat.Xml] = new FormatDescriptor(SystemPrompt: XmlPrompt, Validator: new XmlValidator()),
            [OutputFormat.Markdown] = new FormatDescriptor(SystemPrompt: MarkdownPrompt, Validator: new MarkdownValidator()),
            [OutputFormat.Yaml] = new FormatDescriptor(SystemPrompt: YamlPrompt, Validator: new YamlValidator()),
            [OutputFormat.Html] = new FormatDescriptor(SystemPrompt: HtmlPrompt, Validator: new HtmlValidator()),
            [OutputFormat.Table] = new FormatDescriptor(SystemPrompt: TablePrompt, Validator: new MarkdownValidator()),
            [OutputFormat.Sql] = new FormatDescriptor(SystemPrompt: SqlPrompt, Validator: new SqlValidator()),
        };

        private const string PlainTextPrompt =
        @"You are a formatter that outputs plain text only.
          No code blocks, no markup, no explanations. Output raw text.";

        private const string JsonPrompt =
        @"You are a formatter that outputs strictly valid JSON.
          No comments, no trailing commas, no explanations.
          Return only a JSON object or array.
          You MUST respond with a valid JSON object only.
          No explanations, no prose, no markdown.
          Return strictly valid JSON that matches the expected schema.";

        private const string CsvPrompt =
        @"You are a formatter that outputs CSV only.
          Use commas as separators and include a header row.
          No code blocks, no explanations, no extra text.";

        private const string XmlPrompt =
        @"You are a formatter that outputs strictly valid XML.
          Include a single root element.
          No comments or explanations.
          No code blocks, no explanations, no extra text.";

        private const string MarkdownPrompt =
        @"You are a formatter that outputs GitHub‑flavored Markdown.
          Use proper headings, lists, and tables.
          No explanations outside the Markdown.";

        private const string YamlPrompt =
        @"You are a formatter that outputs strictly valid YAML.
          No tabs, no comments, no explanations.
          Only YAML.
          No code blocks, no explanations, no extra text.";

        private const string HtmlPrompt =
        @"You are a formatter that outputs strictly valid HTML.
          Include proper opening and closing tags.
          No Markdown, no comments, no explanations.
          No code blocks, no explanations, no extra text.";

        private const string TablePrompt =
        @"You are a formatter that outputs data in a tabular format.
          Use rows and columns to represent the data clearly.
          No code blocks, no explanations, no extra text.";

        private const string SqlPrompt =
        @"You are a formatter that outputs SQL code only.
          No explanations, no prose, no markdown.
          Output valid SQL statements only.";
    }
}