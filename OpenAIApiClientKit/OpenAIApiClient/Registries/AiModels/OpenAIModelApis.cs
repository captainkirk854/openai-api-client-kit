// <copyright file="OpenAIModelApis.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using System;
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;

    /// <summary>
    /// OpenAI Model API Registry.
    /// </summary>
    public static class OpenAIModelApis
    {
        /// <summary>
        /// Reverse dictionary: Map API string → Enum.
        /// </summary>
        private static readonly Dictionary<string, OpenAIModel> ApiToEnumMap = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Primary dictionary: Map Enum → API string.
        /// </summary>
        private static readonly Dictionary<OpenAIModel, string> EnumToApiMap =
            new()
            {
                // -------------------------
                // GPT‑5 Family
                // -------------------------
                [OpenAIModel.GPT5_2] = "gpt-5.2",
                [OpenAIModel.GPT5_2_Pro] = "gpt-5.2-pro",
                [OpenAIModel.GPT5] = "gpt-5",
                [OpenAIModel.GPT5_Mini] = "gpt-5-mini",
                [OpenAIModel.GPT5_Nano] = "gpt-5-nano",

                // -------------------------
                // GPT‑4 Family
                // -------------------------
                [OpenAIModel.GPT4_1] = "gpt-4.1",
                [OpenAIModel.GPT4_1_Mini] = "gpt-4.1-mini",
                [OpenAIModel.GPT4_Turbo] = "gpt-4-turbo",

                // -------------------------
                // GPT‑4o Family
                // -------------------------
                [OpenAIModel.GPT4o] = "gpt-4o",
                [OpenAIModel.GPT4o_Mini] = "gpt-4o-mini",

                // -------------------------
                // GPT‑3.5 Family
                // -------------------------
                [OpenAIModel.GPT3_5_Turbo] = "gpt-3.5-turbo",

                // -------------------------
                // Embedding Models
                // -------------------------
                [OpenAIModel.TextEmbedding_3_Large] = "text-embedding-3-large",
                [OpenAIModel.TextEmbedding_3_Small] = "text-embedding-3-small",
                [OpenAIModel.TextEmbedding_Ada_002] = "text-embedding-ada-002",

                // -------------------------
                // Audio / TTS / Whisper
                // -------------------------
                [OpenAIModel.TTS_1] = "tts-1",
                [OpenAIModel.TTS_1_HD] = "tts-1-hd",
                [OpenAIModel.Whisper_1] = "whisper-1",
                [OpenAIModel.TTS1_1106] = "tts-1-1106",
                [OpenAIModel.TTS1HD_1106] = "tts-1-hd-1106",

                // -------------------------
                // Image Models
                // -------------------------
                [OpenAIModel.DALL_E_3] = "dall-e-3",

                // -------------------------
                // Open‑weight Models
                // -------------------------
                [OpenAIModel.O1] = "o1",
                [OpenAIModel.O1_Mini] = "o1-mini",
                [OpenAIModel.O3] = "o3",
                [OpenAIModel.O3_Mini] = "o3-mini",
                [OpenAIModel.O4_Mini] = "o4-mini",

                // -------------------------
                // Moderation
                // -------------------------
                [OpenAIModel.OmniModerationLatest] = "omni-moderation-latest",
            };

        /// <summary>
        /// Initializes static members of the <see cref="OpenAIModelApis"/> class.
        /// Static constructor to initialize the reverse mapping dictionary.
        /// </summary>
        static OpenAIModelApis()
        {
            // Build ApiToEnumMap from EnumToApiMap ..
            foreach (KeyValuePair<OpenAIModel, string> kvp in EnumToApiMap)
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
        public static string ToApiString(this OpenAIModel model)
        {
            if (EnumToApiMap.TryGetValue(model, out var apiId))
            {
                return apiId;
            }

            throw new ArgumentOutOfRangeException(nameof(model), model, "Unknown Model Enum value");
        }

        /// <summary>
        /// Converts the specified API string identifier to its corresponding OpenAI model enumeration value.
        /// </summary>
        /// <param name="apiModelId"></param>
        /// <returns>OpenAIModels object.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified API model ID does not correspond to a known model enum value.</exception>
        public static OpenAIModel FromApiString(string apiModelId)
        {
            if (ApiToEnumMap.TryGetValue(apiModelId, out OpenAIModel model))
            {
                return model;
            }

            throw new ArgumentException($"Unknown API Model ID: [{apiModelId}]", nameof(apiModelId));
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
        /// A dictionary where the key is the task name and the value is the corresponding <see cref="OpenAIModel"/> enum value representing the latest recommended model for that task.
        /// </returns>
        public static Dictionary<string, OpenAIModel> LatestRecommendedModels()
        {
            return new Dictionary<string, OpenAIModel> {
                { "Chat", OpenAIModel.GPT5_2_Pro },
                { "Embeddings", OpenAIModel.TextEmbedding_3_Large },
                { "Audio", OpenAIModel.TTS_1_HD },
                { "Image", OpenAIModel.DALL_E_3 },
                { "Open-Weight", OpenAIModel.O1 },
            };
        }
    }
}
