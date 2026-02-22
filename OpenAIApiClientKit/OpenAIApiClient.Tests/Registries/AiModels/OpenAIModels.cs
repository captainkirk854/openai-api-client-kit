// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.AiModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using testClass = OpenAIApiClient.Registries.AiModels.OpenAIModels;

    /// <summary>
    /// Tests for the <see cref="OpenAIModels"/> class.
    /// </summary>
    [TestClass]
    public class OpenAIModels
    {
        // Access the registry from actual class under test ..
        private static Dictionary<OpenAIModel, AiModelDescriptor> Registry => new testClass().GetRegistry();

        [TestMethod]
        public void All_Descriptors_Have_Matching_Model_Key()
        {
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in Registry)
            {
                Assert.IsNotNull(descriptor, $"Descriptor for {model} is null");
                Assert.AreEqual(model, descriptor.Name, $"Descriptor.Model mismatch for {model}");
            }
        }

        [TestMethod]
        public void All_Descriptors_Have_NonNull_Capabilities()
        {
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in Registry)
            {
                Assert.IsNotNull(descriptor.Capabilities, $"Capabilities for {model} must not be null");
            }
        }

        [TestMethod]
        public void All_Descriptors_Have_NonNull_Pricing()
        {
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in Registry)
            {
                Assert.IsNotNull(descriptor.Pricing, $"Pricing for {model} must not be null");
            }
        }

        [TestMethod]
        public void All_Descriptors_Have_AtLeast_One_Capability()
        {
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in Registry)
            {
                Assert.IsNotEmpty(descriptor.Capabilities, $"Capabilities for {model} must not be empty");
            }
        }

        [TestMethod]
        public void Registry_Has_No_Duplicate_Keys()
        {
            List<OpenAIModel> keys = [.. Registry.Keys];
            List<IGrouping<OpenAIModel, OpenAIModel>> duplicates = [.. keys.GroupBy(k => k).Where(g => g.Count() > 1)];
            Assert.IsEmpty(duplicates, "Registry contains duplicate model keys");
        }

        [TestMethod]
        public void Registry_Is_Immutable()
        {
            Dictionary<OpenAIModel, AiModelDescriptor> dict = Registry;
            bool threw = false;

            try
            {
                dict.Add(OpenAIModel.GPT5_2, Registry[OpenAIModel.GPT5_2]);
            }
            catch (Exception)
            {
                threw = true;
            }

            Assert.IsTrue(threw, "Registry should be immutable and throw on mutation attempts");
        }

        [TestMethod]
        public void All_Enum_Values_Are_Represented_Or_Explicitly_Excluded()
        {
            List<OpenAIModel> enumValues = [.. Enum.GetValues(typeof(OpenAIModel)).Cast<OpenAIModel>()];
            List<OpenAIModel> registryValues = [.. Registry.Keys];

            // Full coverage ..
            Assert.IsTrue(enumValues.All(registryValues.Contains), "Registry missing one or more enum values");

            // If you intentionally exclude some models, enforce explicit documentation ..
            List<OpenAIModel> missing = [.. enumValues.Except(registryValues)];

            // Define a whitelist of intentionally excluded models:
            HashSet<OpenAIModel> allowedMissing =
            [
                 OpenAIModel.Whisper_1, // Deprecated - use Whisper1
            ];

            List<OpenAIModel> unexpectedMissing = [.. missing.Except(allowedMissing)];
            Assert.IsEmpty(unexpectedMissing, $"Registry missing unexpected models: {string.Join(", ", unexpectedMissing)}");
        }

        [TestMethod]
        public void Capabilities_Are_Valid_Enum_Values()
        {
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in Registry)
            {
                foreach (AiModelCapability cap in descriptor.Capabilities)
                {
                    Assert.IsTrue(Enum.IsDefined(typeof(AiModelCapability), cap), $"Invalid capability {cap} found in {model}");
                }
            }
        }
    }
}
