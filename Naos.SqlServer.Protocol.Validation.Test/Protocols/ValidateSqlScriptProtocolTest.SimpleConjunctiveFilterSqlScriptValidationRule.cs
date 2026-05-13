// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SimpleConjunctiveFilterSqlScriptValidationRule.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.SqlServer.Domain;
    using Xunit;

    public static partial class ValidateSqlScriptProtocolTest
    {
        // Scripts whose WHERE / HAVING / ON filter contains a top-level OR or an explicit
        // NOT wrapper.  Offsets point at the OR-binary-expression or NOT-wrapper node.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SimpleConjunctiveFilterSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // OR in WHERE — the BooleanBinaryExpression-Or node starts at the leftmost operand.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where a = 1 Or b = 2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 26, Details = "OR not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // NOT-wrapper in WHERE — BooleanNotExpression starts at the "NOT" keyword.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where Not (a = 1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 26, Details = "NOT not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // Chained ORs — the outermost BBE-Or fires once (recursion short-circuits on
            // first OR encountered).  The outer BBE-Or starts at the leftmost predicate.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where a = 1 Or b = 2 Or c = 3",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 26, Details = "OR not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // OR nested inside an AND — recursion walks into the AND's branches and fires
            // on the OR.  Offset points at the OR's leftmost operand (inside the parens).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where a = 1 And (b = 2 Or c = 3)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 37, Details = "OR not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // OR in HAVING.
            new TestScenariosWithExpected
            {
                Sql = "Select a, Count(*) From dbo.t Group By a Having Count(*) = 1 Or Min(a) = 0",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 48, Details = "OR not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // OR in JOIN ON.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id Or a.x = b.x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "OR not allowed in filter; filter must be a simple conjunction" },
                },
            },

            // NOT in JOIN ON.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On Not (a.id = b.id)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "NOT not allowed in filter; filter must be a simple conjunction" },
                },
            },
        };

        // Scripts whose filters are pure conjunctions (or trivial / empty) — the rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SimpleConjunctiveFilterSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No filter at all.
            new TestScenariosWithExpected { Sql = "Select 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },

            // Single comparison.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a = 1" },

            // AND-only conjunctions of varying depth.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a = 1 And b = 2" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a = 1 And b > 0 And c <= 10" },

            // Parenthesized ANDs.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where (a = 1) And (b = 2)" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where ((a = 1 And b = 2) And c = 3)" },

            // Inline-encoded "not" forms are leaf predicates, NOT BooleanNotExpression
            // wrappers — they pass.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a <> 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a != 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Is Null" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Is Not Null" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Not Like 'x%'" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Not In (1, 2, 3)" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Not Between 1 And 10" },

            // AND of inline-not predicates.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where a Is Not Null And b <> 0 And c Not In (1, 2)" },

            // HAVING with AND only.
            new TestScenariosWithExpected { Sql = "Select a, Count(*) From dbo.t Group By a Having Count(*) > 1 And Min(a) = 0" },

            // JOIN ON with AND only.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id And a.x = b.x" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SimpleConjunctiveFilterSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SimpleConjunctiveFilterSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SimpleConjunctiveFilterSqlScriptValidationRule();

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustBeEqualTo(testScenariosWithExpected);
        }

        [Fact]
        public static void Execute___Should_return_no_violations___When_SimpleConjunctiveFilterSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SimpleConjunctiveFilterSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SimpleConjunctiveFilterSqlScriptValidationRule();

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustNotHaveAnyViolations();
        }
    }
}
