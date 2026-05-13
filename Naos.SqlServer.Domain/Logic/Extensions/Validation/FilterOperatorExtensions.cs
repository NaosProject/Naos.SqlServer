// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOperatorExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Extension methods on <see cref="FilterOperator"/> and <see cref="FilterOperators"/>.
    /// </summary>
    public static class FilterOperatorExtensions
    {
        /// <summary>
        /// Converts a <see cref="FilterOperator"/> into a <see cref="FilterOperators"/>.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <returns>
        /// The <see cref="FilterOperators"/> converted from a <see cref="FilterOperator"/>.
        /// </returns>
        public static FilterOperators ToFilterOperators(
            this FilterOperator op)
        {
            new { op }.AsArg().Must().NotBeEqualTo(FilterOperator.Unknown);

            if (!Enum.TryParse<FilterOperators>(op.ToString(), ignoreCase: false, out var result))
            {
                throw new InvalidOperationException(Invariant($"This {nameof(FilterOperator)} does not have a corresponding flag in {nameof(FilterOperators)}: {op}."));
            }

            return result;
        }
    }
}
