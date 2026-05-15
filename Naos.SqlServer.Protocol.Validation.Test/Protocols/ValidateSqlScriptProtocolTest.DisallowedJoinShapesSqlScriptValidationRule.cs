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
        // The 8 "non-vanilla" shapes — every join-related issue OTHER than the plain
        // join types (INNER / LEFT OUTER / RIGHT OUTER / FULL OUTER / CROSS APPLY /
        // OUTER APPLY).  Used by the legacy-style violation/no-violation tests below,
        // which include vanilla INNER and LEFT OUTER joins among the no-violation
        // scenarios.
        private const JoinShapes DisallowedJoinShapesSqlScriptValidationRuleNonVanillaShapes =
            JoinShapes.SelfJoin
            | JoinShapes.ConstantOn
            | JoinShapes.CrossJoin
            | JoinShapes.WhereBasedJoin
            | JoinShapes.LiteralInOn
            | JoinShapes.NonEqualityOn
            | JoinShapes.FunctionInOn
            | JoinShapes.ImplicitCrossJoin;

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

        // Scenarios for the join-type flags (InnerJoin / LeftOuterJoin / RightOuterJoin /
        // FullOuterJoin / CrossApply / OuterApply).  Each scenario configures only the
        // single flag it tests so the violation can be tied to that flag specifically;
        // CrossJoin already has coverage in the non-vanilla set above.
        private static readonly IReadOnlyList<JoinTypeScenario> DisallowedJoinShapesSqlScriptValidationRuleJoinTypeScenarios = new[]
        {
            new JoinTypeScenario
            {
                Flag = JoinShapes.InnerJoin,
                Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id",
                Expected = new ExpectedViolation { Offset = 31, Details = "INNER JOIN not allowed" },
            },
            new JoinTypeScenario
            {
                Flag = JoinShapes.LeftOuterJoin,
                Sql = "Select * From dbo.a Left Outer Join dbo.b On a.id = b.id",
                Expected = new ExpectedViolation { Offset = 36, Details = "LEFT OUTER JOIN not allowed" },
            },
            new JoinTypeScenario
            {
                Flag = JoinShapes.RightOuterJoin,
                Sql = "Select * From dbo.a Right Outer Join dbo.b On a.id = b.id",
                Expected = new ExpectedViolation { Offset = 37, Details = "RIGHT OUTER JOIN not allowed" },
            },
            new JoinTypeScenario
            {
                Flag = JoinShapes.FullOuterJoin,
                Sql = "Select * From dbo.a Full Outer Join dbo.b On a.id = b.id",
                Expected = new ExpectedViolation { Offset = 36, Details = "FULL OUTER JOIN not allowed" },
            },
            new JoinTypeScenario
            {
                Flag = JoinShapes.CrossApply,
                Sql = "Select * From dbo.a Cross Apply (Select * From dbo.b) sub",
                Expected = new ExpectedViolation { Offset = 32, Details = "CROSS APPLY not allowed" },
            },
            new JoinTypeScenario
            {
                Flag = JoinShapes.OuterApply,
                Sql = "Select * From dbo.a Outer Apply (Select * From dbo.b) sub",
                Expected = new ExpectedViolation { Offset = 32, Details = "OUTER APPLY not allowed" },
            },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_DisallowedJoinShapesSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowedJoinShapesSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowedJoinShapesSqlScriptValidationRule(
                DisallowedJoinShapesSqlScriptValidationRuleNonVanillaShapes);

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
                DisallowedJoinShapesSqlScriptValidationRuleNonVanillaShapes);

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustNotHaveAnyViolations();
        }

        [Fact]
        public static void Execute___Should_return_a_violation_for_each_join_type___When_the_type_is_set_in_isolation()
        {
            // Arrange
            var scenarios = DisallowedJoinShapesSqlScriptValidationRuleJoinTypeScenarios;

            var testScenariosWithExpected = scenarios
                .Select(s => new TestScenariosWithExpected
                {
                    Sql = s.Sql,
                    ExpectedViolations = new[] { s.Expected },
                })
                .ToList();

            // Each scenario uses a rule configured with ONLY its flag — verifies that the
            // flag fires for its shape AND that the corresponding join shape doesn't
            // accidentally fire for a different flag.
            var rules = scenarios.Select(s => new DisallowedJoinShapesSqlScriptValidationRule(s.Flag)).ToList();

            var operations = scenarios
                .Select((s, i) => new ValidateSqlScriptOp(SqlServerVersion, s.Sql, new[] { rules[i] }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            // Act
            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            // Assert
            actual.MustBeEqualTo(testScenariosWithExpected);
        }

        [Fact]
        public static void Execute___Should_return_no_violations___When_DisallowedJoinShapesSqlScriptValidationRule_uses_All_and_query_has_no_joins()
        {
            // With JoinShapes.All, even vanilla joins are flagged.  Only queries with no
            // joins at all should pass.
            var testScenariosWithExpected = new[]
            {
                new TestScenariosWithExpected { Sql = "Select 1" },
                new TestScenariosWithExpected { Sql = "Select * From dbo.a" },
                new TestScenariosWithExpected { Sql = "Select * From dbo.a Where a.x = 1" },
            };

            var rule = new DisallowedJoinShapesSqlScriptValidationRule(JoinShapes.All);

            var operations = testScenariosWithExpected
                .Select(_ => new ValidateSqlScriptOp(SqlServerVersion, _.Sql, new[] { rule }))
                .ToList();

            var systemUnderTest = new ValidateSqlScriptProtocol();

            var actual = operations.Select(_ => systemUnderTest.Execute(_)).ToList();

            actual.MustNotHaveAnyViolations();
        }

        private class JoinTypeScenario
        {
            public JoinShapes Flag { get; set; }

            public string Sql { get; set; }

            public ExpectedViolation Expected { get; set; }
        }
    }
}
