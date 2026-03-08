// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// New model registry that is constructed from a pre-built descriptor dictionary.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OpenAIModels"/> class.
    /// </remarks>
    /// <param name="models">
    /// The descriptor dictionary keyed by <see cref="string"/>.
    /// This must contain one entry per supported model.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="models"/> is null.</exception>
    public sealed class OpenAIModels(IReadOnlyDictionary<string, AiModelDescriptor> models) : IAiModelRegistry
    {
        private readonly IReadOnlyDictionary<string, AiModelDescriptor> models = models ?? throw new ArgumentNullException(nameof(models));

        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        /// <returns see cref="IReadOnlyDictionary(string, AiModelDescriptor)">.</returns>
        public IReadOnlyDictionary<string, AiModelDescriptor> GetRegistry() => this.models;

        /// <summary>
        /// Returns all model descriptors in the registry.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{AiModelPropertyRegistryModel}"/> of matching descriptors.</returns>
        public IReadOnlyCollection<AiModelDescriptor> GetAll() => [.. this.models.Values];

        /// <summary>
        /// Gets the model descriptor for a specified model by its unique name, or null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        public AiModelDescriptor? TryGetByName(string name) => this.models.Values.Where(m => m.Name == name).FirstOrDefault();

        /// <summary>
        /// Gets the model descriptor for a specified model ..
        /// </summary>
        /// <param name="model">The model key.</param>
        /// <returns><see cref="AiModelDescriptor"/>.</returns>
        public AiModelDescriptor Get(string model) => this.models[model];

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
        public bool TryGet(string model, out AiModelDescriptor? descriptor)
        {
            // Guard against invalid model keys to prevent runtime errors.
            return this.models.TryGetValue(model, out descriptor);
        }
    }
}
