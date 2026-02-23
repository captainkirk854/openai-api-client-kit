// <copyright file="AdvancedEnsembleConsolidation.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation
{
    using System.Diagnostics;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Models.Consolidation.Options.ResponseFusion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Prompts;

    /// <summary>
    /// Advanced consolidation strategies for fan-out multi-model responses.
    /// Supports: LLM Judge, Heuristic Scoring, and Response Fusion.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AdvancedEnsembleConsolidation"/> class.
    /// </remarks>
    /// <param name="client">The ChatClient instance for making API calls.</param>
    public class AdvancedEnsembleConsolidation(ChatClient client)
    {
        private const string RegexMatchNumberPattern = @"\b\d+\b";
        private const string RegexMatchYearPattern = @"\b(19|20)\d{2}\b";
        private const string RegexMatchPercentagePattern = @"\d+\s*%";
        private readonly ChatClient client = client;
        private readonly OpenAIModels modelRegistry = new();
        private readonly SingleAiModelExecutor singleModelExecutor = new(client: client);

        /// <summary>
        /// Fan-out to N models and consolidate using specified strategy.
        /// </summary>
        /// <param name="prompt">The user prompt to send to all models.</param>
        /// <param name="fanoutModels">The array of models to fan-out to.</param>
        /// <param name="consolidationMode">The consolidation strategy to use.</param>
        /// <param name="judgeModel">The judge model for LLM-based strategies (optional).</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>An AdvancedConsolidatedResponse with the consolidated result.</returns>
        public async Task<AdvancedConsolidatedResponse> FanOutAndConsolidateAdvancedAsync(string prompt, OpenAIModel[] fanoutModels, ConsolidationMode consolidationMode, OpenAIModel? judgeModel = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            if (fanoutModels.Length == 0)
            {
                throw new ArgumentException("At least one model must be specified", nameof(fanoutModels));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                // Step 1: Fan-out to all models
                Console.WriteLine($" Fanning out to {fanoutModels.Length} models...");
                List<AiModelResponse> fanoutResponses = await this.FanOutToModelsAsync(prompt, fanoutModels, cancellationToken);

                // Step 2: Consolidate based on strategy
                string consolidatedContent;
                object? consolidationMetadata = null;

                switch (consolidationMode)
                {
                    case ConsolidationMode.LLMAsJudge:
                        if (judgeModel == null)
                        {
                            throw new ArgumentException("A Judge model is required for LLM Judge consolidation!", nameof(judgeModel));
                        }

                        LLMJudgeResult judgeResult = await this.ConsolidateWithLLMJudgeAsync(prompt, fanoutResponses, judgeModel.Value, cancellationToken);
                        consolidatedContent = judgeResult.SelectedResponse;
                        consolidationMetadata = judgeResult;
                        break;

                    case ConsolidationMode.HeuristicScoring:
                        HeuristicScoringResult heuristicResult = ConsolidateWithHeuristicScoring(prompt, fanoutResponses);
                        consolidatedContent = heuristicResult.SelectedResponse;
                        consolidationMetadata = heuristicResult;
                        break;

                    case ConsolidationMode.ResponseFusion:
                        if (judgeModel == null)
                        {
                            throw new ArgumentException("A Judge model is required for Response Fusion", nameof(judgeModel));
                        }

                        ResponseFusionResult fusionResult = await this.ConsolidateWithResponseFusionAsync(prompt, fanoutResponses, judgeModel.Value, cancellationToken);
                        consolidatedContent = fusionResult.SynthesizedResponse;
                        consolidationMetadata = fusionResult;
                        break;

                    default:
                        throw new ArgumentException($"Unknown consolidation mode: {consolidationMode}");
                }

                stopwatch.Stop();

                return new AdvancedConsolidatedResponse
                {
                    UserPrompt = prompt,
                    ConsolidatedContent = consolidatedContent,
                    FanoutResponses = fanoutResponses,
                    ConsolidationMode = consolidationMode,
                    ConsolidationMetadata = consolidationMetadata,
                    TotalLatency = stopwatch.Elapsed,
                    SuccessCount = fanoutResponses.Count(r => r.IsSuccessful),
                    FailureCount = fanoutResponses.Count(r => !r.IsSuccessful),
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException(
                    $"Advanced fan-out and consolidation failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}",
                    ex);
            }
        }

        // ============================================================================
        // OPTION 1: LLM AS JUDGE
        // ============================================================================

        /// <summary>
        /// Option 1: LLM As Judge - Asks a judge model to select the best answer.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of model responses to evaluate.</param>
        /// <param name="judgeModel">The model to use as judge.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The LLMJudgeResult with the selected response and reasoning.</returns>
        private async Task<LLMJudgeResult> ConsolidateWithLLMJudgeAsync(string prompt, List<AiModelResponse> responses, OpenAIModel judgeModel, CancellationToken cancellationToken)
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

            AiModelResponse judgeResponse = await this.singleModelExecutor.ExecuteAsync(request: judgeRequest, cancelToken: cancellationToken);

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
        /// <param name="responses">The candidate responses to evaluate.</param>
        /// <returns>A formatted judgment prompt string.</returns>
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
        /// <param name="responses">The candidate responses being judged.</param>
        /// <returns>A ParsedJudgeResponse with extracted data.</returns>
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
        /// Extracts numerical scores from a JsonElement.
        /// </summary>
        /// <param name="rootElement">The root JsonElement to extract scores from.</param>
        /// <returns>A dictionary of score names and their values.</returns>
        private static Dictionary<string, int> ExtractScoresFromJsonElement(JsonElement rootElement)
        {
            Dictionary<string, int> scores = [];

            if (rootElement.TryGetProperty("scores", out JsonElement scoresElement) && scoresElement.ValueKind == JsonValueKind.Object)
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

        // ============================================================================
        // OPTION 2: HEURISTIC SCORING
        // ============================================================================

        /// <summary>
        /// Option 2: Heuristic Scoring - Deterministic scoring without additional API calls.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of responses to score.</param>
        /// <returns>A HeuristicScoringResult with scoring details.</returns>
        private static HeuristicScoringResult ConsolidateWithHeuristicScoring(string prompt, List<AiModelResponse> responses)
        {
            Console.WriteLine(" Scoring responses using heuristics...");

            List<AiModelResponse> successfulResponses = [.. responses.Where(r => r.IsSuccessful)];

            if (successfulResponses.Count == 0)
            {
                throw new InvalidOperationException("No successful model responses to score");
            }

            // Score each response
            List<ScoredResponse> scoredResponses = [.. successfulResponses
                .Select((response, idx) =>
                {
                    int score = CalculateHeuristicScore(response.RawOutput, prompt);
                    return new ScoredResponse
                    {
                        Index = idx,
                        Response = response,
                        Score = score,
                    };
                })];

            ScoredResponse bestScoredResponse = scoredResponses.OrderByDescending(x => x.Score).First();

            Dictionary<string, ScoredResponseDetail> scoredResponsesDict = scoredResponses.ToDictionary(
                x => x.Response.Model.Name.ToApiString(),
                x => new ScoredResponseDetail
                {
                    Content = x.Response.RawOutput,
                    TotalScore = x.Score,
                    ScoreBreakdown = CalculateScoreBreakdown(x.Response.RawOutput, prompt),
                });

            return new HeuristicScoringResult
            {
                SelectedResponse = bestScoredResponse.Response.RawOutput,
                SelectedModelIndex = bestScoredResponse.Index,
                SelectedModel = bestScoredResponse.Response.Model.Name.ToApiString(),
                ScoredResponses = scoredResponsesDict,
            };
        }

        /// <summary>
        /// Calculates a heuristic score for a response based on multiple factors.
        /// </summary>
        /// <remarks>
        /// Scoring Methodology:
        /// - Length: Rewards comprehensive responses (up to 300 points).
        /// - Hallucination Markers: Penalizes uncertainty and AI disclaimers (-50 points each).
        /// - Factual Claims: Bonus for concrete data points like numbers, years, and percentages (+15 points each).
        /// - Confidence Markers: Rewards assertive language (+30 points each).
        /// - Domain Keywords: Bonus for topic-specific terminology matched to the user's prompt (+20 points each).
        /// - Structure: Rewards well-organized responses with clear sentence boundaries (+10 points per sentence, capped at 200).
        /// </remarks>
        /// <param name="response">The response content to score.</param>
        /// <param name="prompt">The original user prompt for context.</param>
        /// <returns>An integer score value.</returns>
        private static int CalculateHeuristicScore(string response, string prompt)
        {
            int score = 0;

            // 1. Length score (favor comprehensive answers)
            // Basis: Longer responses tend to be more thorough, but cap at 300 to avoid penalizing conciseness
            // Multiplier: 1/20 (length / 20) gives proportional weight; cap at 300 points prevents length alone from dominating score
            score += Math.Min(response.Length / 20, 300); // Cap at 300 points

            // 2. Penalty for hallucination markers
            // Regex: @"as an ai|cannot|unable|might|perhaps|possibly|i think" (case-insensitive)
            // Purpose: Detects phrases that indicate uncertainty, hedging, or AI self-references
            // These suggest the model is unsure or disclaiming responsibility for its answer
            // Basis: Each marker indicates lower confidence in the response content
            // Multiplier: -50 points per marker penalizes hedging language; these reduce perceived reliability
            string[] hallucMarkers = ["as an ai", "cannot", "unable", "might", "perhaps", "possibly", "i think"];
            int hallucCount = hallucMarkers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
            score -= hallucCount * 50;

            // 3. Bonus for factual claims (numbers, dates, statistics)
            // Regex patterns:
            //   - @"\b\d+\b": Matches any standalone integer (word boundaries ensure "123" in "test123ing" is not counted)
            //   - @"\b(19|20)\d{2}\b": Matches 4-digit years starting with 19 or 20 (1900-2099 range)
            //   - @"\d+\s*%": Matches percentages (e.g., "95%" or "95 %")
            // Purpose: Identifies concrete data points that suggest the response contains factual information
            // Basis: Responses with specific facts (numbers, dates, percentages) are typically more credible
            // Multiplier: Years get +30 (2x multiplier), other numbers and percentages get +15; this weights temporal references higher
            MatchCollection numberMatches = Regex.Matches(response, RegexMatchNumberPattern);
            MatchCollection yearMatches = Regex.Matches(response, RegexMatchYearPattern);
            MatchCollection percentMatches = Regex.Matches(response, RegexMatchPercentagePattern);
            score += (numberMatches.Count + (yearMatches.Count * 2) + percentMatches.Count) * 15;

            // 4. Confidence markers
            // Regex: @"clearly|definitely|certainly|absolutely|proven" (case-insensitive)
            // Purpose: Detects assertive language that indicates high confidence in the answer
            // These words suggest the model believes strongly in the content it's providing
            // Basis: Confident, assertive language (when combined with factual accuracy) is preferred over hedging
            // Multiplier: +30 points per marker; confidence language signals stronger, more reliable answers
            string[] confidenceMarkers = ["clearly", "definitely", "certainly", "absolutely", "proven"];
            int confidenceCount = confidenceMarkers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
            score += confidenceCount * 30;

            // 5. Domain-specific keywords (customize per use case)
            // Regex: Regex.Escape() ensures special characters in keywords are treated literally
            // Purpose: Matches domain-specific terms extracted from the user's prompt
            // Keywords are extracted dynamically from the user's question to identify topical relevance
            // Basis: Responses that directly address the user's domain/topic are more relevant and useful
            // Multiplier: +20 points per keyword; shows the response is targeted and directly addresses the user's concern
            List<string> domainKeywords = ExtractDomainKeywords(prompt);
            int keywordMatches = domainKeywords.Count(kw => Regex.IsMatch(response, Regex.Escape(kw), RegexOptions.IgnoreCase));
            score += keywordMatches * 20;

            // 6. Structure score (sentences, paragraphs, lists)
            // Regex: response.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            // Purpose: Counts sentence boundaries to measure response organization
            // Well-structured responses with clear sentences are easier to read and understand
            // Basis: More sentences (up to a reasonable limit) indicate better organization and clarity
            // Multiplier: +10 points per sentence (capped at 200) rewards structure without penalizing brevity
            int sentenceCount = response.Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries).Length;
            score += Math.Min(sentenceCount * 10, 200);

            return score;
        }

        /// <summary>
        /// Calculates a detailed score breakdown for a response.
        /// </summary>
        /// <param name="response">The response content to analyze.</param>
        /// <param name="userPrompt">The original user prompt.</param>
        /// <returns>A dictionary with score breakdown by factor.</returns>
        private static Dictionary<string, int> CalculateScoreBreakdown(string response, string userPrompt)
        {
            return new Dictionary<string, int>
            {
                ["Length"] = Math.Min(response.Length / 20, 300),
                ["HallucinationMarkers"] = -GetHallucinationMarkerCount(response) * 50,
                ["FactualClaims"] = GetFactualClaimCount(response) * 15,
                ["ConfidenceMarkers"] = GetConfidenceMarkerCount(response) * 30,
                ["DomainKeywords"] = GetDomainKeywordCount(response, userPrompt) * 20,
                ["Structure"] = Math.Min(GetSentenceCount(response) * 10, 200),
            };
        }

        /// <summary>
        /// Gets the count of hallucination markers in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The count of hallucination markers found.</returns>
        private static int GetHallucinationMarkerCount(string response)
        {
            string[] markers = ["as an ai", "cannot", "unable", "might", "perhaps", "possibly", "i think"];
            return markers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the count of factual claims in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The count of factual claim markers found.</returns>
        private static int GetFactualClaimCount(string response)
        {
            return Regex.Matches(response, RegexMatchNumberPattern).Count
                 + (Regex.Matches(response, RegexMatchYearPattern).Count * 2)
                 + Regex.Matches(response, RegexMatchPercentagePattern).Count;
        }

        /// <summary>
        /// Gets the count of confidence markers in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The count of confidence markers found.</returns>
        private static int GetConfidenceMarkerCount(string response)
        {
            string[] markers = ["clearly", "definitely", "certainly", "absolutely", "proven"];
            return markers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the count of domain-specific keywords in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <param name="userPrompt">The original user prompt for context.</param>
        /// <returns>The count of domain keywords found.</returns>
        private static int GetDomainKeywordCount(string response, string userPrompt)
        {
            List<string> keywords = ExtractDomainKeywords(userPrompt);
            return keywords.Count(kw => Regex.IsMatch(response, Regex.Escape(kw), RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the sentence count in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The count of sentences.</returns>
        private static int GetSentenceCount(string response)
        {
            return response.Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Extracts domain-specific keywords from the user prompt.
        /// </summary>
        /// <param name="prompt">The user prompt to extract keywords from.</param>
        /// <returns>A list of domain keywords found in the prompt.</returns>
        private static List<string> ExtractDomainKeywords(string prompt)
        {
            // Customize based on your domain
            string[] commonKeywords = ["machine learning", "deep learning", "neural network", "algorithm", "data", "model", "training"];
            return [.. commonKeywords.Where(kw => prompt.Contains(kw, StringComparison.OrdinalIgnoreCase))];
        }

        // ============================================================================
        // OPTION 3: RESPONSE FUSION
        // ============================================================================

        /// <summary>
        /// Option 3: Response Fusion - Judge synthesizes best elements from all responses.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of responses to synthesize.</param>
        /// <param name="judgeModel">The judge model for synthesis.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A ResponseFusionResult with the synthesized response.</returns>
        private async Task<ResponseFusionResult> ConsolidateWithResponseFusionAsync(string prompt, List<AiModelResponse> responses, OpenAIModel judgeModel, CancellationToken cancellationToken)
        {
            Console.WriteLine($" Asking {judgeModel} to synthesize responses...");

            List<AiModelResponse> successfulResponses = [.. responses.Where(r => r.IsSuccessful)];

            if (successfulResponses.Count == 0)
            {
                throw new InvalidOperationException("No successful model responses to fuse");
            }

            // Build fusion prompt
            string fusionPrompt = BuildFusionPrompt(prompt, successfulResponses);

            ChatCompletionRequest fusionRequest = new ClientRequestBuilder()
                .WithModel(judgeModel)
                .AddSystemMessage(PromptRegistry.Prompts[PromptId.SetModelSynthesisMode])
                .AddUserMessage(fusionPrompt)
                .Build();

            AiModelResponse fusionResponse = await this.singleModelExecutor.ExecuteAsync(request: fusionRequest, cancelToken: cancellationToken);

            string synthesizedContent = fusionResponse.RawOutput ?? string.Empty;

            return new ResponseFusionResult
            {
                JudgeModel = judgeModel,
                SynthesizedResponse = synthesizedContent,
                SourceResponses = successfulResponses,
                RawFusionOutput = synthesizedContent,
            };
        }

        /// <summary>
        /// Builds the fusion prompt from the user prompt and candidate responses.
        /// </summary>
        /// <param name="userPrompt">The original user prompt.</param>
        /// <param name="responses">The candidate responses to synthesize.</param>
        /// <returns>A formatted fusion prompt string.</returns>
        private static string BuildFusionPrompt(string userPrompt, List<AiModelResponse> responses)
        {
            StringBuilder sb = new();
            sb.AppendLine("USER QUESTION:");
            sb.AppendLine(userPrompt);
            sb.AppendLine();
            sb.AppendLine("CANDIDATE ANSWERS TO SYNTHESIZE:");
            sb.AppendLine();

            for (int i = 0; i < responses.Count; i++)
            {
                sb.AppendLine($"[ANSWER {i + 1}]");
                sb.AppendLine(responses[i].RawOutput);
                sb.AppendLine();
            }

            sb.AppendLine("SYNTHESIZE: Combine the best parts of all answers above into ONE excellent response.");

            return sb.ToString();
        }

        // ============================================================================
        // HELPER: FAN-OUT TO MODELS
        // ============================================================================

        /// <summary>
        /// Fan-out: Send the prompt to N models in parallel using SingleAiModelExecutor.
        /// </summary>
        /// <param name="prompt">The prompt to send to all models.</param>
        /// <param name="models">The models to query in parallel.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of AiModelResponse objects from all models.</returns>
        private async Task<List<AiModelResponse>> FanOutToModelsAsync(string prompt, OpenAIModel[] models, CancellationToken cancellationToken)
        {
            List<Task<AiModelResponse>> tasks = [];

            foreach (OpenAIModel model in models)
            {
                Task<AiModelResponse> task = this.QuerySingleModelAsync(prompt, model, cancellationToken);
                tasks.Add(task);
            }

            AiModelResponse[] results = await Task.WhenAll(tasks);
            return [.. results];
        }

        /// <summary>
        /// Queries a single model using the SingleAiModelExecutor.
        /// </summary>
        /// <param name="prompt">The prompt to send to the model.</param>
        /// <param name="model">The model to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A AiModelResponse from the model.</returns>
        private async Task<AiModelResponse> QuerySingleModelAsync(string prompt, OpenAIModel model, CancellationToken cancellationToken)
        {
            AiModelDescriptor modelDescriptor = this.modelRegistry.Get(model);

            ChatCompletionRequest request = new ClientRequestBuilder()
                .WithModel(model)
                .AddSystemMessage("You are a helpful, accurate, and concise assistant.")
                .AddUserMessage(prompt)
                .UsingMaxTokens(2000)
                .Build();

            try
            {
                AiModelResponse response = await this.singleModelExecutor.ExecuteAsync(request: request, cancelToken: cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                return new AiModelResponse
                {
                    Model = modelDescriptor,
                    RawOutput = string.Empty,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Latency = TimeSpan.Zero,
                    TotalTokens = 0,
                    EstimatedCost = 0,
                };
            }
        }
    }
}