// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.ConstrainedFilterOperatorsByColumnSqlScriptValidationRule.cs" company="Naos Project">
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
        // Rule config used by both tests: dbo.users.entity_id allows only =, IN.  Other
        // columns are unconstrained.
        private static readonly IReadOnlyCollection<ColumnFilterOperators> ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleConfig = new[]
        {
            new ColumnFilterOperators(
                new SchemaQualifiedColumnName("dbo", "users", "entity_id"),
                FilterOperators.Equal | FilterOperators.In),
        };

        // Scripts that use a disallowed operator on a configured column — the rule fires.
        // Offsets point at the predicate node (BooleanComparisonExpression / LikePredicate /
        // InPredicate / BooleanTernaryExpression / BooleanIsNullExpression) — typically at
        // the start of the column reference on the left-hand side of the predicate.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // LIKE on entity_id — not in the allow-list.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Like 'abc%'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator Like is not allowed on column dbo.users.entity_id" },
                },
            },

            // > on entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id > 'abc'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator GreaterThan is not allowed on column dbo.users.entity_id" },
                },
            },

            // BETWEEN on entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Between 'a' And 'z'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator Between is not allowed on column dbo.users.entity_id" },
                },
            },

            // IS NULL on entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Is Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator IsNull is not allowed on column dbo.users.entity_id" },
                },
            },

            // <> on entity_id — NotEqual is not in the allow-list (only Equal is).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id <> 'abc'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator NotEqual is not allowed on column dbo.users.entity_id" },
                },
            },

            // NOT IN on entity_id — NotIn is not in the allow-list.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id Not In ('a', 'b')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator NotIn is not allowed on column dbo.users.entity_id" },
                },
            },

            // Alias-qualified column reference — resolves to dbo.users.entity_id via the
            // alias map; fires the same violation.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Where u.entity_id Like 'abc%'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 32, Details = "operator Like is not allowed on column dbo.users.entity_id" },
                },
            },

            // Fully-qualified column reference.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where dbo.users.entity_id Like 'abc%'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator Like is not allowed on column dbo.users.entity_id" },
                },
            },

            // Bare column reference in a multi-table FROM — cannot be resolved without
            // schema introspection.  Bare name matches a configured column's name, so the
            // "must be qualified" violation fires (even though the operator IS in fact
            // allowed for entity_id — the rule errs on the side of caller security).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where entity_id Like 'abc%'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 76, Details = "column reference must be table-qualified in multi-table queries: entity_id" },
                },
            },

            // Reversed operand order — `'abc' > entity_id` is the same as `entity_id < 'abc'`
            // when viewed from the column's perspective.  The base class reverses the
            // operator; LessThan is not in the allow-list.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where 'abc' > entity_id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "operator LessThan is not allowed on column dbo.users.entity_id" },
                },
            },
        };

        // Scripts whose filter operators are either on unconfigured columns OR are in the
        // allow-list of configured columns — the rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No filter at all.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users" },

            // Allowed operators on the constrained column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'abc'" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id In ('a', 'b', 'c')" },

            // Any operator on a non-configured column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where name Like 'abc%'" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where age > 18" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where created_at Between '2024-01-01' And '2024-12-31'" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where deleted_at Is Null" },

            // Allowed operator on alias-qualified entity_id.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Where u.entity_id = 'abc'" },

            // Allowed operator on fully-qualified entity_id.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where dbo.users.entity_id = 'abc'" },

            // JOIN ON with the constrained column.  u.entity_id is compared to o.user_id
            // with Equal — allowed for entity_id; o.user_id is unconfigured.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id" },

            // Multi-table query, bare reference to a column NOT in the configured set —
            // doesn't trigger the "must be qualified" violation (only configured names do).
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where name Like 'abc%'" },

            // Multiple AND-ed predicates — only entity_id has a constraint and it's used
            // with an allowed operator.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'abc' And name Like 'foo%' And age > 18" },

            // Same column referenced multiple times — all uses allowed operators.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'a' And entity_id In ('a', 'b')" },

            // Bare column reference in a SINGLE-table FROM — resolves to that one table.
            // entity_id with Equal is allowed.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'abc'" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_ConstrainedFilterOperatorsByColumnSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new ConstrainedFilterOperatorsByColumnSqlScriptValidationRule(
                ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleConfig);

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
        public static void Execute___Should_return_no_violations___When_ConstrainedFilterOperatorsByColumnSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleNoViolationScenarios;

            var rule = new ConstrainedFilterOperatorsByColumnSqlScriptValidationRule(
                ConstrainedFilterOperatorsByColumnSqlScriptValidationRuleConfig);

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
