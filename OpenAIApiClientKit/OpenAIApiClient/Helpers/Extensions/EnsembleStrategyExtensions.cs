// <copyright file="EnsembleStrategyExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using System.Linq;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// Provides extension methods to support the <see cref="EnsembleStrategy"/> class.
    /// </summary>
    public static class EnsembleStrategyExtensions
    {
        /// <summary>
        /// Gets an array of <see cref="AiModelDescriptor"/> instances corresponding to the models
        /// selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <param name="registry">The model registry to use for resolving available models.</param>
        /// <returns>
        /// An array of <see cref="AiModelDescriptor"/> representing the models selected
        /// for the specified ensemble strategy.
        /// </returns>
        public static AiModelDescriptor[] GetModelInfos(this AiModelStrategy.Ensemble strategy, IAiModelRegistry registry)
        {
            EnsembleDispatchResult result =
                EnsembleStrategies
                    .Get(strategy)
                    .Invoke(registry.GetAll());

            return [.. result.Models];
        }

        /// <summary>
        /// Gets an array of model identifiers corresponding to the models selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <param name="registry">The model registry to use for resolving available models.</param>
        /// <returns>
        /// An array of <see cref="string"/> values representing the model identifiers
        /// (for example, gpt-4.1, gpt-4o-mini) selected for the specified ensemble strategy.
        /// </returns>
        public static string[] GetModelNames(this AiModelStrategy.Ensemble strategy, IAiModelRegistry registry)
        {
            return [.. strategy
                .GetModelInfos(registry)
                .Select(model => model.Name)];
        }

        /// <summary>
        /// Gets an array of upper-cased model identifiers corresponding to the models selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <param name="registry">The model registry to use for resolving available models.</param>
        /// <returns>
        /// An array of <see cref="string"/> values representing the upper-cased model identifiers
        /// selected for the specified ensemble strategy.
        /// </returns>
        public static string[] GetUpperModelNames(this AiModelStrategy.Ensemble strategy, IAiModelRegistry registry)
        {
            return [.. strategy
                .GetModelInfos(registry)
                .Select(model => model.UpperName)];
        }
    }
}
