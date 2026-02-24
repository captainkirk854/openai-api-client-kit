// <copyright file="DefaultAiModelResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Response
{
    using OpenAIApiClient.Interfaces.Orchestration;

    /// <summary>
    /// Default implementation of <see cref="IAiModelResponseHandler"/> that performs
    /// minimal processing and returns the raw <see cref="AiModelResponse"/> instances
    /// as produced by the underlying executors.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Acts as a centralized hook to add cross-cutting concerns (logging, filtering, aggregation) in the future.</description>
    ///   </item>
    ///   <item>
    ///     <description>Currently returns the input sequence unchanged, preserving executor behavior.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public sealed class DefaultAiModelResponseHandler : IAiModelResponseHandler
    {
        /// <summary>
        /// Gets the raw output from the specified AI model response.
        /// </summary>
        /// <param name="modelResponse">The AI model response to extract the raw output from.</param>
        /// <returns>The raw output string from the AI model response.</returns>
        /// <exception cref="ArgumentNullException">Thrown when modelResponse is null.</exception>
        public string HandleSingle(AiModelResponse modelResponse)
        {
            // For now, just return the raw output as-is. This is the safest default.
            modelResponse = modelResponse ?? throw new ArgumentNullException(nameof(modelResponse));
            return modelResponse.RawOutput;
        }

        /// <summary>
        /// Concatenates the raw outputs from a collection of AI model responses, separated by line breaks.
        /// </summary>
        /// <param name="modelResponses">A read-only list of AI model responses to process.</param>
        /// <returns>A single string containing the concatenated raw outputs, separated by new lines.</returns>
        /// <exception cref="ArgumentNullException">Thrown when modelResponses is null.</exception>
        public string HandleEnsemble(IReadOnlyList<AiModelResponse> modelResponses)
        {
            // For now, just return the concatenated responses raw output as-is. This is the safest default.
            modelResponses = modelResponses ?? throw new ArgumentNullException(nameof(modelResponses));
            return string.Join(Environment.NewLine, modelResponses.Select(r => r.RawOutput));
        }

        /// <summary>
        /// Processes a collection of <see cref="ModelResponse"/> objects and returns
        /// the sequence that should be exposed to the caller.
        /// </summary>
        /// <param name="modelResponses">The raw <see cref="AiModelResponse"/> instances from the executors.</param>
        /// <returns>
        /// A processed <see cref="IReadOnlyList{T}"/> of <see cref="AiModelResponse"/> objects.
        /// The default implementation returns the responses unchanged.
        /// </returns>
        public IReadOnlyList<AiModelResponse> HandleResponses(IReadOnlyList<AiModelResponse> modelResponses)
        {
            // For now, just return the responses as-is. This is the safest default.
            // In the future, this is the central place to:
            //  - Filter failed responses
            //  - Sort or de-duplicate
            //  - Attach global metadata, logging, etc.
            return modelResponses;
        }
    }
}
