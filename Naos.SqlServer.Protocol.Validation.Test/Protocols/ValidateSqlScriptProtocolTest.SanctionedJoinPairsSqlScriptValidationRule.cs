// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SanctionedJoinPairsSqlScriptValidationRule.cs" company="Naos Project">
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
        // Two sanctioned pairs — the FK edges from a parent table to two child tables.
        // Joining the two children directly on the FK column is NOT sanctioned.
        private static readonly IReadOnlyCollection<SanctionedJoinPair> SanctionedJoinPairsSqlScriptValidationRuleConfig = new[]
        {
            new SanctionedJoinPair(
                new SchemaQualifiedColumnName("dbo", "metric_absolute", "metric_absolute_id"),
                new SchemaQualifiedColumnName("dbo", "value_absolute_calendar_quarter", "metric_absolute_id")),
            new SanctionedJoinPair(
                new SchemaQualifiedColumnName("dbo", "metric_absolute", "metric_absolute_id"),
                new SchemaQualifiedColumnName("dbo", "value_absolute_fiscal_year", "metric_absolute_id")),
        };

        private static readonly IReadOnlyList<TestScenariosWithExpected> SanctionedJoinPairsSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // Two children joined directly on the FK — neither side is the parent, but both
            // sides are constrained columns (they appear in some sanctioned pair).
            // Violation at the ON BCE.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.value_absolute_calendar_quarter v Inner Join dbo.value_absolute_fiscal_year f On v.metric_absolute_id = f.metric_absolute_id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 99, Details = "join pair (dbo.value_absolute_calendar_quarter.metric_absolute_id <-> dbo.value_absolute_fiscal_year.metric_absolute_id) is not sanctioned" },
                },
            },

            // Same shape, tables in reversed FROM order.  Pair is canonicalized so the
            // order-agnostic check still rejects it.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.value_absolute_fiscal_year f Inner Join dbo.value_absolute_calendar_quarter v On f.metric_absolute_id = v.metric_absolute_id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 99, Details = "join pair (dbo.value_absolute_fiscal_year.metric_absolute_id <-> dbo.value_absolute_calendar_quarter.metric_absolute_id) is not sanctioned" },
                },
            },

            // Parent joined to child via a DIFFERENT column on the child — left side is
            // constrained (metric_absolute.metric_absolute_id), right side is not (some_other_id
            // never appears in any sanctioned pair), pair is not sanctioned → violation.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On m.metric_absolute_id = v.some_other_id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 88, Details = "join pair (dbo.metric_absolute.metric_absolute_id <-> dbo.value_absolute_calendar_quarter.some_other_id) is not sanctioned" },
                },
            },

            // 3-table chain — first join is sanctioned, second join (between the two children)
            // is not.  Violation only on the second join.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On m.metric_absolute_id = v.metric_absolute_id Inner Join dbo.value_absolute_fiscal_year f On v.metric_absolute_id = f.metric_absolute_id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 179, Details = "join pair (dbo.value_absolute_calendar_quarter.metric_absolute_id <-> dbo.value_absolute_fiscal_year.metric_absolute_id) is not sanctioned" },
                },
            },
        };

        private static readonly IReadOnlyList<TestScenariosWithExpected> SanctionedJoinPairsSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // Sanctioned pair: parent ⨝ first child.
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On m.metric_absolute_id = v.metric_absolute_id" },

            // Sanctioned pair: parent ⨝ second child.
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_fiscal_year f On m.metric_absolute_id = f.metric_absolute_id" },

            // Same sanctioned pair, FROM tables reversed; pair matching is order-agnostic.
            new TestScenariosWithExpected { Sql = "Select * From dbo.value_absolute_calendar_quarter v Inner Join dbo.metric_absolute m On v.metric_absolute_id = m.metric_absolute_id" },

            // Same sanctioned pair, comparison operands reversed within the ON.
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On v.metric_absolute_id = m.metric_absolute_id" },

            // 3-table chain — both joins sanctioned (parent ⨝ each child).
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On m.metric_absolute_id = v.metric_absolute_id Inner Join dbo.value_absolute_fiscal_year f On m.metric_absolute_id = f.metric_absolute_id" },

            // No join at all — single-table query.
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute Where metric_absolute_id = 'x'" },

            // Join on columns NOT in any sanctioned pair — uncovered, skipped silently.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id" },

            // ON predicate with a literal on one side — not a column-on-column pair, so the
            // rule has nothing to match; skipped.  (DisallowedJoinShapesSqlScriptValidationRule
            // with LiteralInOn can flag this from a different angle.)
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On m.metric_absolute_id = 'literal'" },

            // ON predicate with a function on one side — not a column-on-column pair, so
            // skipped.  (DisallowedJoinShapes with FunctionInOn can flag this.)
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute m Inner Join dbo.value_absolute_calendar_quarter v On Lower(m.metric_absolute_id) = v.metric_absolute_id" },

            // Fully-qualified column references on both sides resolve to the same identities
            // as alias-qualified, so the pair still matches.
            new TestScenariosWithExpected { Sql = "Select * From dbo.metric_absolute Inner Join dbo.value_absolute_calendar_quarter On dbo.metric_absolute.metric_absolute_id = dbo.value_absolute_calendar_quarter.metric_absolute_id" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SanctionedJoinPairsSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SanctionedJoinPairsSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SanctionedJoinPairsSqlScriptValidationRule(
                SanctionedJoinPairsSqlScriptValidationRuleConfig);

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
        public static void Execute___Should_return_no_violations___When_SanctionedJoinPairsSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SanctionedJoinPairsSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SanctionedJoinPairsSqlScriptValidationRule(
                SanctionedJoinPairsSqlScriptValidationRuleConfig);

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
