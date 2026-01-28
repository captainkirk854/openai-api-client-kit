// <copyright file="JsonValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using System.Text.Json;
    using OpenAIApiClient.Interfaces.Validators;

    public sealed class JsonValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            try
            {
                JsonDocument.Parse(content);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Invalid JSON: {ex.Message}";
                return false;
            }
        }
    }
}
