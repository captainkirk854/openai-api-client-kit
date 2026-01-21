// <copyright file="MarkdownValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces;

    public sealed class MarkdownValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            if (!content.Contains('#') && !content.Contains('*') && !content.Contains('|'))
            {
                error = $"{OutputFormat.Markdown} must contain at least one heading, list, or table.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
