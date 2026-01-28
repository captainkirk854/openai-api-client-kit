// <copyright file="SqlValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OutputFormat.Validators
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Validators;

    public sealed class SqlValidator : IOutputFormatValidator
    {
        private static readonly string[] SqlKeywords =
        [
            "SELECT",
            "INSERT",
            "UPDATE",
            "DELETE",
            "CREATE",
            "DROP",
            "ALTER",
            "WHERE",
            "FROM",
            "JOIN",
            "GROUP",
            "ORDER",
            "VALUES",
            "INTO",
            "SET",
            "TABLE",
            "VIEW",
            "INDEX",
        ];

        public bool IsValidFormat(string content, out string? error)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                error = $"{OutputFormat.Sql} content cannot be empty.";
                return false;
            }

            string sql = content.Trim();

            // Basic forbidden characters
            if (sql.Contains('\0'))
            {
                error = $"{OutputFormat.Sql} contains invalid characters.";
                return false;
            }

            // Balanced parentheses
            if (!HasBalancedParentheses(sql))
            {
                error = $"{OutputFormat.Sql} has unbalanced parentheses.";
                return false;
            }

            // Must contain at least one SQL keyword
            if (!SqlKeywords.Any(k => sql.Contains(k, StringComparison.InvariantCultureIgnoreCase)))
            {
                error = $"{OutputFormat.Sql} does not contain any recognized keywords.";
                return false;
            }

            // Optional: require semicolon
            if (!sql.EndsWith(';'))
            {
                error = $"{OutputFormat.Sql} should end with a semicolon.";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Helper method to check for balanced parentheses in the SQL string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>bool.</returns>
        private static bool HasBalancedParentheses(string text)
        {
            int depth = 0;

            foreach (char chr in text)
            {
                if (chr == '(')
                {
                    depth++;
                }

                if (chr == ')')
                {
                    depth--;
                }

                if (depth < 0)
                {
                    return false;
                }
            }

            return depth == 0;
        }
    }
}