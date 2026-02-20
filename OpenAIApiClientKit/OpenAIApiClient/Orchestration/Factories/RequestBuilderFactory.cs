// <copyright file="RequestBuilderFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient.Helpers.General;

    /// <summary>
    /// Provides factory methods for creating instances of ClientRequestBuilder with predefined configurations.
    /// </summary>
    public static class RequestBuilderFactory
    {
        /// <summary>
        /// Creates a new instance of ClientRequestBuilder initialized with default settings.
        /// </summary>
        /// <returns see cref="ClientRequestBuilder">A ClientRequestBuilder configured with default values.</returns>
        public static ClientRequestBuilder CreateDefault()
            => new ClientRequestBuilder().WithDefaults();
    }
}