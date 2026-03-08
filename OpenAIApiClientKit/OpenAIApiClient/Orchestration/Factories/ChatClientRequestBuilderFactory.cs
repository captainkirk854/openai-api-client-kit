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
        /// Returns a factory that creates a new <see cref="ChatClientRequestBuilder"/> with default settings on each invocation.
        /// </summary>
        /// <remarks>
        /// This factory method is useful for scenarios where you need to create multiple builders with the same default configuration.
        /// Each call to the returned factory will yield a fresh builder instance initialised
        /// with the defaults defined in the <see cref="ChatClientRequestBuilder.WithDefaults"/> method.
        /// </remarks>
        /// <returns>A <see cref="Func{ChatClientRequestBuilder}"/> that produces a freshly configured builder each time it is called.</returns>
        public static Func<ChatClientRequestBuilder> CreateDefaultFactory() => () => new ChatClientRequestBuilder().WithDefaults();
    }
}