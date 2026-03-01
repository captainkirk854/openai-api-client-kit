// <copyright file="ChatClientRequestBuilderFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient.Helpers;

    /// <summary>
    /// Provides factory method(s) for creating instances of <see cref="ChatClientRequestBuilder"/> with predefined configurations.
    /// </summary>
    public static class ChatClientRequestBuilderFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChatClientRequestBuilder"/> initialized with default settings.
        /// </summary>
        /// <returns see cref="ChatClientRequestBuilder">A ClientRequestBuilder configured with default values.</returns>
        public static ChatClientRequestBuilder CreateDefault()
            => new ChatClientRequestBuilder().WithDefaults();

        /// <summary>
        /// Returns a factory that creates a new <see cref="ChatClientRequestBuilder"/> with default settings on each invocation.
        /// </summary>
        /// <returns>A <see cref="Func{ChatClientRequestBuilder}"/> that produces a freshly configured builder each time it is called.</returns>
        public static Func<ChatClientRequestBuilder> CreateDefaultFactory()
            => () => new ChatClientRequestBuilder().WithDefaults();
    }
}