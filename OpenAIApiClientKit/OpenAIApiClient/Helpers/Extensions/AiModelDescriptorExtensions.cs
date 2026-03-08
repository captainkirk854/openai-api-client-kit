// <copyright file="AiModelDescriptorExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Provides extension methods to support the <see cref="AiModelDescriptor"/> class.
    /// </summary>
    public static class AiModelDescriptorExtensions
    {
        /// <summary>
        /// Determines whether the specified model supports the given capability by checking if the capability score falls
        /// within the provided minimum and maximum thresholds (inclusive).
        /// </summary>
        /// <param name="model">The model to evaluate.</param>
        /// <param name="capability">The capability to check.</param>
        /// <param name="minScore">The minimum score threshold for the capability (default is 0).</param>
        /// <param name="maxScore">The maximum score threshold for the capability (default is 5).</param>
        /// <returns>
        /// <see langword="true"/> if the model supports the specified capability; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasCapability(this AiModelDescriptor model, AiModelCapability capability, int minScore = 0, int maxScore = 5)
        {
            switch (capability)
            {
                // Core Capabilities ..
                case AiModelCapability.AudioIn:
                    {
                        return model.Capabilities.Core.AudioIn.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.AudioOut:
                    {
                        return model.Capabilities.Core.AudioOut.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Chat:
                    {
                        return model.Capabilities.Core.Chat.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Reasoning:
                    {
                        return model.Capabilities.Core.Reasoning.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Text:
                    {
                        return model.Capabilities.Core.Text.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Vision:
                    {
                        return model.Capabilities.Core.Vision.IsBetween(minScore, maxScore);
                    }

                // Advanced Capabilities ..
                case AiModelCapability.Critic:
                    {
                        return model.Capabilities.Advanced.Critic.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Editor:
                    {
                        return model.Capabilities.Advanced.Editor.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Embedding:
                    {
                        return model.Capabilities.Advanced.Embedding.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.ImageGeneration:
                    {
                        return model.Capabilities.Advanced.ImageGeneration.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.JSONMode:
                    {
                        return model.Capabilities.Advanced.JSONMode.IsBetween(minScore, maxScore);
                    }

                // Performance Capabilities ..
                case AiModelCapability.FastInference:
                    {
                        return model.Capabilities.Performance.FastInference.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.HighPerformance:
                    {
                        return model.Capabilities.Performance.HighPerformance.IsBetween(minScore, maxScore);
                    }

                // Operational Capabilities ..
                case AiModelCapability.LowCost:
                    {
                        return model.Capabilities.Operational.LowCost.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.Moderation:
                    {
                        return model.Capabilities.Operational.Moderation.IsBetween(minScore, maxScore);
                    }

                case AiModelCapability.OpenWeight:
                    {
                        return model.Capabilities.Operational.OpenWeight.IsBetween(minScore, maxScore);
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Builds a dictionary keyed by upper-case model name from the supplied registry data.
        /// </summary>
        /// <param name="registryData">The registry data containing models.</param>
        /// <returns>
        /// A dictionary whose keys are upper-case model names and whose values are the corresponding
        /// <see cref="AiModelDescriptor"/> instances.
        /// </returns>
        public static Dictionary<string, AiModelDescriptor> ToUpperNameDictionary(this AiModelPropertyRegistryData registryData)
        {
            ArgumentNullException.ThrowIfNull(registryData);

            if (registryData.Models is null)
            {
                throw new InvalidOperationException("Registry data must contain a non-null Models collection.");
            }

            Dictionary<string, AiModelDescriptor> dictionary = registryData.Models.ToDictionary(keySelector: model => model.UpperName,
                                                                                                elementSelector: model => model,
                                                                                                comparer: StringComparer.OrdinalIgnoreCase);

            return dictionary;
        }
    }
}