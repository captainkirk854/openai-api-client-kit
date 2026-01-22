// <copyright file="ModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Orchestration
{
    using OpenAIApiClient.Models.Selection;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// A router that selects OpenAI models based on required capabilities.
    /// </summary>
    /// <param name="registry">The model registry to use for routing.</param>
    public sealed class ModelRouter(OpenAIModelRegistry registry)
    {
        /// <summary>
        /// Routes the prompt context to suitable models}based on required capabilities.
        /// </summary>
        /// <param name="context"></param>
        /// <returns><see cref="IReadOnlyList{ModelDescriptor}"/>.</returns>
        /// <exception cref="InvalidOperationException">.</exception>
        public IReadOnlyList<ModelDescriptor> Route(PromptContext context)
        {
            // Find model(s) that satisfy all required capabilities ..
            List<ModelDescriptor> candidates = [.. registry.All.Where(m => context.RequiredCapabilities.All(rc => m.Capabilities.Contains(rc)))];
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException("No model(s) satisfy all required capabilities.");
            }

            // Return the list of candidate model(s) ..
            return candidates;
        }
    }
}