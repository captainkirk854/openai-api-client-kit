// <copyright file="RangeExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    /// <summary>
    /// Provides extension methods for range operations on comparable types.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Determines whether the specified value is within the inclusive range [min, max].
        /// </summary>
        /// <typeparam name="T">The comparable value type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The inclusive lower bound.</param>
        /// <param name="max">The inclusive upper bound.</param>
        /// <param name="isInclusive">Indicates whether the range is inclusive (default is <see langword="true"/>).</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value"/> is between
        /// <paramref name="min"/> and <paramref name="max"/> (inclusive); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsBetween<T>(this T value, T min, T max, bool isInclusive = true)
            where T : IComparable<T>
        {
            // Running in exclusive mode?
            if (!isInclusive)
            {
                // Validate that value is greater than min and less than max for exclusive range ..
                return value.CompareTo(min) > 0 && value.CompareTo(max) < 0;
            }

            // Validate that value is greater than or equal to min and less than or equal to max for inclusive range ..
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
    }
}