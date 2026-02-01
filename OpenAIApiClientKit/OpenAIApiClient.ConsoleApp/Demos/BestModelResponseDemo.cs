// <copyright file="BestModelResponseDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Routing;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Console App Demo to demonstrate implementation example for OpenAI optimal model selection.
    /// </summary>
    public static class BestModelResponseDemo
    {
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // Initialise model registry ..
            OpenAIModels registry = new();

            // Initialise Model Routers which filter which model(s) to use ..
            SingleModelRouter singleModelRouter = new(modelRegistry: registry.Registry);
            EnsembleRouter ensembleRouter = new(modelRegistry: registry.Registry);

            // Create the executor stack in which the ensemble executor uses the single-model executor ..
            SingleModelExecutor singleModelExecutor = new(client: client);
            EnsembleExecutor ensembleExecutor = new(singleModelExecutor: singleModelExecutor);

            // Define a model response handler ..
            DemoResponseHandler responseHandler = new();

            // Initialise the request builder
            ClientRequestBuilder requestBuilder = new();

            // Create the orchestrator using all the components ..
            Orchestrator orchestrator = new(singleModelRouter: singleModelRouter, ensembleRouter: ensembleRouter, singleModelExecutor: singleModelExecutor, ensembleExecutor: ensembleExecutor, requestBuilder: requestBuilder, responseHandler: responseHandler);

            // Define prompt context including model requirements ..
            OrchestrationRequest ensembleRequest = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleRouterRequest
                {
                    Strategy = EnsembleRoutingStrategy.Custom,
                    RequiredCapabilities =
                    [
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    ],
                },
            };

            // Execute selected model(s) in asynchronous pipelines ..
            IReadOnlyList<Orchestration.ModelResponse> responses = await orchestrator.ProcessAsync(request: ensembleRequest, cancelToken: cts.Token);
            Orchestration.CollatedModelResponse final = Orchestration.ModelResponseSelector.SelectOptimal(responses: responses);

            // Output computed best response information ..
            Console.WriteLine();
            Console.WriteLine("=== BEST RESPONSE NEW ===");
            Console.WriteLine($"Model: {final.Name}");
            Console.WriteLine($"Output: {final.Content}");

            // Output all model response(s) for results comparison transparency ..
            Console.WriteLine();
            Console.WriteLine("=== SOURCE RESPONSE(S) ===");
            foreach (Orchestration.ModelResponse response in final.SourceResponses)
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