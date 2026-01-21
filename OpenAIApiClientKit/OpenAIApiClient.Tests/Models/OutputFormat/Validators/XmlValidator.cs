// <copyright file="XmlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.XmlValidator;

    [TestClass]
    public class XmlValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("<root><item>value</item></root>")]
        public void ValidXml_ShouldPass(string xml)
        {
            bool result = this.validator.IsValidFormat(content: xml, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("<root><item></root>")]
        public void InvalidXml_ShouldFail(string xml)
        {
            bool result = this.validator.IsValidFormat(content: xml, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
