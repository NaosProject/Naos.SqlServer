// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.BenchmarkingFilterValuesByColumnSqlScriptValidationRule.cs" company="Naos Project">
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
        // Rule config used across the tests below.  Owned values: O1, O2.  Anything else
        // (P1, P2, P3, P4, ...) is a "peer."  Minimum distinct peer values defaults to 3.
        private static readonly SchemaQualifiedColumnName BenchmarkingFilterValuesByColumnSqlScriptValidationRuleColumn =
            new SchemaQualifiedColumnName("dbo", "users", "entity_id");

        private static readonly IReadOnlyCollection<string> BenchmarkingFilterValuesByColumnSqlScriptValidationRuleOwnedValues =
            new[] { "O1", "O2" };

        // Scripts that violate the rule.  Offsets follow the predicate-start convention used
        // throughout the protocol tests: BCE/Like/In/IsNull/Between predicates start at the
        // column reference (offset 30 in "Select * From dbo.users Where ...").  The
        // "filter required" violation is emitted at the QuerySpecification's StartOffset
        // (offset 0).
        private static readonly IReadOnlyList<TestScenariosWithExpected> BenchmarkingFilterValuesByColumnSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // ===== Unsupported operator =====

            // LIKE — not in the allow-list.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Like 'a%'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter operator Like on constrained column dbo.users.entity_id is not supported for benchmarking; use =, IN, <>, or NOT IN" },
                },
            },

            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id > 'a'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter operator GreaterThan on constrained column dbo.users.entity_id is not supported for benchmarking; use =, IN, <>, or NOT IN" },
                },
            },

            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Between 'a' And 'z'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter operator Between on constrained column dbo.users.entity_id is not supported for benchmarking; use =, IN, <>, or NOT IN" },
                },
            },

            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Is Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter operator IsNull on constrained column dbo.users.entity_id is not supported for benchmarking; use =, IN, <>, or NOT IN" },
                },
            },

            // ===== Non-literal value =====

            // Parameter — caller can't enumerate the value statically.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = @param",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter value on constrained column dbo.users.entity_id must be a literal constant; found parameter" },
                },
            },

            // NULL — rejected even though NullLiteral is technically a Literal in the AST.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter value on constrained column dbo.users.entity_id must be a literal constant; found NULL" },
                },
            },

            // ===== Include shape (= / IN) violations =====

            // Single peer value with `=` — too few distinct peers for case (b).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'P1'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter on constrained column dbo.users.entity_id contains 1 distinct peer value(s); minimum is 3" },
                },
            },

            // Two distinct peers with IN — still below the (b) threshold.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id In ('P1', 'P2')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter on constrained column dbo.users.entity_id contains 2 distinct peer value(s); minimum is 3" },
                },
            },

            // Three values BUT one is a duplicate — only 2 distinct peers.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id In ('P1', 'P1', 'P2')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter on constrained column dbo.users.entity_id contains 2 distinct peer value(s); minimum is 3" },
                },
            },

            // Mixed owned + peer values — neither (a) nor (b) holds.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id In ('O1', 'P1')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter on constrained column dbo.users.entity_id mixes owned and peer values; must be either all owned (your data) OR all peer with at least 3 distinct values" },
                },
            },

            // Mixed with 3+ peer values — still mixed, still fails.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id In ('O1', 'P1', 'P2', 'P3')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "filter on constrained column dbo.users.entity_id mixes owned and peer values; must be either all owned (your data) OR all peer with at least 3 distinct values" },
                },
            },

            // ===== Exclude shape (<> / NOT IN) violations =====

            // <> with a single peer value — exclude shape requires owned values only.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id <> 'P1'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "exclusion filter (<> / NOT IN) on constrained column dbo.users.entity_id must list only owned values; found peer value(s)" },
                },
            },

            // NOT IN with all peers (even 3+ distinct) — there is no NOT-IN equivalent of (b).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Not In ('P1', 'P2', 'P3')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "exclusion filter (<> / NOT IN) on constrained column dbo.users.entity_id must list only owned values; found peer value(s)" },
                },
            },

            // NOT IN with mixed owned + peer — fails.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Not In ('O1', 'P1')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "exclusion filter (<> / NOT IN) on constrained column dbo.users.entity_id must list only owned values; found peer value(s)" },
                },
            },

            // ===== Required-filter violations =====

            // Constrained table in scope; no filter at all.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "filter on constrained column dbo.users.entity_id is required" },
                },
            },

            // Constrained table in scope; filter present but on a different column.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where name = 'x'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "filter on constrained column dbo.users.entity_id is required" },
                },
            },

            // ===== Multi-filter on constrained column =====

            // Two filters on entity_id — the rule punts rather than analyzing intersection.
            // Violation emitted at each EXTRA filter (the second IN at offset 51).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'O1' And entity_id In ('P1', 'P2', 'P3')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 51, Details = "multiple filters on constrained column dbo.users.entity_id are not allowed; only one filter is permitted" },
                },
            },

            // ===== Bare-reference qualification =====

            // Multi-table query; bare "entity_id" can't be resolved without schema introspection.
            // The "must be qualified" violation fires; the "filter required" violation is
            // suppressed (same root cause).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where entity_id = 'O1'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 76, Details = "column reference must be table-qualified in multi-table queries: entity_id" },
                },
            },
        };

        // Scripts that pass.
        private static readonly IReadOnlyList<TestScenariosWithExpected> BenchmarkingFilterValuesByColumnSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // (a) Own data — single owned value with `=`.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'O1'" },

            // (a) Own data — both owned values via IN.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('O1', 'O2')" },

            // (a) Single-value IN is equivalent to `=`.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('O1')" },

            // (b) Benchmark cohort — exactly the minimum distinct peers.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('P1', 'P2', 'P3')" },

            // (b) More than the minimum distinct peers.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('P1', 'P2', 'P3', 'P4', 'P5')" },

            // (b) Duplicates don't count — as long as DISTINCT count meets the threshold.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('P1', 'P2', 'P3', 'P1')" },

            // (c) Peer exploration — single owned with `<>`.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id <> 'O1'" },

            // (c) Peer exploration — NOT IN listing all owned values.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id Not In ('O1', 'O2')" },

            // (c) Peer exploration — NOT IN listing just one owned value (subset of owned).
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id Not In ('O1')" },

            // Alias-qualified case (a).
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Where u.entity_id = 'O1'" },

            // Fully-qualified case (a).
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where dbo.users.entity_id = 'O1'" },

            // Multi-table case (a) — entity_id properly alias-qualified.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where u.entity_id = 'O1'" },

            // Multi-table case (b) — alias-qualified peer cohort.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where u.entity_id In ('P1', 'P2', 'P3')" },

            // Multi-table case (c) — alias-qualified peer exploration.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where u.entity_id <> 'O1'" },

            // Constrained table NOT in scope — rule skips the query entirely.  No filter
            // required when the table simply isn't being queried.
            new TestScenariosWithExpected { Sql = "Select * From dbo.orders Where name = 'x'" },

            // Reversed comparison — `'O1' = entity_id` is the same as `entity_id = 'O1'`
            // from the column's perspective.  Passes as case (a).
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where 'O1' = entity_id" },
        };

        // Scripts under a custom config where requireFilterOnConstrainedColumn=false.
        // The "filter required" violation no longer fires when the table is in scope but
        // no filter is present.  All other rule logic is unchanged.
        private static readonly IReadOnlyList<TestScenariosWithExpected> BenchmarkingFilterValuesByColumnSqlScriptValidationRuleRequireFilterFalseScenarios = new[]
        {
            // Constrained table in scope; no filter — passes because requireFilter is off.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users" },

            // Constrained table in scope; filter on a different column — passes.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where name = 'x'" },

            // The other shapes still validate normally.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'O1'" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('P1', 'P2', 'P3')" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id <> 'O1'" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_BenchmarkingFilterValuesByColumnSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = BenchmarkingFilterValuesByColumnSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleColumn,
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleOwnedValues);

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
        public static void Execute___Should_return_no_violations___When_BenchmarkingFilterValuesByColumnSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = BenchmarkingFilterValuesByColumnSqlScriptValidationRuleNoViolationScenarios;

            var rule = new BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleColumn,
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleOwnedValues);

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
        public static void Execute___Should_return_no_violations___When_BenchmarkingFilterValuesByColumnSqlScriptValidationRule_requireFilterOnConstrainedColumn_is_false()
        {
            // Arrange
            var testScenariosWithExpected = BenchmarkingFilterValuesByColumnSqlScriptValidationRuleRequireFilterFalseScenarios;

            var rule = new BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleColumn,
                BenchmarkingFilterValuesByColumnSqlScriptValidationRuleOwnedValues,
                requireFilterOnConstrainedColumn: false);

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
