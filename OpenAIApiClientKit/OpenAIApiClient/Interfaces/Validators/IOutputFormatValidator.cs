// <copyright file="IOutputFormatValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Validators
{
    /// <summary>
    /// Defines a contract for validating the format of output content.
    /// </summary>
    public interface IOutputFormatValidator
    {
        bool IsValidFormat(string content, out string? error);
    }
}
