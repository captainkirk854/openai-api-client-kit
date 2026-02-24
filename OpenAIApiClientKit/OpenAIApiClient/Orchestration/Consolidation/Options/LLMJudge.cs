// <copyright file="LLMJudge.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation.Options
{
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Prompts;

    /// <summary>
    /// Consolidation strategy that uses a judge model (LLM-as-judge)
    /// to select the best response among fan-out model responses.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LLMJudge"/> class.
    /// </remarks>
    /// <param name="client">The <see cref="ChatClient"/> instance for making API calls.</param>
    public sealed class LLMJudge(ChatClient client)
    {
        private readonly ChatClient client = client;
        private readonly OpenAIModels modelRegistry = new();
        private readonly SingleAiModelExecutor singleModelExecutor = new(client: client);

        /// <summary>
        /// Executes the LLM-as-judge consolidation strategy.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of model responses to evaluate.</param>
        /// <param name="judgeModel">The model to use as judge.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An instance of <see cref="LLMJudgeResult"/> containing the selected response,
        /// reasoning, and any associated scoring information.
        /// </returns>
        public async Task<LLMJudgeResult> ConsolidateWithLLMJudgeAsync(
            string prompt,
            List<AiModelResponse> responses,
            OpenAIModel judgeModel,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($" Asking {judgeModel} to judge the responses...");

            List<AiModelResponse> successfulResponses = [.. responses.Where(r => r.IsSuccessful)];

            if (successfulResponses.Count == 0)
            {
                throw new InvalidOperationException("No successful model response(s) to judge");
            }

            // Build judgment prompt
            string judgmentPrompt = BuildJudgmentPrompt(prompt, successfulResponses);

            ChatCompletionRequest judgeRequest = new ClientRequestBuilder()
                .WithModel(judgeModel)
                .AddSystemMessage(PromptRegistry.Prompts[PromptId.SetModelJudgementMode])
                .AddUserMessage(judgmentPrompt)
                .Build();

            AiModelResponse judgeResponse = await this.singleModelExecutor.ExecuteAsync(
                request: judgeRequest,
                cancelToken: cancellationToken);

            string judgeContent = judgeResponse.RawOutput ?? string.Empty;

            // Parse judge response
            ParsedJudgeResponse parseResult = ParseJudgeResponse(judgeContent, successfulResponses);

            return new LLMJudgeResult
            {
                JudgeModel = judgeModel,
                JudgeReasoning = parseResult.Reasoning,
                JudgeScores = parseResult.Scores,
                SelectedIndex = parseResult.SelectedIndex,
                SelectedResponse = parseResult.SelectedResponse,
                RawJudgeOutput = judgeContent,
            };
        }

        /// <summary>
        /// Builds the judgment prompt from the user prompt and candidate responses.
        /// </summary>
        /// <param name="userPrompt">The original user prompt.</param>
        /// <param name="responses">The candidate <see cref="AiModelResponse"/> instances to evaluate.</param>
        /// <returns>A formatted judgment prompt <see cref="string"/>.</returns>
        private static string BuildJudgmentPrompt(string userPrompt, List<AiModelResponse> responses)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("USER PROMPT:");
            stringBuilder.AppendLine(userPrompt);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("CANDIDATE RESPONSES:");
            stringBuilder.AppendLine();

            for (int i = 0; i < responses.Count; i++)
            {
                stringBuilder.AppendLine($"[RESPONSE #{i + 1} from {responses[i].Model.Name}]");
                stringBuilder.AppendLine(responses[i].RawOutput);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("SELECT THE BEST RESPONSE AND PROVIDE YOUR EVALUATION.");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Parses the judge model's response to extract the selected index and reasoning.
        /// </summary>
        /// <param name="judgeContent">The raw judge model output.</param>
        /// <param name="responses">The candidate <see cref="AiModelResponse"/> instances being judged.</param>
        /// <returns>
        /// A <see cref="ParsedJudgeResponse"/> containing the selected index,
        /// selected response, reasoning, and score dictionary.
        /// </returns>
        private static ParsedJudgeResponse ParseJudgeResponse(string judgeContent, List<AiModelResponse> responses)
        {
            try
            {
                // Try to parse JSON using System.Text.Json
                Match jsonMatch = Regex.Match(judgeContent, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    string json = jsonMatch.Value;
                    JsonDocument jsonDocument = JsonDocument.Parse(json);
                    JsonElement rootElement = jsonDocument.RootElement;

                    // Extract selected_model_index
                    if (rootElement.TryGetProperty("selected_model_index", out JsonElement indexElement) &&
                        indexElement.TryGetInt32(out int selectedIndex))
                    {
                        if (selectedIndex >= 0 && selectedIndex < responses.Count)
                        {
                            // Extract reasoning
                            string reasoning = "Judge selected this response.";
                            if (rootElement.TryGetProperty("reasoning", out JsonElement reasoningElement))
                            {
                                reasoning = reasoningElement.GetString() ?? reasoning;
                            }

                            // Extract scores
                            Dictionary<string, int> scores = ExtractScoresFromJsonElement(rootElement);

                            return new ParsedJudgeResponse
                            {
                                SelectedIndex = selectedIndex,
                                SelectedResponse = responses[selectedIndex].RawOutput,
                                Reasoning = reasoning,
                                Scores = scores,
                            };
                        }
                    }
                }

                // Fallback: Select longest response if parsing fails
                int longestIndex = responses.IndexOf(responses.OrderByDescending(r => r.RawOutput.Length).First());
                return new ParsedJudgeResponse
                {
                    SelectedIndex = longestIndex,
                    SelectedResponse = responses[longestIndex].RawOutput,
                    Reasoning = "Parsed as longest response (fallback).",
                    Scores = [],
                };
            }
            catch
            {
                // Final fallback
                return new ParsedJudgeResponse
                {
                    SelectedIndex = 0,
                    SelectedResponse = responses[0].RawOutput,
                    Reasoning = "Error parsing judge response; defaulting to first.",
                    Scores = [],
                };
            }
        }

        /// <summary>
        /// Extracts numerical scores from a <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="rootElement">The root <see cref="JsonElement"/> to extract scores from.</param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> of score names and their <see cref="int"/> values.
        /// </returns>
        private static Dictionary<string, int> ExtractScoresFromJsonElement(JsonElement rootElement)
        {
            Dictionary<string, int> scores = [];

            if (rootElement.TryGetProperty("scores", out JsonElement scoresElement) &&
                scoresElement.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in scoresElement.EnumerateObject())
                {
                    string key = property.Name;
                    if (property.Value.TryGetInt32(out int value))
                    {
                        scores[key] = value;
                    }
                }
            }

            return scores;
        }
    }
}