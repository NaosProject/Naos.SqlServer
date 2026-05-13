// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOperatorExtensionsTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test.Logic
{
    using System;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using Xunit;

    public static class FilterOperatorExtensionsTest
    {
        [Fact]
        public static void ToFilterOperators___Should_throw_ArgumentException___When_parameter_op_is_Unknown()
        {
            // Arrange, Act
            var actual = Record.Exception(() => FilterOperator.Unknown.ToFilterOperators());

            // Assert
            actual.AsTest().Must().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public static void ToFilterOperators___Should_return_corresponding_FilterOperators___When_called()
        {
            // Arrange
            var operatorsAndExpected = new[]
            {
                new { Operator = FilterOperator.Equal, Expected = FilterOperators.Equal, },
                new { Operator = FilterOperator.NotEqual, Expected = FilterOperators.NotEqual, },
                new { Operator = FilterOperator.LessThan, Expected = FilterOperators.LessThan, },
                new { Operator = FilterOperator.GreaterThan, Expected = FilterOperators.GreaterThan, },
                new { Operator = FilterOperator.LessThanOrEqual, Expected = FilterOperators.LessThanOrEqual, },
                new { Operator = FilterOperator.GreaterThanOrEqual, Expected = FilterOperators.GreaterThanOrEqual, },
                new { Operator = FilterOperator.Like, Expected = FilterOperators.Like, },
                new { Operator = FilterOperator.NotLike, Expected = FilterOperators.NotLike, },
                new { Operator = FilterOperator.In, Expected = FilterOperators.In, },
                new { Operator = FilterOperator.NotIn, Expected = FilterOperators.NotIn, },
                new { Operator = FilterOperator.Between, Expected = FilterOperators.Between, },
                new { Operator = FilterOperator.NotBetween, Expected = FilterOperators.NotBetween, },
                new { Operator = FilterOperator.IsNull, Expected = FilterOperators.IsNull, },
                new { Operator = FilterOperator.IsNotNull, Expected = FilterOperators.IsNotNull, },
            };

            var expected = operatorsAndExpected.Select(_ => _.Expected).ToList();

            // Act
            var actual = operatorsAndExpected.Select(_ => _.Operator.ToFilterOperators()).ToList();

            // Assert
            actual.AsTest().Must().BeEqualTo(expected);
        }
    }
}
