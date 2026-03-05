// <copyright file="OpenAIModelsFactory.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.AiModels.Factories
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels;
    using testClass = OpenAIApiClient.Registries.AiModels.Factories.OpenAIModelsFactory;

    [TestClass]
    public sealed class OpenAIModelsFactory
    {
        [TestMethod]
        public void Create_Returns_Registry_With_Descriptors_For_All_Models()
        {
            // Act
            OpenAIModelsNEW registry = testClass.Create();

            // Assert
            Assert.IsNotNull(registry, "Factory must not return null.");

            // For each enum value, ensure a descriptor exists and has non-null capabilities
            foreach (OpenAIModel model in Enum.GetValues(typeof(OpenAIModel)))
            {
                AiModelDescriptor descriptor = registry.Get(model: model);

                Assert.IsNotNull(descriptor, $"Descriptor for {model} must not be null.");
                Assert.IsNotNull(descriptor.Capabilities, $"Capabilities for {model} must not be null.");
            }
        }

        [TestMethod]
        public void Create_Populates_At_Least_One_Model_With_Capabilities()
        {
            // Act
            OpenAIModelsNEW registry = testClass.Create();

            // Assert
            IEnumerable<AiModelDescriptor> allDescriptors = registry.Find(_ => true);

            Assert.IsTrue(allDescriptors.Any(), "Registry must contain at least one descriptor.");

            bool anyHasCapabilities = allDescriptors.Any(d => d.Capabilities.Any());

            Assert.IsTrue(
                anyHasCapabilities,
                "At least one descriptor should have non-empty Capabilities derived from the capability registry.");
        }

        [TestMethod]
        public void Create_Sets_Descriptor_Name_To_Matching_Model()
        {
            // Act
            OpenAIModelsNEW registry = testClass.Create();

            // Assert
            foreach (OpenAIModel model in Enum.GetValues(typeof(OpenAIModel)))
            {
                AiModelDescriptor descriptor = registry.Get(model: model);

                // Name should be set by the factory finalization loop
                Assert.AreEqual(
                    model,
                    descriptor.Name,
                    $"Descriptor.Name must match the model key for {model}.");
            }
        }
    }
}
