// <copyright file="OpenAIModelHelper.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers
{
    using System;
    using System.Collections.Generic;

    public static class OpenAIModelHelper
    {
        /// <summary>
        /// Reverse dictionary: API string → Enum.
        /// </summary>
        private static readonly Dictionary<string, OpenAIModels> ApiToEnumMap = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Primary dictionary: Enum → API string.
        /// </summary>
        private static readonly Dictionary<OpenAIModels, string> EnumToApiMap =
            new()
            {
                // GPT-5 Family
                { OpenAIModels.GPT5_2, "gpt-5.2" },
                { OpenAIModels.GPT5_2_Pro, "gpt-5.2-pro" },
                { OpenAIModels.GPT5, "gpt-5" },
                { OpenAIModels.GPT5_Mini, "gpt-5-mini" },
                { OpenAIModels.GPT5_Nano, "gpt-5-nano" },

                // GPT-4 Family
                { OpenAIModels.GPT4_1, "gpt-4.1" },
                { OpenAIModels.GPT4o, "gpt-4o" },
                { OpenAIModels.GPT4o_Mini, "gpt-4o-mini" },
                { OpenAIModels.GPT4_Turbo, "gpt-4-turbo" },

                // GPT-3.5 Family
                { OpenAIModels.GPT3_5_Turbo, "gpt-3.5-turbo" },

                // Embedding Models
                { OpenAIModels.TextEmbedding_3_Large, "text-embedding-3-large" },
                { OpenAIModels.TextEmbedding_3_Small, "text-embedding-3-small" },

                // Audio Models
                { OpenAIModels.TTS_1, "gpt-tts-1" },
                { OpenAIModels.TTS_1_HD, "gpt-tts-1-hd" },
                { OpenAIModels.Whisper_1, "whisper-1" },

                // Image Models
                { OpenAIModels.DALL_E_3, "dall-e-3" },

                // Open-weight Models
                { OpenAIModels.O1, "o1" },
                { OpenAIModels.O1_Mini, "o1-mini" },
            };

        /// <summary>
        /// Initializes static members of the <see cref="OpenAIModelHelper"/> class.
        /// Static constructor to initialize the reverse mapping dictionary.
        /// </summary>
        static OpenAIModelHelper()
        {
            // Build ApiToEnumMap from EnumToApiMap ..
            foreach (var kvp in EnumToApiMap)
            {
                ApiToEnumMap[kvp.Value] = kvp.Key;
            }
        }

        /// <summary>
        /// Converts the specified OpenAI model enumeration value to its corresponding API string identifier.
        /// </summary>
        /// <param name="model">The OpenAI model enumeration value to convert.</param>
        /// <returns>The API string identifier that corresponds to the specified model.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified model value does not correspond to a known API identifier.</exception>
        public static string ToApiString(OpenAIModels model)
        {
            if (EnumToApiMap.TryGetValue(model, out var apiId))
            {
                return apiId;
            }

            throw new ArgumentOutOfRangeException(nameof(model), model, "Unknown model enum value");
        }

        /// <summary>
        /// Converts the specified API string identifier to its corresponding OpenAI model enumeration value.
        /// </summary>
        /// <param name="apiModelId"></param>
        /// <returns>OpenAIModels object.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified API model ID does not correspond to a known model enum value.</exception>
        public static OpenAIModels FromApiString(string apiModelId)
        {
            if (ApiToEnumMap.TryGetValue(apiModelId, out OpenAIModels model))
            {
                return model;
            }

            throw new ArgumentException($"Unknown API model ID: {apiModelId}", nameof(apiModelId));
        }

        /// <summary>
        /// Returns a collection of all available model names supported by the API.
        /// </summary>
        /// <returns>An enumerable collection of strings, each representing the name of a supported model. The collection will be
        /// empty if no models are available.</returns>
        public static IEnumerable<string> ListAllModels()
        {
            return EnumToApiMap.Values;
        }

        /// <summary>
        /// Returns the latest recommended models for various tasks as a dictionary mapping task names to OpenAIModels enum values.
        /// </summary>
        /// <returns>
        /// A dictionary where the key is the task name and the value is the corresponding <see cref="OpenAIModels"/> enum value representing the latest recommended model for that task.
        /// </returns>
        public static Dictionary<string, OpenAIModels> LatestRecommendedModels()
        {
            return new Dictionary<string, OpenAIModels> {
                { "Chat", OpenAIModels.GPT5_2_Pro },
                { "Embeddings", OpenAIModels.TextEmbedding_3_Large },
                { "Audio", OpenAIModels.TTS_1_HD },
                { "Image", OpenAIModels.DALL_E_3 },
                { "Open-Weight", OpenAIModels.O1 },
            };
        }
    }
}
