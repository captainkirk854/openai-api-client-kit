// <copyright file="IOutputFormatValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces
{
    public interface IOutputFormatValidator
    {
        bool IsValidFormat(string content, out string? error);
    }
}
