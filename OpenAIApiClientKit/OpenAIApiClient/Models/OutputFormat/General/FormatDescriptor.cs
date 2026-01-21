// <copyright file="FormatDescriptor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
namespace OpenAIApiClient.Models.OutputFormat.General
{
    using OpenAIApiClient.Interfaces;

    public sealed record FormatDescriptor(

        string SystemPrompt,

        IOutputFormatValidator Validator
    );
}
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter