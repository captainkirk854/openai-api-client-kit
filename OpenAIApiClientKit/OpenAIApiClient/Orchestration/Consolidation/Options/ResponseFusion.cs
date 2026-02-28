// <copyright file="ResponseFusion.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation.Options
{
    using System.Text;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Consolidation.Options.ResponseFusion;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Prompts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseFusion"/> class.
    /// Consolidation strategy that performs response fusion (synthesis)
    /// using a judge model that combines best elements from all responses.
    /// </summary>
    /// <param name="client">The <see cref="ChatClient"/> instance for making API calls.</param>
    public sealed class ResponseFusion(ChatClient client)
    {
        private readonly ChatClient client = client;
        private readonly OpenAIModels modelRegistry = new();
        private readonly SingleAiModelExecutor singleModelExecutor = new(client: client);

        /// <summary>
        /// Response Fusion - Judge synthesizes best elements from all responses.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of <see cref="AiModelResponse"/> instances to synthesize.</param>
        /// <param name="judgeModel">The judge model for synthesis.</param>
        /// <param name="options">The <see cref="AiCallOptions"/> for executing the judge model.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="ResponseFusionResult"/> containing the synthesized response
        /// and information about the contributing source responses.
        /// </returns>
        public async Task<ResponseFusionResult> ConsolidateWithResponseFusionAsync(string prompt,
                                                                                   List<AiModelResponse> responses,
                                                                                   OpenAIModel judgeModel,
                                                                                   AiCallOptions options,
                                                                                   CancellationToken cancelToken)
        {
            Console.WriteLine($" Asking [{judgeModel}] to synthesize new response...");

            // Build fusion prompt
            string fusionPrompt = BuildFusionPrompt(prompt: prompt, responses: responses);

            // Create fusion request ..
            ChatCompletionRequest fusionRequest = new ChatClientRequestBuilder()
                .WithModel(judgeModel)
                .AddSystemMessage(PromptRegistry.Prompts[PromptId.SetModelSynthesisMode])
                .AddUserMessage(fusionPrompt)
                .Build();

            // Execute fusion request using judge model to synthesize the best elements from all responses into one excellent response ..
            AiModelResponse fusionResponse = await this.singleModelExecutor.ExecuteAsync(request: fusionRequest, options: options, cancelToken: cancelToken);
            string synthesizedContent = fusionResponse.RawOutput ?? string.Empty;

            return new ResponseFusionResult
            {
                JudgeModel = judgeModel,
                SynthesizedResponse = synthesizedContent,
                SourceResponses = responses,
                RawFusionOutput = synthesizedContent,
            };
        }

        /// <summary>
        /// Builds the fusion prompt from the user prompt and candidate responses.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The candidate <see cref="AiModelResponse"/> instances to synthesize.</param>
        /// <returns>A formatted fusion prompt <see cref="string"/>.</returns>
        private static string BuildFusionPrompt(string prompt, List<AiModelResponse> responses)
        {
            StringBuilder sb = new();
            sb.AppendLine("USER QUESTION:");
            sb.AppendLine(prompt);
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
    }
}