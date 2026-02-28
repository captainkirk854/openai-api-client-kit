// <copyright file="ResponseSynthesis.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation.Options
{
    using System.Text;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Consolidation.Options.ResponseSynthesis;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Prompts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseSynthesis"/> class.
    /// Consolidation strategy that performs response fusion (synthesis)
    /// using a synthesis model that combines best elements from all responses.
    /// </summary>
    /// <param name="client">The <see cref="ChatClient"/> instance for making API calls.</param>
    public sealed class ResponseSynthesis(ChatClient client)
    {
        private readonly ChatClient client = client;
        private readonly OpenAIModels modelRegistry = new();
        private readonly SingleAiModelExecutor singleModelExecutor = new(client: client);

        /// <summary>
        /// Response Fusion - Judge synthesizes a new response from the best elements of input response(s).
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The list of <see cref="AiModelResponse"/> instances to synthesize.</param>
        /// <param name="synthesisModel">The judge model for synthesis.</param>
        /// <param name="options">The <see cref="AiCallOptions"/> for executing the judge model.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="ResponseSynthesisResult"/> containing the synthesized response
        /// and information about the contributing source responses.
        /// </returns>
        public async Task<ResponseSynthesisResult> ConsolidateWithResponseSynthesisAsync(string prompt,
                                                                                         List<AiModelResponse> responses,
                                                                                         OpenAIModel synthesisModel,
                                                                                         AiCallOptions options,
                                                                                         CancellationToken cancelToken)
        {
            Console.WriteLine($" Asking [{synthesisModel}] to synthesise new response...");

            // Build resynthesis prompt for the synthesis model that includes the original user prompt and all candidate responses to synthesize from ..
            string fusionPrompt = BuildSynthesisPrompt(prompt: prompt, responses: responses);

            // Create synthesis request ..
            ChatCompletionRequest synthesisRequest = new ChatClientRequestBuilder()
                .WithModel(synthesisModel)
                .AddSystemMessage(PromptRegistry.Prompts[PromptId.SetModelSynthesisMode])
                .AddUserMessage(fusionPrompt)
                .Build();

            // Execute synthesis request using judge model to synthesize the best elements from all responses into one excellent response ..
            AiModelResponse synthesisResponse = await this.singleModelExecutor.ExecuteAsync(request: synthesisRequest, options: options, cancelToken: cancelToken);
            string synthesisedContent = synthesisResponse.RawOutput ?? string.Empty;

            return new ResponseSynthesisResult
            {
                SynthesisModel = synthesisModel,
                SynthesisedResponse = synthesisedContent,
                SourceResponses = responses,
                RawFusionOutput = synthesisedContent,
            };
        }

        /// <summary>
        /// Builds the synthesis prompt from the user prompt and candidate responses.
        /// </summary>
        /// <param name="prompt">The original user prompt.</param>
        /// <param name="responses">The candidate <see cref="AiModelResponse"/> instances to synthesise.</param>
        /// <returns>A formatted, synthesis prompt <see cref="string"/>.</returns>
        private static string BuildSynthesisPrompt(string prompt, List<AiModelResponse> responses)
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