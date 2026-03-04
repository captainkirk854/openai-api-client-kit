// <copyright file="EmbeddedAiModelCapabilityRegistrySource.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Helpers
{
    using System.Reflection;
    using System.Text.Json;
    using OpenAIApiClient.Models.Registries.AiModels;
    using testClass = OpenAIApiClient.Helpers.EmbeddedAiModelCapabilityRegistrySource;

    [TestClass]
    public class EmbeddedAiModelCapabilityRegistrySource
    {
        [TestMethod]
        public void GetRegistryStreams_Reads_Capability_Resources()
        {
            // Arrange
            Assembly testAssembly = typeof(testClass).Assembly;
            string resourcePrefix = "OpenAIApiClient._internal.OpenAiModels.capabilities.";
            string resourceSuffix = ".json";

            // This prefix should capture only capability JSON in your test harness
            testClass source = new(assembly: testAssembly, resourcePrefix: resourcePrefix, resourceSuffix: resourceSuffix);

            // Act
            List<string> resourceNames = [.. testAssembly.GetManifestResourceNames()];
            List<Stream> streams = [.. source.GetRegistryStreams()];

            // Calculate the expected count of resources that match the prefix and suffix criteria ..
            int expectedCount = resourceNames.Count(n =>
                n.StartsWith(resourcePrefix, StringComparison.OrdinalIgnoreCase) &&
                n.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));

            // Assert: only resources under the prefix and ending in .json are returned
            Assert.HasCount(expectedCount, streams, "Unexpected number of capability registry streams.");
        }

        [TestMethod]
        public void GetRegistryStreams_Reads_Capability_Resources_With_Defaults()
        {
            // Arrange
            Assembly testAssembly = typeof(testClass).Assembly;
            string resourcePrefix = "OpenAIApiClient._internal.OpenAiModels.capabilities.";
            string resourceSuffix = ".json";

            // This prefix should capture only capability JSON in your test harness
            testClass source = new();

            // Act
            List<string> resourceNames = [.. testAssembly.GetManifestResourceNames()];
            List<Stream> streams = [.. source.GetRegistryStreams()];

            // Calculate the expected count of resources that match the prefix and suffix criteria ..
            int expectedCount = resourceNames.Count(n =>
                n.StartsWith(resourcePrefix, StringComparison.OrdinalIgnoreCase) &&
                n.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));

            // Assert: only resources under the prefix and ending in .json are returned
            Assert.HasCount(expectedCount, streams, "Unexpected number of capability registry streams.");
        }

        [TestMethod]
        public void GetRegistryStreams_Returns_AtLeast_One_Stream_When_Embedded_Resources_Exist()
        {
            // Arrange
            testClass source = new();

            // Act
            List<Stream> streams = [.. source.GetRegistryStreams()];

            // Assert
            Assert.IsNotNull(streams);
            Assert.IsNotEmpty(streams, "Expected at least one embedded capability registry stream.");

            foreach (Stream stream in streams)
            {
                Assert.IsNotNull(stream);
                Assert.IsTrue(stream.CanRead, "Stream must be readable.");
            }
        }

        [TestMethod]
        public void GetRegistryStreams_All_Streams_Contain_Valid_Registry_Json()
        {
            // Initialise ..
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true,
            };

            // Arrange
            testClass source = new();

            // Act
            List<Stream> streams = [.. source.GetRegistryStreams()];

            // Assert pre‑conditions
            Assert.IsNotEmpty(streams, "No capability streams were returned.");

            // Validate that each stream contains valid JSON that can be deserialized into the expected registry structure.
            foreach (Stream stream in streams)
            {
                using (stream)
                using (StreamReader reader = new(stream, leaveOpen: false))
                {
                    string json = reader.ReadToEnd();

                    // Assert that the JSON is not null or whitespace, which would indicate an issue with the embedded resource content.
                    Assert.IsFalse(string.IsNullOrWhiteSpace(json), "Embedded capability JSON must not be empty.");

                    // Try to deserialize using internal DTO ... if this fails, the JSON structure is not as expected for the registry data.
                    AiModelCapabilityRegistryData? registry = JsonSerializer.Deserialize<AiModelCapabilityRegistryData>(json: json, options: options);

                    // Assert that deserialization succeeded and that the expected properties are present in the resulting object.
                    Assert.IsNotNull(registry, "Capability registry deserialization returned null.");
                    Assert.IsNotNull(registry.Models, "Capability registry must contain 'models' collection.");
                    Assert.IsTrue(registry.Models.Any(), "Capability registry 'models' collection must not be empty.");
                }
            }
        }
    }
}
