// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    /// <summary>
    /// List of OpenAI models.
    /// list is based on the latest OpenAI documentation as of December 2025.
    /// </summary>
    public enum OpenAIModels
    {
        // -------------------------
        // GPT‑5 Family
        // -------------------------
        GPT5_2,
        GPT5_2_Pro,
        GPT5,
        GPT5_Mini,
        GPT5_Nano,

        // -------------------------
        // GPT‑4 Family
        // -------------------------
        GPT4_1,
        GPT4_1_Mini,
        GPT4_1_Reasoning,
        GPT4_1_Critic,
        GPT4_Turbo,   // Optimized GPT-4 variant

        // -------------------------
        // GPT‑4o Family
        // -------------------------
        GPT4o,        // Multimodal model (text + vision)
        GPT4o_Mini,   // Optimized GPT-4 variant

        // -------------------------
        // GPT‑3.5 Family
        // -------------------------
        GPT3_5_Turbo,

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

        // -------------------------
        // Open‑weight Models
        // -------------------------
        O1,
        O1_Mini,

        // -------------------------
        // Moderation
        // -------------------------
        OmniModerationLatest,
    }
}
