// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.DisallowedJoinShapesSqlScriptValidationRule.cs" company="Naos Project">
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
        // All flags enabled — every recognized "weird" shape will be flagged.
        private const JoinShapeIssues DisallowedJoinShapesSqlScriptValidationRuleAllFlags =
            JoinShapeIssues.SelfJoin
            | JoinShapeIssues.ConstantOn
            | JoinShapeIssues.CrossJoin
            | JoinShapeIssues.WhereBasedJoin
            | JoinShapeIssues.LiteralInOn
            | JoinShapeIssues.NonEqualityOn
            | JoinShapeIssues.FunctionInOn
            | JoinShapeIssues.ImplicitCrossJoin;

        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowedJoinShapesSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // SelfJoin — same physical table referenced twice in FROM (via aliases u1 and u2).
            // Violation at the SECOND NamedTableReference's StartOffset.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u1 Inner Join dbo.users u2 On u1.id = u2.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 38, Details = "self-join: table dbo.users is referenced multiple times in the FROM clause" },
                },
            },

            // ConstantOn — ON 1 = 1 has no column references.  Per-BCE checks are skipped
            // when ConstantOn fires (otherwise the same predicate would also be flagged as
            // an equality of literals or similar).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On 1 = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "ON clause has no column references (constant condition)" },
                },
            },

            // CrossJoin — explicit CROSS JOIN keyword.  Violation at the SecondTableReference
            // (the table being attached via the cross join), not at the UnqualifiedJoin's
            // StartOffset which would point at the LEFT table.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Cross Join dbo.b",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "CROSS JOIN not allowed" },
                },
            },

            // WhereBasedJoin — comma-FROM with a column-on-column equality in WHERE that
            // plays the role of a join condition.  Violation at the WHERE BCE.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a, dbo.b Where a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 33, Details = "old-style WHERE-based join not allowed; use JOIN ... ON" },
                },
            },

            // ImplicitCrossJoin — comma-FROM with no condition linking the tables.
            // Violation at the second top-level TableReference.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a, dbo.b",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 21, Details = "implicit cross join (comma-separated tables in FROM with no condition tying them together) not allowed" },
                },
            },

            // LiteralInOn — ON has a literal on one side of the equality comparison.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On a.x = 'literal'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "literal value in ON clause comparison; ON predicates should reference columns on both sides" },
                },
            },

            // NonEqualityOn — BCE with a non-equality operator on column-on-each-side.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On a.x > b.y",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "non-equality operator in ON; ON predicates should use equality (=)" },
                },
            },

            // NonEqualityOn — LIKE predicate in ON (caught even though it's not a BCE).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On a.x Like b.y",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "non-equality predicate (LIKE) in ON; ON predicates should use equality (=)" },
                },
            },

            // FunctionInOn — function call on one side of the comparison.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join dbo.b On Lower(a.x) = b.y",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "function call in ON clause; use bare column references on both sides" },
                },
            },
        };

        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowedJoinShapesSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // Simple SELECT with no joins at all.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a" },

            // Single-table SELECT with WHERE filter on a single column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Where a.x = 1" },

            // Canonical INNER JOIN ON column-on-column equality.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id" },

            // Multi-clause INNER JOIN ON with AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id And a.x = b.x" },

            // 3-table chain of canonical INNER JOIN ON.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id Inner Join dbo.c On b.id = c.id" },

            // LEFT OUTER JOIN with column-on-column equality.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Left Outer Join dbo.b On a.id = b.id" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_DisallowedJoinShapesSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowedJoinShapesSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowedJoinShapesSqlScriptValidationRule(
                DisallowedJoinShapesSqlScriptValidationRuleAllFlags);

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
        public static void Execute___Should_return_no_violations___When_DisallowedJoinShapesSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowedJoinShapesSqlScriptValidationRuleNoViolationScenarios;

            var rule = new DisallowedJoinShapesSqlScriptValidationRule(
                DisallowedJoinShapesSqlScriptValidationRuleAllFlags);

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
