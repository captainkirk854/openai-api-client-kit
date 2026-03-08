// <copyright file="OutputFormatExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Validators;
    using OpenAIApiClient.Registries.Output;

    /// <summary>
    /// Provides extension methods to support the <see cref="OutputFormat"/> class.
    /// </summary>
    public static class OutputFormatExtensions
    {
        /// <summary>
        /// Validates whether the specified content matches the requirements of the given output format.
        /// </summary>
        /// <param name="content">The content to validate.</param>
        /// <param name="outputFormat">The output format to validate against.</param>
        /// <returns see cref="bool">True if the content is valid for the specified output format; otherwise, false.</returns>
        public static bool IsValidFormat(this string? content, OutputFormat outputFormat)
        {
            IOutputFormatValidator validator = OutputFormats.FormattingPrompts[outputFormat].Validator;
            if (!validator.IsValidFormat(content: content ?? string.Empty, out string? error))
            {
                Console.WriteLine();
                Console.WriteLine($"Warning: The response format is invalid based on the {error}.");
                return false;
            }

            return true;
        }
    }
}
