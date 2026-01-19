// <copyright file="OutputFormat.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Enums
{
    public enum OutputFormat
    {
        // Textual formats
        PlainText,
        Markdown,
        Html,
        Json,
        Xml,
        Csv,
        Yaml,

        // Structured / semantic formats
        StructuredObject,
        KeyValuePairs,
        Table,

        // Code formats
        Code,
        Sql,
        Regex,

        // Audio formats
        AudioMp3,
        AudioWav,
        AudioOgg,
        AudioFlac,

        // Image formats
        ImagePng,
        ImageJpeg,
        ImageWebp,
        ImageSvg,

        // Video formats (for completeness)
        VideoMp4,
        VideoWebm,

        // Embedding / vector formats
        EmbeddingVector,

        // Special-purpose formats
        Binary,
        Base64,
        Url,
    }
}
