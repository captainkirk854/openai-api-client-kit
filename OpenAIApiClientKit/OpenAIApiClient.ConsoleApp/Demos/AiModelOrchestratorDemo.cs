// <copyright file="AiModelOrchestratorDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Factories;
    using OpenAIApiClient.Registries.AiModels;

    public static class AiModelOrchestratorDemo
    {
        public static async Task RunAsync(ChatClient client, string prompt, CancellationToken cancelToken = default)
        {
            Console.WriteLine("=== AI Orchestrator Demo ===");
            Console.WriteLine();

            // Create the orchestrator with all the components using fluent orchestrator builder ..
            Orchestrator orchestrator = new OrchestratorBuilder()
                                            .WithClient(client)
                                            .WithResponseHandler(new AiModelResponseHandlerDemo())
                                            .WithModelRegistry(new OpenAIModels())
                                            .WithRequestBuilder(new ChatClientRequestBuilder().WithDefaults())
                                            .Build();

            // ------------------------------------------------------------
            // Run a single-model request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Single Model Request");
            OrchestrationRequest singleModelRequest = new()
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest
                {
                    Strategy = SingleAiModelStrategy.BestReasoning,
                },
                CallOptions = new AiCallOptions
                {
                    Mode = AiCallMode.BufferedStreaming,
                    OnChunkDeltaContentToken = async (model, chunkDeltaContent) =>
                    {
                        Console.WriteLine($" => {chunkDeltaContent}");   // stream chunk delta(s) to console
                        await Task.Yield();                              // keep it async
                    },
                    AggregateChunkContent = true,
                },
            };

            _ = await orchestrator.ProcessAsync(request: singleModelRequest, cancelToken: cancelToken);

            // ------------------------------------------------------------
            // Run an explicitly defined single-model request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Explicitly defined Single Model Request");
            OrchestrationRequest singleExplicitlyDefinedModelRequest = new()
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest
                {
                    Strategy = SingleAiModelStrategy.Explicit,
                    ExplicitModel = OpenAIModel.GPT4o_Mini,
                },
            };

            _ = await orchestrator.ProcessAsync(request: singleExplicitlyDefinedModelRequest, cancelToken: cancelToken);

            // ------------------------------------------------------------
            // Run an ensemble request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Ensemble Request");
            OrchestrationRequest ensembleRequest = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest
                {
                    Strategy = EnsembleStrategy.Reasoning,
                },
            };

            _ = await orchestrator.ProcessAsync(request: ensembleRequest, cancelToken: cancelToken);

            // ------------------------------------------------------------
            // Run a custom capability ensemble request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Ensemble Custom Model Capability(s) Request");
            OrchestrationRequest ensembleCustomRequest = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.Table,
                EnsembleRequest = new EnsembleDispatchRequest
                {
                    Strategy = EnsembleStrategy.Custom,
                    RequiredCapabilities =
                    [
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                    ],
                },
            };

            _ = await orchestrator.ProcessAsync(request: ensembleCustomRequest, cancelToken: cancelToken);

            // ------------------------------------------------------------
            // Complete
            // ------------------------------------------------------------
            Console.WriteLine("=== Demo Complete ===");
        }
    }
}
