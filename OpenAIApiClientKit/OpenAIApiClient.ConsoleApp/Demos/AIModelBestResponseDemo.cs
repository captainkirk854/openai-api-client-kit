// <copyright file="AiModelBestResponseDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Consolidation;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Console App Demo to demonstrate implementation example for OpenAI optimal model selection.
    /// </summary>
    public static class AiModelBestResponseDemo
    {
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // Initialise model registry ..
            OpenAIModels models = new();

            // Initialise base definition for request builder ..
            ClientRequestBuilder requestBuilder = new ClientRequestBuilder().WithDefaults();

            // Initialise Dispatchers to select which model(s) to use ..
            SingleAiModelDispatcher singleModelDispatcher = new(registry: models);
            EnsembleDispatcher ensembleDispatcher = new(registry: models);

            // Create the executor stack in which the ensemble executor uses the single-model executor ..
            SingleAiModelExecutor singleModelExecutor = new(client: client);
            EnsembleExecutor ensembleExecutor = new(singleModelExecutor: singleModelExecutor);

            // Define a model response handler ..
            AiModelResponseHandlerDemo responseHandlerDemo = new();

            // Create the orchestrator using all the components ..
            Orchestrator orchestrator = new(requestBuilder: requestBuilder,
                                            singleModelDispatcher: singleModelDispatcher,
                                            ensembleDispatcher: ensembleDispatcher,
                                            singleModelExecutor: singleModelExecutor,
                                            ensembleExecutor: ensembleExecutor,
                                            responseHandler: responseHandlerDemo);

            // Define prompt context including model requirements ..
            OrchestrationRequest ensembleRequest = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest
                {
                    Strategy = EnsembleStrategy.Custom,
                    RequiredCapabilities =
                    [
                        AiModelCapability.LowCost,
                        AiModelCapability.FastInference,
                    ],
                },
            };

            // Execute selected model(s) in asynchronous pipelines ..
            IReadOnlyList<AiModelResponse> responses = await orchestrator.ProcessAsync(request: ensembleRequest, cancelToken: cts.Token);
            AiModelResponseCollator final = AiModelResponseSelector.GetOptimal(responses: responses);

            // Output computed best response information ..
            Console.WriteLine();
            Console.WriteLine("=== BEST RESPONSE ===");
            Console.WriteLine($"Model: {final.Name}");
            Console.WriteLine($"Output: {final.Content}");

            // Output all model response(s) for results comparison transparency ..
            Console.WriteLine();
            Console.WriteLine("=== SOURCE RESPONSE(S) ===");
            foreach (AiModelResponse response in final.SourceResponses)
            {
                Console.WriteLine();
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Model: {response.Model.Name}");
                Console.WriteLine($"Success: {response.IsSuccessful}");
                Console.WriteLine($"TokenCost: {response.TotalTokens}");
                Console.WriteLine($"Cost: {response.EstimatedCost}");
                Console.WriteLine($"Latency: {response.Latency.TotalMilliseconds} ms");
                Console.WriteLine($"Output: {response.RawOutput}");
            }
            Console.WriteLine(new string('-', 80));
        }
    }
}