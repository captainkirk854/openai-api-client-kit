// <copyright file="OpenAIModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.OptimalModelSelection;

    public sealed class OpenAIModelRegistry
    {
        // Dictionary to hold model descriptors ..
        private readonly Dictionary<OpenAIModels, OpenAIModelDescriptor> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModelRegistry"/> class.
        /// </summary>
        public OpenAIModelRegistry()
        {
            // Initialize the model descriptors ..
            this.models = new()
            {
                // -------------------------
                // GPT‑5 Family
                // -------------------------
                [OpenAIModels.GPT5_2] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT5_2,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT5_2_Pro] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT5_2_Pro,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT5] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT5,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT5_Mini] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT5_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                },

                [OpenAIModels.GPT5_Nano] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT5_Nano,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                },

                // -------------------------
                // GPT‑4 Family
                // -------------------------
                [OpenAIModels.GPT4_1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4_1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                    },
                },

                [OpenAIModels.GPT4_1_Mini] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4_1_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    },
                },

                [OpenAIModels.GPT4_1_Reasoning] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4_1_Reasoning,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT4_1_Critic] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4_1_Critic,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.Critic,
                    },
                },

                [OpenAIModels.GPT4_Turbo] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4_Turbo,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
                },

                // -------------------------
                // GPT‑4o Family
                // -------------------------
                [OpenAIModels.GPT4o] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4o,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Vision,
                        ModelCapability.AudioIn,
                        ModelCapability.AudioOut,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT4o_Mini] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT4o_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Vision,
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    },
                },

                // -------------------------
                // GPT‑3.5 Family
                // -------------------------
                [OpenAIModels.GPT3_5_Turbo] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.GPT3_5_Turbo,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                },

                // -------------------------
                // Embedding Models
                // -------------------------
                [OpenAIModels.TextEmbedding_3_Large] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TextEmbedding_3_Large,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbedding_3_Small] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TextEmbedding_3_Small,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbedding3Small] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TextEmbedding3Small,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbedding3Large] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TextEmbedding3Large,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbeddingAda002] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TextEmbeddingAda002,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                // -------------------------
                // Audio / TTS / Whisper
                // -------------------------
                [OpenAIModels.TTS_1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS_1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS_1_HD] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS_1_HD,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.Whisper_1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.Whisper_1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                },

                [OpenAIModels.Whisper1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.Whisper1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                },

                [OpenAIModels.TTS1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1HD] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS1HD,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1_1106] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS1_1106,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1HD_1106] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.TTS1HD_1106,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                // -------------------------
                // Image Models
                // -------------------------
                [OpenAIModels.DALL_E_3] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.DALL_E_3,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.ImageGeneration,
                    },
                },

                // -------------------------
                // Open‑weight Models
                // -------------------------
                [OpenAIModels.O1] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.O1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.OpenWeight,
                    },
                },

                [OpenAIModels.O1_Mini] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.O1_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.OpenWeight,
                        ModelCapability.LowCost,
                    },
                },

                // -------------------------
                // Moderation
                // -------------------------
                [OpenAIModels.OmniModerationLatest] = new OpenAIModelDescriptor
                {
                    Model = OpenAIModels.OmniModerationLatest,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Moderation,
                    },
                },
            };
        }

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        public IEnumerable<OpenAIModelDescriptor> All => this.models.Values;

        /// <summary>
        /// Gets the model descriptor for the specified model ..
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="OpenAIModelDescriptor"/>.</returns>
        public OpenAIModelDescriptor Get(OpenAIModels model) => this.models[model];
    }
}
