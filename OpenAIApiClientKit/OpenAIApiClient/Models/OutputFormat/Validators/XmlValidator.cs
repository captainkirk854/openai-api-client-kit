// <copyright file="XmlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using System.Xml.Linq;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces;

    public sealed class XmlValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            try
            {
                XDocument.Parse(content);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Invalid {OutputFormat.Xml}: {ex.Message}";
                return false;
            }
        }
    }
}
