// <copyright file="OpenAIModelsNEW.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// New model registry that is constructed from a pre-built descriptor dictionary.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OpenAIModelsNEW"/> class.
    /// </remarks>
    /// <param name="models">
    /// The descriptor dictionary keyed by <see cref="OpenAIModel"/>.
    /// This must contain one entry per supported model.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="models"/> is null.</exception>
    public sealed class OpenAIModelsNEW(Dictionary<OpenAIModel, AiModelDescriptor> models) //: IAiModelRegistry<AiModelDescriptor>
    {
        private readonly Dictionary<OpenAIModel, AiModelDescriptor> models = models ?? throw new ArgumentNullException(nameof(models));

        /// <summary>
        /// Gets the descriptor for a given model.
        /// </summary>
        /// <param name="model">The model key.</param>
        public AiModelDescriptor this[OpenAIModel model] => this.models[model];

        /// <summary>
        /// Returns all model descriptors in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{AiModelDescriptor}"/> of matching descriptors.</returns>
        public IEnumerable<AiModelDescriptor> All() => this.models.Values;

        /// <summary>
        /// Returns all models that satisfy the provided predicate.
        /// </summary>
        /// <param name="predicate">Filter applied to descriptors.</param>
        /// <returns>An <see cref="IEnumerable{AiModelDescriptor}"/> of matching descriptors.</returns>
        public IEnumerable<AiModelDescriptor> Find(Func<AiModelDescriptor, bool> predicate)
        {
            // Guard against null predicate to prevent runtime errors.
            ArgumentNullException.ThrowIfNull(predicate);
            return this.models.Values.Where(predicate);
        }

        /// <summary>
        /// Attempts to get a descriptor for the specified model.
        /// </summary>
        /// <param name="model">The model key.</param>
        /// <param name="descriptor">The descriptor, if found.</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool TryGet(OpenAIModel model, out AiModelDescriptor? descriptor)
        {
            // Guard against invalid model keys to prevent runtime errors.
            return this.models.TryGetValue(model, out descriptor);
        }
    }
}
