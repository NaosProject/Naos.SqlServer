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
        // the trigger columns.  When a query's filter clauses reference either (resolved or
        // bare-name match in multi-table queries), the query's filters must be a simple
        // conjunction.  Two columns are configured so the multi-column message branch is
        // exercised.
        private static readonly IReadOnlyCollection<SchemaQualifiedColumnName> ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleConfig = new[]
        {
            new SchemaQualifiedColumnName("dbo", "users", "entity_id"),
            new SchemaQualifiedColumnName("dbo", "users", "tenant_id"),
        };

        // Scripts where a configured column is referenced AND an OR or explicit NOT wrapper
        // appears in some filter clause — the rule fires.  Offsets match the conventions of
        // SimpleConjunctiveFilterSqlScriptValidationRule: BBE-Or at its leftmost operand,
        // BNot at the "NOT" keyword.
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

            // WHERE: NOT wrapper around the only filter — entity_id is referenced INSIDE the
            // NOT.  Base walks through NOT to leaf, so HandleResolvedFilterPredicate fires
            // (column added), then OnComplete sees the NOT wrapper.
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

            // WHERE: nested OR inside an AND.  entity_id is in the AND's left branch; OR is
            // in the right branch.  Cross-branch coverage — column referenced anywhere
            // triggers the OR-anywhere check.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' And (a = 1 Or b = 2)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 51, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
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

            // Cross-clause: entity_id only in WHERE, OR only in JOIN ON.  Column recorded
            // during WHERE walk; OnComplete walks JOIN ON and emits there.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Or u.id = o.id Where u.entity_id = 'x'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 53, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Multi-table: bare reference to "entity_id" cannot be resolved without schema
            // introspection.  Bare name matches a configured column's name → the rule errs on
            // the safe side and records the configured column.  OR in WHERE fires.
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

            // Chained ORs at the top — only the outermost BBE-Or fires (recursion
            // short-circuits on first OR encountered).  Column recorded because entity_id
            // appears in the leftmost branch.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' Or a = 1 Or b = 2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // JOIN ON references u.entity_id (records the column); WHERE has a NOT wrapper
            // that does not reference entity_id but still fires because a constrained
            // column was recorded.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id Where Not (a = 1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 83, Details = "NOT not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Multi-table bare-name match in WHERE (via IS NOT NULL — an inline-not leaf, NOT
            // a BooleanNotExpression), with OR in JOIN ON.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Or u.id = o.id Where entity_id Is Not Null",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 53, Details = "OR not allowed in filter referencing constrained column dbo.users.entity_id; filter must be a simple conjunction" },
                },
            },

            // Multi-column reference — BOTH entity_id and tenant_id are referenced in the
            // query.  The message lists both, sorted (case-sensitive ordinal).  OR fires at
            // the outer BBE-Or which wraps the AND-conjunction-with-tenant-id on the left
            // and `name = 'z'` on the right (AND binds tighter than OR).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where entity_id = 'x' And tenant_id = 'y' Or name = 'z'",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "OR not allowed in filter referencing constrained columns dbo.users.entity_id, dbo.users.tenant_id; filter must be a simple conjunction" },
                },
            },
        };

        // Scripts where the rule does NOT fire — either because no configured column is
        // referenced in any filter clause, or because the query's filters are pure
        // conjunctions.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ColumnScopedSimpleConjunctiveFilterSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No filter at all.
            new TestScenariosWithExpected { Sql = "Select 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users" },

            // OR in WHERE but no configured column referenced — the key differentiator from
            // the unscoped SimpleConjunctiveFilter rule.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where name = 'x' Or age = 18" },

            // NOT in WHERE but no configured column referenced.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where Not (name = 'x')" },

            // Configured column with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And name = 'y'" },

            // Configured column alone.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x'" },

            // Both configured columns with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id = 'x' And tenant_id = 'y'" },

            // Multi-table with bare reference to an UNCONFIGURED column name and OR — bare
            // "name" doesn't match either configured column name, no column is recorded.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.id = o.user_id Where name = 'x' Or age = 18" },

            // JOIN ON OR with no configured column reference anywhere.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Inner Join dbo.orders o On dbo.users.id = o.user_id Or dbo.users.id = o.id Where name = 'x'" },

            // Alias-qualified configured column with pure AND.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Where u.entity_id = 'x' And u.name = 'y'" },

            // Inline-not predicates on the configured column — these are LEAF predicates
            // (not BooleanNotExpression wrappers), so they pass even though entity_id is
            // referenced.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where entity_id <> 'x' And entity_id Not In ('a', 'b') And entity_id Is Not Null" },

            // Configured column referenced in JOIN ON with pure AND in WHERE.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users u Inner Join dbo.orders o On u.entity_id = o.user_id Where u.name = 'x' And o.amount > 0" },

            // Single-table query against a non-configured table — bare "entity_id" resolves
            // to dbo.audit_log.entity_id, not dbo.users.entity_id; full identity mismatches,
            // so no column is recorded even though OR is present.
            new TestScenariosWithExpected { Sql = "Select * From dbo.audit_log Where entity_id = 'x' Or x = 1" },

            // Multi-table against non-configured tables — alias-qualified "a.entity_id"
            // resolves to dbo.audit_log.entity_id; bare "x" doesn't match any configured
            // column name.  Nothing recorded.
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
