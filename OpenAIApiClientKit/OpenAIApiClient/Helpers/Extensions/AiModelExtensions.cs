// <copyright file="AiModelExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using System.Linq;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// Provides extension methods for working with ensembles of AI models.
    /// </summary>
    public static class AiModelExtensions
    {
        /// <summary>
        /// Gets an array of <see cref="AiModelPropertyRegistryModel"/> instances corresponding to the models
        /// selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <param name="registry">The model registry to use for resolving available models.</param>
        /// <returns>
        /// An array of <see cref="AiModelPropertyRegistryModel"/> representing the models selected
        /// for the specified ensemble strategy.
        /// </returns>
        public static AiModelPropertyRegistryModel[] GetModelInfos(
            this EnsembleStrategy strategy,
            IAiModelRegistryNEW registry)
        {
            EnsembleDispatchResultNEW result =
                EnsembleStrategies
                    .GetNew(strategy)
                    .Invoke(registry.GetAll());

            return result.Models.ToArray();
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
        public static string[] GetModelNames(
            this EnsembleStrategy strategy,
            IAiModelRegistryNEW registry)
        {
            return strategy
                .GetModelInfos(registry)
                .Select(model => model.Name)
                .ToArray();
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
        public static string[] GetUpperModelNames(
            this EnsembleStrategy strategy,
            IAiModelRegistryNEW registry)
        {
            return strategy
                .GetModelInfos(registry)
                .Select(model => model.UpperName)
                .ToArray();
        }
    }
}
