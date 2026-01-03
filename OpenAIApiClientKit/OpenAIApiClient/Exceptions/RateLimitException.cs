// <copyright file="RateLimitException.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Exceptions
{
    using System;

    /// <summary>
    /// Custom exception to indicate that the rate limit has been exceeded.
    /// </summary>
    public class RateLimitException(string message, HttpResponseMessage response) : Exception(message)
    {
        public HttpResponseMessage Response { get; } = response;
    }
}