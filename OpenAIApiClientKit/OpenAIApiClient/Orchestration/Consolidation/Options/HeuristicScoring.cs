// <copyright file="HeuristicScoring.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation.Options
{
    using System.Text.RegularExpressions;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeuristicScoring"/> class.
    /// Consolidation strategy that uses deterministic heuristic scoring
    /// to choose the best response among fan-out model outputs.
    /// </summary>
    public static class HeuristicScoring
    {
        // Preserve original regex constants and names
        private const string RegexMatchNumberPattern = @"\b\d+\b";
        private const string RegexMatchYearPattern = @"\b(19|20)\d{2}\b";
        private const string RegexMatchPercentagePattern = @"\d+\s*%";

        /// <summary>
        /// Heuristic Scoring - Deterministic scoring without additional API calls.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of <see cref="AiModelResponse"/> instances to score.</param>
        /// <returns>
        /// A <see cref="HeuristicScoringResult"/> containing the selected response,
        /// selected model information, and per-model scoring details.
        /// </returns>
        public static HeuristicScoringResult ConsolidateWithHeuristicScoring(string prompt, List<AiModelResponse> responses)
        {
            Console.WriteLine(" Scoring responses using heuristics...");

            // Score each response
            List<ScoredResponse> scoredResponses = [.. responses
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
                x => x.Response.Model.Name,
                x => new ScoredResponseDetail
                {
                    Content = x.Response.RawOutput,
                    TotalScore = x.Score,
                    ScoreBreakdown = GetHeuristicScoresBreakdown(x.Response.RawOutput, prompt),
                });

            return new HeuristicScoringResult
            {
                SelectedResponse = bestScoredResponse.Response.RawOutput,
                SelectedModelIndex = bestScoredResponse.Index,
                SelectedModel = bestScoredResponse.Response.Model.Name,
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
        /// <returns>An <see cref="int"/> score value representing the heuristic quality of the response.</returns>
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
            score += GetHallucinationMarkerCount(response: response) * -50;

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
            score += GetConfidenceMarkerCount(response: response) * 30;

            // 5. Domain-specific keywords (customize per use case)
            // Regex: Regex.Escape() ensures special characters in keywords are treated literally
            // Purpose: Matches domain-specific terms extracted from the user's prompt
            // Keywords are extracted dynamically from the user's question to identify topical relevance
            // Basis: Responses that directly address the user's domain/topic are more relevant and useful
            // Multiplier: +20 points per keyword; shows the response is targeted and directly addresses the user's concern
            score += GetDomainKeywordCount(response: response, prompt: prompt) * 20;

            // 6. Structure score (sentences, paragraphs, lists)
            // Regex: response.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            // Purpose: Counts sentence boundaries to measure response organization
            // Well-structured responses with clear sentences are easier to read and understand
            // Basis: More sentences (up to a reasonable limit) indicate better organization and clarity
            // Multiplier: +10 points per sentence (capped at 200) rewards structure without penalizing brevity
            score += Math.Min(GetSentenceCount(response: response) * 10, 200);

            return score;
        }

        /// <summary>
        /// Gets the detailed score breakdown for a response.
        /// </summary>
        /// <param name="response">The response content to analyze.</param>
        /// <param name="prompt">The original user prompt.</param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing a breakdown of
        /// individual scoring components as <see cref="int"/> values.
        /// </returns>
        private static Dictionary<string, int> GetHeuristicScoresBreakdown(string response, string prompt)
        {
            return new Dictionary<string, int>
            {
                ["Length"] = Math.Min(response.Length / 20, 300),
                ["HallucinationMarkers"] = GetHallucinationMarkerCount(response: response) * -50,
                ["FactualClaims"] = GetFactualClaimCount(response: response) * 15,
                ["ConfidenceMarkers"] = GetConfidenceMarkerCount(response: response) * 30,
                ["DomainKeywords"] = GetDomainKeywordCount(response: response, prompt: prompt) * 20,
                ["Structure"] = Math.Min(GetSentenceCount(response: response) * 10, 200),
            };
        }

        /// <summary>
        /// Gets the distinct count of hallucination markers in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The <see cref="int"/> count of hallucination markers found.</returns>
        private static int GetHallucinationMarkerCount(string response)
        {
            string[] markers = [
                                "as an ai",
                                "cannot",
                                "unable",
                                "might",
                                "perhaps",
                                "possibly",
                                "i think",
                                "you're correct",
                                "that's what I meant",
                                "it's well known that",
                                "it is widely known that",
                                "everyone knows that",
                                "of course it",
                                "clearly it",
                                "obviously it",
                                "it is obvious that",
                                "without a doubt",
                                "undoubtedly",
                                "it always",
                                "it never",
                                "this will always",
                                "it is guaranteed that",
                                "this cannot fail",
                                "this model cannot make mistakes",
                                "there is no scenario where",
                                "according to recent research",
                                "recent studies have shown",
                                "experts agree that",
                                "many experts believe that",
                                "it is generally accepted that",
                                "this is documented as",
                                "the documentation states that",
                                "the official docs say that",
                                "the standard practice is",
                                "the best practice is always",
                                "you can just",
                                "simply",
                                "just need to",
                                "you only need to",
                                "the endpoint is probably",
                                "it should be under",
                                "it should be located at",
                                "it might be named something like",
                                "it likely resides in",
                                "for example in the file",
                                "for instance in the class",
                                "typically this is defined in",
                                "you can assume that",
                                "we can assume that",
                                "in most cases this will",
                                "almost certainly",
                                "virtually always",
                                "essentially the same as",
                                "equivalent to",
                                "identical to",
                                "this is similar to",
                                "this is basically",
                                "in practice this means",
                                "in other words it just",
                                "as you already know",
                                "as is commonly known",
                                "as is obvious from",
                                "as mentioned above"
                               ];
            return markers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the count of factual claims in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The <see cref="int"/> count of factual claim markers found.</returns>
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
        /// <returns>The <see cref="int"/> count of confidence markers found.</returns>
        private static int GetConfidenceMarkerCount(string response)
        {
            string[] markers = ["clearly", "definitely", "certainly", "absolutely", "proven"];
            return markers.Count(marker => Regex.IsMatch(response, marker, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the count of domain-specific keywords in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <param name="prompt">The original user prompt for context.</param>
        /// <returns>The <see cref="int"/> count of domain keywords found.</returns>
        private static int GetDomainKeywordCount(string response, string prompt)
        {
            List<string> keywords = ExtractCustomDomainKeywords(prompt: prompt);
            return keywords.Count(kw => Regex.IsMatch(response, Regex.Escape(kw), RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets the sentence count in a response.
        /// </summary>
        /// <param name="response">The response to analyze.</param>
        /// <returns>The <see cref="int"/> count of sentences.</returns>
        private static int GetSentenceCount(string response)
        {
            return response.Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Extracts domain-specific keywords from the user prompt.
        /// </summary>
        /// <remarks>
        /// Customize this method to extract relevant keywords based on the expected domain of the prompt.
        /// </remarks>
        /// <param name="prompt">The user prompt to extract keywords from.</param>
        /// <returns>A <see cref="List{T}"/> of domain keyword <see cref="string"/> values found in the prompt.</returns>
        private static List<string> ExtractCustomDomainKeywords(string prompt)
        {
            string[] commonKeywords = ["machine learning", "deep learning", "neural network", "algorithm", "data", "model", "training"];
            return [.. commonKeywords.Where(kw => prompt.Contains(kw, StringComparison.OrdinalIgnoreCase))];
        }
    }
}