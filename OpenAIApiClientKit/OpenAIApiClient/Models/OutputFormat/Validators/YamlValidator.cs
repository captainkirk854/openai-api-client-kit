// <copyright file="YamlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces;
    using YamlDotNet.Core;
    using YamlDotNet.RepresentationModel;

    public sealed class YamlValidator : IOutputFormatValidator
    {
        public bool IsValidFormat(string content, out string? error)
        {
            try
            {
                YamlStream yaml = [];
                yaml.Load(new StringReader(content));
                error = null;
                return true;
            }
            catch (YamlException ex)
            {
                error = $"Invalid {OutputFormat.Yaml}: {ex.Message}";
                return false;
            }
        }
    }
}
