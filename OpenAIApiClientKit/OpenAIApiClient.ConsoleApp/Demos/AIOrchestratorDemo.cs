// <copyright file="AIOrchestratorDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.OrchestrationNEW04;
    using OpenAIApiClient.Registries;

    public static class AIOrchestratorDemo
    {
        public static async Task RunAsync(ChatClient client, string prompt, CancellationToken cancelToken = default)
        {
            Console.WriteLine("=== AI Orchestrator Demo ===");
            Console.WriteLine();

            // ------------------------------------------------------------
            // 1. Build the model registry (your combined model registry)
            // ------------------------------------------------------------
            OpenAIModelRegistry registry = new();

            // ------------------------------------------------------------
            // 2. Create routers
            // ------------------------------------------------------------
            var singleRouter = new SingleModelRouter(registry.Registry);
            var ensembleRouter = new EnsembleRouter(registry.Registry);

            // ------------------------------------------------------------
            // 3. Create the executor stack
            // ------------------------------------------------------------
            var modelExecutor = new ModelExecutor(client);
            var ensembleExecutor = new EnsembleExecutor(modelExecutor);

            // ------------------------------------------------------------
            // 4. Create the response handler
            // ------------------------------------------------------------
            var responseHandler = new DemoResponseHandler();

            // ------------------------------------------------------------
            // 5. Create the request builder
            // ------------------------------------------------------------
            var requestBuilder = new ClientRequestBuilder();

            // ------------------------------------------------------------
            // 6. Create the orchestrator
            // ------------------------------------------------------------
            var orchestrator = new AIOrchestrator(
                singleRouter,
                ensembleRouter,
                modelExecutor,
                ensembleExecutor,
                responseHandler,
                requestBuilder);

            // ------------------------------------------------------------
            // 7. Run a single-model request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Single Model Request");
            var singleRequest = new OrchestrationRequest
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = (int)Enums.OutputFormat.PlainText,
                SingleModelRequest = new ModelRouterRequest
                {
                    Strategy = ModelRoutingStrategy.BestReasoning,
                },
            };

            var singleResult = await orchestrator.ProcessAsync(singleRequest, cancelToken);
            Console.WriteLine(singleResult);
            Console.WriteLine();

            // ------------------------------------------------------------
            // 8. Run an ensemble request
            // ------------------------------------------------------------
            Console.WriteLine(">>> Ensemble Request");
            var ensembleRequest = new OrchestrationRequest
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = (int)Enums.OutputFormat.PlainText,
                EnsembleRequest = new EnsembleRouterRequest
                {
                    Strategy = EnsembleRoutingStrategy.Reasoning,
                },
            };

            var ensembleResult = await orchestrator.ProcessAsync(ensembleRequest, cancelToken);
            Console.WriteLine(ensembleResult);
            Console.WriteLine();

            Console.WriteLine("=== Demo Complete ===");
        }
    }
}
