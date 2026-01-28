// <copyright file="CsvValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Validators;

    public sealed class CsvValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
            {
                error = $"{OutputFormat.Csv} must contain a header row and at least one data row.";
                return false;
            }

            string[] headerColumns = lines[0].Split(',');
            if (headerColumns.Length < 2)
            {
                error = $"{OutputFormat.Csv} header must contain at least two columns.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
