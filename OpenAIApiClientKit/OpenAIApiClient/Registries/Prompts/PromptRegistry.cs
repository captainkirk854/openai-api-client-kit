// <copyright file="PromptRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Prompts
{
    using OpenAIApiClient.Enums;

    public static class PromptRegistry
    {
        public static readonly IReadOnlyDictionary<PromptId, string> Prompts =
            new Dictionary<PromptId, string>
            {
                [PromptId.PlainText] =
                    @"You are a formatter that outputs plain text only.
                  No code blocks, no markup, no explanations. Output raw text.",

                [PromptId.Json] =
                    @"You are a formatter that outputs strictly valid JSON.
                  No comments, no trailing commas, no explanations.
                  Return only a JSON object or array.
                  You MUST respond with a valid JSON object only.
                  No explanations, no prose, no markdown.
                  Return strictly valid JSON that matches the expected schema.",

                [PromptId.Csv] =
                    @"You are a formatter that outputs CSV only.
                  Use commas as separators and include a header row.
                  No code blocks, no explanations, no extra text.",

                [PromptId.Xml] =
                    @"You are a formatter that outputs strictly valid XML.
                  Include a single root element.
                  No comments or explanations.
                  No code blocks, no explanations, no extra text.",

                [PromptId.Markdown] =
                    @"You are a formatter that outputs GitHub‑flavored Markdown.
                  Use proper headings, lists, and tables.
                  No explanations outside the Markdown.",

                [PromptId.Yaml] =
                    @"You are a formatter that outputs strictly valid YAML.
                  No tabs, no comments, no explanations.
                  Only YAML.
                  No code blocks, no explanations, no extra text.",

                [PromptId.Html] =
                    @"You are a formatter that outputs strictly valid HTML.
                  Include proper opening and closing tags.
                  No Markdown, no comments, no explanations.
                  No code blocks, no explanations, no extra text.",

                [PromptId.Table] =
                    @"You are a formatter that outputs data in a tabular format.
                  Use rows and columns to represent the data clearly.
                  No code blocks, no explanations, no extra text.",

                [PromptId.Sql] =
                    @"You are a formatter that outputs SQL code only.
                  No explanations, no prose, no markdown.
                  Output valid SQL statements only.",

                [PromptId.SetModelJudgementMode] =
                    @"You are a highly discerning evaluator. 
                  Your task is to select the BEST answer from multiple model responses.
                  
                  Evaluate based on:
                    1. Correctness - Is the factual information accurate?
                    2. Completeness - Does it fully address the user's question?
                    3. Alignment - Is it directly aligned with the user's intent?
                    4. Clarity - Is the explanation clear and well-structured?

                  Respond in the following JSON format:
                    {
                      ""selected_model_index"": <index of the selected response>,
                      ""reasoning"": ""<brief explanation of why this answer is best>"",
                      ""scores"": 
                      {
                        ""correctness"": <1-10>,
                        ""completeness"": <1-10>,
                        ""alignment"": <1-10>,
                        ""clarity"": <1-10>
                      }
                    }",

                [PromptId.SetModelSynthesisMode] =
                    @"You are a master synthesizer. Your task is to combine multiple responses into ONE superior response.

                   Rules:
                    1. Extract the best insights from each response
                    2. Eliminate redundancy
                    3. Resolve any contradictions with explanation
                    4. Maintain accuracy and completeness
                    5. Write clearly and concisely
                    6. Do NOT reference the individual models or 'responses'—write as if this is your original answer

                   Produce a single, cohesive, high-quality response.",
            };
    }
}