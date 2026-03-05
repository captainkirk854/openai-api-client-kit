// <copyright file="OpenAIModel.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    /// <summary>
    /// List of OpenAI models.
    /// list is based on the latest OpenAI documentation as of December 2025.
    /// </summary>
    public enum OpenAIModel
    {
        // -------------------------
        // GPT‑5 Family
        // -------------------------
        GPT5_2_Vision,
        GPT5_2_Audio,
        GPT5_2,
        GPT5_2_Pro,
        GPT5_2_Mini,
        GPT5_2_Nano,
        GPT5_1_Vision,
        GPT5_1_Audio,
        GPT5_1,
        GPT5_1_Pro,
        GPT5_1_Mini,
        GPT5_1_Nano,
        GPT5,
        GPT5_Pro,
        GPT5_Vision,
        GPT5_Audio,
        GPT5_Mini,
        GPT5_Nano,

        // -------------------------
        // GPT‑4 Family
        // -------------------------
        GPT4_1,
        GPT4_1_Mini,
        GPT4_Turbo,   // Optimized GPT-4 variant

        // -------------------------
        // GPT‑4o Family
        // -------------------------
        GPT4o,        // Multimodal model (text + vision)
        GPT4o_Mini,   // Optimized GPT-4 variant
        GPT4o_Realtime_Preview, // Real-time content moderation model

        // -------------------------
        // GPT‑3.5 Family
        // -------------------------
        GPT3_5_Turbo,
        GPT3_5_Turbo_16k, // Extended context window
        GPT3_5_Turbo_Instruct, // Instruction-following variant

        // -------------------------
        // GPT‑3 Family
        // -------------------------
        Babbage_002,
        Curie_001,
        Ada_001,
        Text_Davinci_003,
        Davinci_002,

        // -------------------------
        // Embedding Models
        // -------------------------
        TextEmbedding_3_Large,
        TextEmbedding_3_Small,
        TextEmbedding_Ada_002,

        // -------------------------
        // Audio / TTS / Whisper
        // -------------------------
        TTS_1,        // Text-to-Speech
        TTS_1_HD,
        TTS1_1106,
        TTS1HD_1106,
        Whisper_1,    // Speech-to-Text

        // -------------------------
        // Image Models
        // -------------------------
        DALL_E_3,
        DALL_E_2,
        GPT_Image_1, // Multimodal model (text + vision)

        // -------------------------
        // Open‑weight Models
        // -------------------------
        O1,
        O1_Mini,
        O3,
        O3_Mini,
        O4_Mini,

        // -------------------------
        // Moderation
        // -------------------------
        OmniModerationLatest,
        TextModerationLatest,
    }
}
