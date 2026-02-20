// <copyright file="ExecutorFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient.Orchestration.Execution;

    /// <summary>
    /// Provides factory methods for creating executor instances that interact with chat clients.
    /// </summary>
    public static class ExecutorFactory
    {
        /// <summary>
        /// Creates and returns instances of SingleModelExecutor and EnsembleExecutor using the specified chat client.
        /// </summary>
        /// <param name="client">The ChatClient to use for executor initialization.</param>
        /// <returns see cref="(SingleModelExecutor, EnsembleExecutor)">A tuple containing the created SingleModelExecutor and EnsembleExecutor instances.</returns>
        public static (SingleModelExecutor, EnsembleExecutor) Create(ChatClient client)
        {
            SingleModelExecutor single = new(client: client);
            EnsembleExecutor ensemble = new(singleModelExecutor: single);
            return (single, ensemble);
        }
    }
}