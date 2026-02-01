// <copyright file="BestModelResponseDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.OrchestrationNEW04;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Console App Demo to demonstrate implementation example for OpenAI optimal model selection.
    /// </summary>
    public static class BestModelResponseDemo
    {
        /// <summary>
        /// A demo implementation to get the best model response for the given prompt using multi-model orchestration.
        /// </summary>
        /// <param name="client"><see cref="ChatClient"/> instance.</param>
        /// <param name="prompt">The prompt to send to the model(s).</param>
        /// <param name="cts"><see cref="CancellationTokenSource"/> instance.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // Initialise model registry ..
            OpenAIModelRegistry registry = new();

            // Initialise Model Router which filters which model(s) to use ..
            ModelRouter router = new(registry: registry);

            // Initialise Model Executor ..
            ModelExecutor executor = new(client: client);

            // Initialise Multi-Model Orchestrator ..
            ModelOrchestrator orchestrator = new(router: router, executor: executor);

            // Define prompt context including model requirements ..
            Orchestration.PromptContext context = new()
            {
                Prompt = prompt,
                RequiredCapabilities = new HashSet<ModelCapability>
                {
                    ModelCapability.LowCost,
                    ModelCapability.FastInference,
                },
                OutputFormat = OutputFormat.PlainText,
            };

            // Execute selected model(s) in asynchronous pipelines ..
            Orchestration.CollatedModelResponse final = await orchestrator.ExecuteAsync(context: context, cancelToken: cts.Token);

            // Output computed best response information ..
            Console.WriteLine();
            Console.WriteLine("=== BEST RESPONSE ===");
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

        public static async Task GetBestModelResponseAsyncNew(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // Initialise model registry ..
            OpenAIModelRegistry registry = new();

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
            AIOrchestrator orchestrator = new(singleModelRouter: singleModelRouter, ensembleRouter: ensembleRouter, singleModelExecutor: singleModelExecutor, ensembleExecutor: ensembleExecutor, requestBuilder: requestBuilder, responseHandler: responseHandler);

            // Define prompt context including model requirements ..
            OrchestrationRequest ensembleRequest = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleRouterRequest
                {
                    Strategy = EnsembleRoutingStrategy.Reasoning,
                },
            };

            // Execute selected model(s) in asynchronous pipelines ..
            IReadOnlyList<OrchestrationNEW04.ModelResponse> responses = await orchestrator.ProcessAsync(request: ensembleRequest, cancelToken: cts.Token);
            OrchestrationNEW04.CollatedModelResponse final = OrchestrationNEW04.ModelResponseSelector.SelectOptimal(responses: responses);

            // Output computed best response information ..
            Console.WriteLine();
            Console.WriteLine("=== BEST RESPONSE NEW ===");
            Console.WriteLine($"Model: {final.Name}");
            Console.WriteLine($"Output: {final.Content}");

            // Output all model response(s) for results comparison transparency ..
            Console.WriteLine();
            Console.WriteLine("=== SOURCE RESPONSE(S) ===");
            foreach (OrchestrationNEW04.ModelResponse response in final.SourceResponses)
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