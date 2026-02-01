// <copyright file="FormatDescriptor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
namespace OpenAIApiClient.Models.OutputFormat.General
{
    using OpenAIApiClient.Interfaces.Validators;

    /// <summary>
    /// Format Descriptor.
    /// </summary>
    /// <param name="SystemPrompt"></param>
    /// <param name="Validator"></param>
    public sealed record FormatDescriptor(

        /// <summary>
        /// Record that contains the system prompt to instruct the AI on the desired output format.
        /// </summary>
        string SystemPrompt,

        IOutputFormatValidator Validator
    );
}
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter