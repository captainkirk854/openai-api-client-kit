// <copyright file="AiModelExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;

    public static class AiModelExtensions
    {
        /// <summary>
        /// Gets array of <see cref="AiModelDescriptor"/> corresponding to the models selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <returns>Array of <see cref="AiModelDescriptor"/>.</returns>
        public static AiModelDescriptor[] GetModelDescriptors(this EnsembleStrategy strategy)
        {
            // Return AiModelDescriptor(s) corresponding to the specified ensemble strategy by invoking the strategy handler directly and passing the OpenAIModels registry to it, then returning the Models from the resulting EnsembleDispatchResult ..
            return [.. EnsembleStrategies.Get(strategy).Invoke(modelRegistry: new OpenAIModels().GetRegistry()).Models];
        }

        /// <summary>
        /// Gets array of <see cref="OpenAIModel"/> corresponding to the models selected for the specified ensemble strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <returns>Array of <see cref="OpenAIModel"/>.</returns>
        public static OpenAIModel[] GetOpenAIModels(this EnsembleStrategy strategy)
        {
            // Return OpenAIModel(s) corresponding to the specified ensemble strategy ..
            return [.. GetModelDescriptors(strategy: strategy).Select(m => m.Name)];
        }
    }
}
