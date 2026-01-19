// <copyright file="OutputFormatRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;

    /// <summary>
    /// Provides a registry of output format prompts mapped to their corresponding output format types.
    /// </summary>
    public static class OutputFormatRegistry
    {
        public static readonly IReadOnlyDictionary<OutputFormat, string> Prompts =
            new Dictionary<OutputFormat, string>
            {
                [OutputFormat.PlainText] =
                    "You are a formatter that outputs plain text only. " +
                    "No code blocks, no markup, no explanations. Output raw text.",

                [OutputFormat.Json] =
                    "You are a formatter that outputs strictly valid JSON. " +
                    "No comments, no trailing commas, no explanations. " +
                    "Return only a JSON object or array." +
                    "You MUST respond with a valid JSON object only. " +
                    "No explanations, no prose, no markdown. " +
                    "Return strictly valid JSON that matches the expected schema.",

                [OutputFormat.Csv] =
                    "You are a formatter that outputs CSV only. " +
                    "Use commas as separators and include a header row. " +
                    "No code blocks, no explanations, no extra text.",

                [OutputFormat.Xml] =
                    "You are a formatter that outputs strictly valid XML. " +
                    "Include a single root element. No comments or explanations." +
                    "No code blocks, no explanations, no extra text.",

                [OutputFormat.Markdown] =
                    "You are a formatter that outputs GitHub‑flavored Markdown. " +
                    "Use proper headings, lists, and tables. No explanations outside the Markdown.",

                [OutputFormat.Yaml] =
                    "You are a formatter that outputs strictly valid YAML. " +
                    "No tabs, no comments, no explanations. Only YAML." +
                    "No code blocks, no explanations, no extra text.",

                [OutputFormat.Html] =
                    "You are a formatter that outputs strictly valid HTML. " +
                    "Include proper opening and closing tags. No Markdown, no comments, no explanations." +
                    "No code blocks, no explanations, no extra text.",

                [OutputFormat.Table] =
                    "You are a formatter that outputs data in a tabular format. " +
                    "Use rows and columns to represent the data clearly. " +
                    "No code blocks, no explanations, no extra text.",

                [OutputFormat.Sql] =
                    "You are a formatter that outputs SQL code only. " +
                    "No explanations, no prose, no markdown. Output valid SQL statements.",
            };
    }
}