// <copyright file="HtmlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.HtmlValidator;

    [TestClass]
    public class HtmlValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("<p>Hello</p>")]
        public void ValidHtml_ShouldPass(string html)
        {
            bool result = this.validator.IsValidFormat(html, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("<p>Missing closing tag")]
        public void InvalidHtml_ShouldFail(string html)
        {
            bool result = this.validator.IsValidFormat(html, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
