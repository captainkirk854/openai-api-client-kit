// <copyright file="BestModelResponseDemo.cs" company="854 Things (tm)">
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

    /// <summary>
    /// Console App Demo to demonstrate implementation example for OpenAI optimal model selection.
    /// </summary>
    public static class BestModelResponseDemo
    {
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
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
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    ],
                },
            };

            // Execute selected model(s) in asynchronous pipelines ..
            IReadOnlyList<ModelResponse> responses = await orchestrator.ProcessAsync(request: ensembleRequest, cancelToken: cts.Token);
            CollatedModelResponse final = ModelResponseSelector.SelectOptimal(responses: responses);

            // Output computed best response information ..
            Console.WriteLine();
            Console.WriteLine("=== BEST RESPONSE ===");
            Console.WriteLine($"Model: {final.Name}");
            Console.WriteLine($"Output: {final.Content}");

            // Output all model response(s) for results comparison transparency ..
            Console.WriteLine();
            Console.WriteLine("=== SOURCE RESPONSE(S) ===");
            foreach (ModelResponse response in final.SourceResponses)
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