// <copyright file="SqlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.SqlValidator;

    [TestClass]
    public class SqlValidator
    {
        private readonly testClass validator = new();

        // ---------------------------
        // VALID SQL CASES
        // ---------------------------
        [TestMethod]
        [DataRow("SELECT * FROM Users;")]
        public void Valid_SelectStatement_ShouldPass(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("INSERT INTO Users (Name, Age) VALUES ('Klaus', 42);")]
        public void Valid_InsertStatement_ShouldPass(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("UPDATE Users SET Age = 43 WHERE Name = 'Klaus';")]
        public void Valid_UpdateStatement_ShouldPass(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        // ---------------------------
        // INVALID SQL CASES
        // ---------------------------
        [TestMethod]
        [DataRow("SELECT * FROM Users")]
        public void MissingSemicolon_ShouldFail(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        [DataRow("SELECT (Name FROM Users;")]
        public void UnbalancedParentheses_ShouldFail(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        [DataRow("This is not SQL;")]
        public void NoSqlKeywords_ShouldFail(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        [DataRow("   ")]
        public void NullOrWhitespace_ShouldFail(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }

        // ---------------------------
        // EDGE CASES
        // ---------------------------
        [TestMethod]
        [DataRow("select * from users;")]
        public void LowercaseKeywords_ShouldPass(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("SeLeCt * FrOm Users;")]
        public void MixedCaseKeywords_ShouldPass(string sql)
        {
            bool result = this.validator.IsValidFormat(sql, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }
    }
}
