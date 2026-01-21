// <copyright file="HtmlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using HtmlAgilityPack;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces;

    public sealed class HtmlValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(content);

            if (doc.ParseErrors.Any())
            {
                error = $"Invalid {OutputFormat.Html}: " + string.Join("; ", doc.ParseErrors.Select(e => e.Reason));
                return false;
            }

            error = null;
            return true;
        }
    }
}