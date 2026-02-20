// <copyright file="MockAIModelResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using System.Text;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Orchestration;

    /// <summary>
    /// Defines a Demo <see cref="IResponseHandler"/> for processing <see cref="AIModelResponse"/>.
    /// </summary>
    public sealed class MockAIModelResponseHandler : IResponseHandler
    {
        /// <summary>
        /// Handles a single model response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns cref="string">Returns a formatted string representation of the response.</returns>
        public string HandleSingle(AIModelResponse response)
        {
            if (!response.IsSuccessful)
            {
                return $"[ERROR from {response.Model.Name}] {response.ErrorMessage}";
            }

            return $"[Single Model: {response.Model.Name}]\n{response.RawOutput}";
        }

        /// <summary>
        /// Handles ensemble model responses.
        /// </summary>
        /// <param name="responses"></param>
        /// <returns cref="string">Returns a formatted string representation of the ensemble responses.</returns>
        public string HandleEnsemble(IReadOnlyList<AIModelResponse> responses)
        {
            StringBuilder sb = new();
            sb.AppendLine("[Ensemble Output]");

            foreach (AIModelResponse response in responses)
            {
                sb.AppendLine();
                sb.AppendLine($"--- {response.Model.Name} ---");

                if (response.IsSuccessful)
                {
                    sb.AppendLine(response.RawOutput);
                }
                else
                {
                    sb.AppendLine($"ERROR: {response.ErrorMessage}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Shows detailed information about each model response before returning them.
        /// </summary>
        /// <param name="modelResponses"></param>
        /// <returns>IReadOnlyList&lt;ModelResponse&gt;.</returns>
        public IReadOnlyList<AIModelResponse> HandleResponses(IReadOnlyList<AIModelResponse> modelResponses)
        {
            for (int modelResponse = 0; modelResponse < modelResponses.Count; modelResponse++)
            {
                Console.WriteLine();
                Console.WriteLine($"--------- Model Response {modelResponse + 1} ---------");
                Console.WriteLine($"Model: {modelResponses[modelResponse].Model.Name} ({modelResponses[modelResponse].Model.Name})");
                Console.WriteLine($"Successful: {modelResponses[modelResponse].IsSuccessful}");
                Console.WriteLine($"Latency: {modelResponses[modelResponse].Latency.TotalMilliseconds} ms");
                Console.WriteLine($"Total Tokens: {modelResponses[modelResponse].TotalTokens}");
                Console.WriteLine($"Estimated Cost: ${modelResponses[modelResponse].EstimatedCost}");
                if (!modelResponses[modelResponse].IsSuccessful)
                {
                    Console.WriteLine($"Error Message: {modelResponses[modelResponse].ErrorMessage}");
                }
                else
                {
                    Console.WriteLine("Output:");
                    Console.WriteLine();
                    Console.WriteLine(modelResponses[modelResponse].RawOutput);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();

            return modelResponses;
        }
    }
}