// <copyright file="IntPropertyAggregationExtensions.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Indicates which visibility of properties should be included in reflection-based operations.
    /// </summary>
    public enum PropertyVisibility
    {
        /// <summary>
        /// Only public instance properties are included.
        /// </summary>
        PublicOnly,

        /// <summary>
        /// Only non-public instance properties (for example, private, protected) are included.
        /// </summary>
        NonPublicOnly,

        /// <summary>
        /// Both public and non-public instance properties are included.
        /// </summary>
        PublicAndNonPublic,
    }

    /// <summary>
    /// Provides helper methods for aggregating values from properties on an object instance.
    /// </summary>
    public static class IntPropertyAggregationExtensions
    {
        /// <summary>
        /// Gets all instance properties on the specified type whose property type matches <typeparamref name="TProperty"/>,
        /// filtered by the specified <see cref="PropertyVisibility"/>.
        /// </summary>
        /// <typeparam name="TProperty">The property type to match.</typeparam>
        /// <param name="type">The type to inspect.</param>
        /// <param name="visibility">
        /// The visibility filter that determines which properties are included in the result.
        /// </param>
        /// <returns>
        /// A read-only collection of <see cref="PropertyInfo"/> instances representing the matching properties.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        public static IReadOnlyCollection<PropertyInfo> GetInstancePropertiesOfType<TProperty>(Type type, PropertyVisibility visibility)
        {
            ArgumentNullException.ThrowIfNull(type);

            Type propertyType = typeof(TProperty);

            BindingFlags flags = BindingFlags.Instance;

            switch (visibility)
            {
                case PropertyVisibility.PublicOnly:
                    {
                        flags |= BindingFlags.Public;
                        break;
                    }

                case PropertyVisibility.NonPublicOnly:
                    {
                        flags |= BindingFlags.NonPublic;
                        break;
                    }

                case PropertyVisibility.PublicAndNonPublic:
                    {
                        flags |= BindingFlags.Public | BindingFlags.NonPublic;
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(visibility), visibility, "Unsupported property visibility value.");
                    }
            }

            PropertyInfo[] properties = [.. type
                .GetProperties(flags)
                .Where(property => property.PropertyType == propertyType)];

            return properties;
        }

        /// <summary>
        /// Sums all <see cref="int"/> properties on the specified instance and assigns the result to a target property.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance whose properties will be aggregated.</param>
        /// <param name="targetPropertyName">
        /// The name of the <see cref="int"/> property that should receive the aggregated value
        /// (for example, Total).
        /// </param>
        /// <param name="excludeTargetFromSum">
        /// If <see langword="true"/>, the target property will be excluded from the sum calculation.
        /// This is typically desirable when the target is itself an <see cref="int"/> property such as Total.
        /// </param>
        /// <param name="visibility">
        /// The visibility filter that determines which <see cref="int"/> properties are included in the sum.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="targetPropertyName"/> is <see langword="null"/>,
        /// empty, or consists only of white-space characters.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified target property cannot be found, is not an <see cref="int"/> property,
        /// or is not writable.
        /// </exception>
        public static void SumIntPropertiesInto<T>(this T instance, string targetPropertyName, bool excludeTargetFromSum = true, PropertyVisibility visibility = PropertyVisibility.PublicAndNonPublic)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(instance);

            if (string.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new ArgumentException("Target property name must be provided.", nameof(targetPropertyName));
            }

            Type type = instance.GetType();

            BindingFlags targetFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            PropertyInfo? targetProperty = type.GetProperty(name: targetPropertyName, bindingAttr: targetFlags) ??
                throw new InvalidOperationException($"Type '{type.FullName}' does not define a property named '{targetPropertyName}'.");

            if (targetProperty.PropertyType != typeof(int) || !targetProperty.CanWrite)
            {
                throw new InvalidOperationException($"Property '{targetPropertyName}' on type '{type.FullName}' must be a writable int property.");
            }

            // Get all instance properties of type int based on the specified visibility filter ..
            IReadOnlyCollection<PropertyInfo> intProperties = GetInstancePropertiesOfType<int>(type: type, visibility: visibility);

            int sum = 0;

            // .. then sum their values, optionally excluding the target property itself from the sum if it is also an int property (for example, Total) ..
            foreach (PropertyInfo property in intProperties)
            {
                if (excludeTargetFromSum && string.Equals(property.Name, targetPropertyName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!property.CanRead)
                {
                    continue;
                }

                object? value = property.GetValue(instance);

                if (value is int intValue)
                {
                    sum += intValue;
                }
            }

            targetProperty.SetValue(instance, sum);
        }
    }
}