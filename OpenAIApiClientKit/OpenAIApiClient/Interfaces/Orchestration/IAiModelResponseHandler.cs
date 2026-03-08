// <copyright file="IAiModelResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Defines methods for handling AI model responses, including processing single, ensemble, and multiple responses.
    /// </summary>
    public interface IAiModelResponseHandler
    {
        /// <summary>
        /// Processes a single AI model response and returns the result as a string.
        /// </summary>
        /// <param name="modelResponse">The see cref="AiModelResponse"/> to be processed.</param>
        /// <returns see cref="string">The processed result from the AI model response.</returns>
        string HandleSingle(AiModelResponse modelResponse);

        /// <summary>
        /// Aggregates responses from multiple AI models into a single ensemble result.
        /// </summary>
        /// <param name="modelResponses">A <see cref="IReadOnlyList{AiModelResponse}"/> to be aggregated.</param>
        /// <returns see cref="string">The aggregated result from the ensemble of AI model responses.</returns>
        string HandleEnsemble(IReadOnlyList<AiModelResponse> modelResponses);

        /// <summary>
        /// Processes a collection of AI model responses and returns the resulting responses.
        /// </summary>
        /// <param name="modelResponses">A <see cref="IReadOnlyList{AiModelResponse}"/> to be aggregated.</param>
        /// <returns see cref="IReadOnlyList{AiModelResponse}">The processed results from the collection of AI model responses.</returns>
        IReadOnlyList<AiModelResponse> HandleResponses(IReadOnlyList<AiModelResponse> modelResponses);
    }
}