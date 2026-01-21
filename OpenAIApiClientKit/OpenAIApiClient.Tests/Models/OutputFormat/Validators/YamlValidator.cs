// <copyright file="YamlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.YamlValidator;

    [TestClass]
    public class YamlValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("name: Klaus\nage: 42")]
        public void ValidYaml_ShouldPass(string yaml)
        {
            bool result = this.validator.IsValidFormat(yaml, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("name: Klaus: extra")]
        public void InvalidYaml_ShouldFail(string yaml)
        {
            bool result = this.validator.IsValidFormat(yaml, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
