// <copyright file="OrchestratorDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Registries;

    public static class OrchestratorDemo
    {
        public static async Task RunAsync(ChatClient client, string prompt, CancellationToken cancelToken = default)
        {
            Console.WriteLine("=== AI Orchestrator Demo ===");
            Console.WriteLine();

            // Initialise model registry ..
            OpenAIModels models = new();

            // Initialise Dispatchers to select which model(s) to use ..
            SingleModelDispatcher singleModelDispatcher = new(modelRegistry: models.Registry);
            EnsembleDispatcher ensembleDispatcher = new(modelRegistry: models.Registry);

            // Create the executor stack in which the ensemble executor uses the single-model executor ..
            SingleModelExecutor singleModelExecutor = new(client: client);
            EnsembleExecutor ensembleExecutor = new(singleModelExecutor: singleModelExecutor);

            // Define a model response handler ..
            ResponseHandlerDemo responseHandler = new();

            // Initialise the request builder
            ClientRequestBuilder requestBuilder = new();

            // Create the orchestrator using all the components ..
            Orchestrator orchestrator = new(singleModelDispatcher: singleModelDispatcher,
                                            ensembleDispatcher: ensembleDispatcher,
                                            singleModelExecutor: singleModelExecutor,
                                            ensembleExecutor: ensembleExecutor,
                                            requestBuilder: requestBuilder,
                                            responseHandler: responseHandler);

            // ------------------------------------------------------------
            // Run a single-model request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Single Model Request");
            OrchestrationRequest singleModelRequest = new()
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelDispatchRequest
                {
                    Strategy = SingleModelStrategy.BestReasoning,
                },
            };

            _ = await orchestrator.ProcessAsync(singleModelRequest, cancelToken);

            // ------------------------------------------------------------
            // Run an explicitly defined single-model request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Explicitly defined Single Model Request");
            OrchestrationRequest singleExplicitlyDefinedModelRequest = new()
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelDispatchRequest
                {
                    Strategy = SingleModelStrategy.Explicit,
                    ExplicitModel = OpenAIModel.GPT4o_Mini,
                },
            };

            _ = await orchestrator.ProcessAsync(singleExplicitlyDefinedModelRequest, cancelToken);

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

            _ = await orchestrator.ProcessAsync(ensembleRequest, cancelToken);

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
                        ModelCapability.Text,
                        ModelCapability.Chat,
                    ],
                },
            };

            _ = await orchestrator.ProcessAsync(ensembleCustomRequest, cancelToken);

            // ------------------------------------------------------------
            // Complete
            // ------------------------------------------------------------
            Console.WriteLine("=== Demo Complete ===");
        }
    }
}
