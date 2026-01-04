// <copyright file="SimpleModelSelectionDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    using OpenAIApiClient.Enums;

    public static class SimpleModelSelectionDemo
    {
        public static async Task RunAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // Initialise model registry ..
            OpenAIModelRegistry registry = new();

            // Initialise Model Router which filters which model(s) to use ..
            OpenAIModelRouter router = new(registry: registry);

            // Initialise the model executor ..
            OpenAIModelExecutor executor = new(client: client);

            // Initialise the multi-model orchestrator ..
            MultiModelOrchestrator orchestrator = new(router: router, executor: executor);

            // Define prompt context including model requirements ..
            PromptContext context = new()
            {
                Prompt = prompt,
                RequiredCapabilities = new HashSet<ModelCapability>
                {
                    ModelCapability.LowCost,
                    ModelCapability.FastInference,
                },
                DesiredOutputFormat = "text", // e.g., "text", "json", "markdown"
            };

            // Execute the multi-model pipeline
            FinalResponse final = await orchestrator.ExecuteAsync(context: context, token: cts.Token);

            // Output the final aggregated answer
            Console.WriteLine("=== BEST RESPONSE ===");
            Console.WriteLine(final.ToString());
            Console.WriteLine(final.Content);
            Console.WriteLine();

            // Output all model responses for transparency
            Console.WriteLine("=== SOURCE RESPONSE(S) ===");
            foreach (ModelResponse r in final.SourceResponses)
            {
                Console.WriteLine($"Model: {r.Model.Name}");
                Console.WriteLine($"Success: {r.IsSuccessful}");
                Console.WriteLine($"Tokens: {r.TotalTokens}");
                Console.WriteLine($"Cost: {r.EstimatedCost}");
                Console.WriteLine($"Latency: {r.Latency.TotalMilliseconds} ms");
                Console.WriteLine($"Output: {r.RawOutput}");
                Console.WriteLine(new string('-', 80));
            }
        }
    }
}
