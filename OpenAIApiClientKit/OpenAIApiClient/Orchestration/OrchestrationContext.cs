// <copyright file="OrchestrationContext.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;

    public sealed class OrchestrationContext
    {
        /// <summary>
        /// Gets a value indicating whether to use ensemble routing or single model routing.
        /// </summary>
        public bool UseEnsemble
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the single model context if <see cref="UseEnsemble"/> is false.
        /// </summary>
        public SingleModelContext? SingleModelContext
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the ensemble context if <see cref="UseEnsemble"/> is true.
        /// </summary>
        public EnsembleContext? EnsembleContext
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the prompt to be sent to the model(s).
        /// </summary>
        public string Prompt
        {
            get;
            init;
        } = string.Empty;
    }
}
