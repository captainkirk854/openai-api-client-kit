// <copyright file="PlainTextValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces;

    public sealed class PlainTextValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            if (content.Contains('<') || content.Contains('{') || content.Contains('['))
            {
                error = $"{OutputFormat.PlainText} output must not contain markup or structured data.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
