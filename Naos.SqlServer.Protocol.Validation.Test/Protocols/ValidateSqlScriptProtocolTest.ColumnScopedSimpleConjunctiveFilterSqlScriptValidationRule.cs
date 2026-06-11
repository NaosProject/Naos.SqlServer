// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule.cs" company="Naos Project">
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
        // Rule config used by both tests: dbo.users.entity_id and dbo.users.tenant_id are
        // the constrained columns — they may not be referenced within an OR or NOT filter
        // sub-expression.  Two columns are configured so the multi-column message branch is
        // exercised.
        private static readonly IReadOnlyCollection<SchemaQualifiedColumnName> ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleConfig = new[]
        {
            new SchemaQualifiedColumnName("dbo", "users", "entity_id"),
            new SchemaQualifiedColumnName("dbo", "users", "tenant_id"),
        };

        // Scripts where an OR / NOT subtree references a constrained column — the rule
        // fires.  Offsets: BBE-Or at its leftmost operand, BNot at the "NOT" keyword.
        // The violation message names the constrained columns referenced WITHIN the
        // offending subtree (sorted ordinal), not all columns referenced in the query.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // WHERE: entity_id used in OR's left branch — fires at OR's leftmost operand.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' Or name = 'y'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // WHERE: NOT wrapper whose operand references entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where Not (entity_id = 'x')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "NOT not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // WHERE: both OR branches reference entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' Or entity_id = 'y'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // WHERE: OR nested inside an AND, with entity_id INSIDE the OR's branches —
            // fires at the inner OR.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where name = 'x' And (entity_id = 'a' Or entity_id = 'b')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 46, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // WHERE: NOT nested inside an AND, with entity_id INSIDE the NOT's operand —
            // fires at the NOT.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where name = 'x' And Not (entity_id = 'a')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 45, Details = "NOT not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // WHERE: entity_id deep in the OR's RIGHT branch — the outer OR fires (the
            // constrained predicate is optional for rows matching the left branch).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where a = 1 Or (b = 2 And entity_id = 'x')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // HAVING: entity_id in HAVING with OR.
            new TestScenariosWithExpected
            {
                Sql = "Select entity_id From dbo.users Group By entity_id Having entity_id = 'x' Or Count(*) > 0",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 58, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // JOIN ON: OR whose subtree references u.entity_id — fires at the OR in the ON.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id Or u.id = o.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 53, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Multi-table: bare reference to "entity_id" inside the OR cannot be resolved
            // without schema introspection.  Bare name matches a configured column's name →
            // the rule errs on the safe side and fires.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where entity_id = 'x' Or name = 'y'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 76, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Alias-qualified entity_id resolves through the alias map to dbo.users.entity_id.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Where u.entity_id = 'x' Or u.name = 'y'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 32, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Fully-qualified entity_id reference.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where dbo.users.entity_id = 'x' Or name = 'y'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Chained ORs — the outermost BBE-Or contains entity_id (in its leftmost
            // branch) and fires once; recursion stops there.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' Or a = 1 Or b = 2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Multi-column reference — BOTH entity_id and tenant_id are inside the OR
            // subtree (`(entity_id = 'x' And tenant_id = 'y') Or name = 'z'`; AND binds
            // tighter than OR).  The message lists both, sorted (case-sensitive ordinal).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' And tenant_id = 'y' Or name = 'z'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained columns dbo.users.entity_id, dbo.users.tenant_id; filter must be a simple conjunction" },
                },
            },
        };

        // Scripts where the rule does NOT fire — either because no OR / NOT subtree
        // references a constrained column, or because there is no OR / NOT at all.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No filter at all.
            new TestScenariosWithExpected { Sql = "Select 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users" },

            // OR in WHERE but no constrained column referenced anywhere.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where name = 'x' Or age = 18" },

            // NOT in WHERE but no constrained column referenced.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where Not (name = 'x')" },

            // Constrained column with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And name = 'y'" },

            // Constrained column alone.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x'" },

            // Both constrained columns with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And tenant_id = 'y'" },

            // THE KEY SHAPE this rule permits (and the binary referenced-anywhere
            // implementation used to reject): constrained column AND-ed OUTSIDE an OR whose
            // branches only reference other columns.  Every result row still satisfies the
            // entity_id predicate.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And ((calendar_year = 2026 And calendar_quarter = 1) Or (calendar_year = 2025 And calendar_quarter In (1, 4)))" },

            // Same idea, simpler: entity_id outside, OR over other columns inside an AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And (a = 1 Or b = 2)" },

            // entity_id outside, NOT over another column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And Not (a = 1)" },

            // entity_id filtered conjunctively in WHERE; OR in JOIN ON whose subtree does
            // not reference a constrained column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Or u.id = o.id Where u.entity_id = 'x'" },

            // entity_id in JOIN ON (conjunctive); NOT in WHERE over another column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id Where Not (a = 1)" },

            // Bare entity_id in WHERE via an inline-not leaf (IS NOT NULL — not a
            // BooleanNotExpression); OR in JOIN ON does not reference a constrained column.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Or u.id = o.id Where entity_id Is Not Null" },

            // Multi-table with bare reference to an UNCONFIGURED column name inside the OR —
            // bare "name" / "age" don't match either configured column name.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where name = 'x' Or age = 18" },

            // JOIN ON OR with no constrained column reference anywhere.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Inner Join dbo.orders o On dbo.users.id = o.user_id Or dbo.users.id = o.id Where name = 'x'" },

            // Alias-qualified constrained column with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Where u.entity_id = 'x' And u.name = 'y'" },

            // Inline-not predicates on the constrained column — these are LEAF predicates
            // (not BooleanNotExpression wrappers), so they pass.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id <> 'x' And entity_id Not In ('a', 'b') And entity_id Is Not Null" },

            // Constrained column referenced in JOIN ON with pure AND in WHERE.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id Where u.name = 'x' And o.amount > 0" },

            // Single-table query against a non-configured table — bare "entity_id" inside
            // the OR resolves to dbo.audit_log.entity_id, not dbo.users.entity_id; full
            // identity mismatches.
            new TestScenariosWithExpected { Sql = "Select * From dbo.audit_log Where entity_id = 'x' Or x = 1" },

            // Multi-table against non-configured tables — alias-qualified "a.entity_id"
            // resolves to dbo.audit_log.entity_id; bare "x" doesn't match any configured
            // column name.
            new TestScenariosWithExpected { Sql = "Select * From dbo.audit_log a Inner Join dbo.something b On a.id = b.aid Where a.entity_id = 'x' Or x = 1" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule(
                ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleConfig);

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
        public static void Execute___Should_return_no_violations___When_ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleNoViolationScenarios;

            var rule = new ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRule(
                ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleConfig);

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
