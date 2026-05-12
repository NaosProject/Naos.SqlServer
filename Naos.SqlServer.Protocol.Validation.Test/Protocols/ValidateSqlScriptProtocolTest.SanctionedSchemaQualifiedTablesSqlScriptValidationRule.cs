// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SanctionedSchemaQualifiedTablesSqlScriptValidationRule.cs" company="Naos Project">
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
        // Sanctioned set used by both tests: dbo.users, dbo.orders, audit.events.  Covers
        // same-schema-different-table and different-schema cases.
        private static readonly IReadOnlyCollection<SchemaQualifiedTableName> SanctionedSchemaQualifiedTablesSqlScriptValidationRuleSanctioned = new[]
        {
            new SchemaQualifiedTableName("dbo", "users"),
            new SchemaQualifiedTableName("dbo", "orders"),
            new SchemaQualifiedTableName("audit", "events"),
        };

        // Scripts that reference at least one (schema, table) tuple outside the sanctioned set
        // — rule fires once per offending reference.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SanctionedSchemaQualifiedTablesSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // Same schema (sanctioned), different table (not sanctioned).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.products",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned table: dbo.products" },
                },
            },

            // Different schema entirely (not sanctioned).
            new TestScenariosWithExpected
            {
                Sql = "Select * From other.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned table: other.t" },
                },
            },

            // Mixed JOIN: sanctioned + unsanctioned — only the unsanctioned one fires.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Inner Join dbo.products On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 35, Details = "reference to unsanctioned table: dbo.products" },
                },
            },

            // Two unsanctioned tables joined across different schemas — two violations.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.products Inner Join other.t On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned table: dbo.products" },
                    new ExpectedViolation { Offset = 38, Details = "reference to unsanctioned table: other.t" },
                },
            },

            // Two unsanctioned tables in the same (sanctioned) schema — two violations.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.products Inner Join dbo.shipments On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned table: dbo.products" },
                    new ExpectedViolation { Offset = 38, Details = "reference to unsanctioned table: dbo.shipments" },
                },
            },

            // UPDATE target is an unsanctioned table.
            new TestScenariosWithExpected
            {
                Sql = "Update dbo.products Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 7, Details = "reference to unsanctioned table: dbo.products" },
                },
            },

            // DELETE target is an unsanctioned table.
            new TestScenariosWithExpected
            {
                Sql = "Delete From dbo.products Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned table: dbo.products" },
                },
            },

            // INSERT target is an unsanctioned table.
            new TestScenariosWithExpected
            {
                Sql = "Insert Into other.t (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "reference to unsanctioned table: other.t" },
                },
            },

            // Subquery references an unsanctioned table.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.users Where id In (Select aid From other.t)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 53, Details = "reference to unsanctioned table: other.t" },
                },
            },

            // Case is preserved in the violation message — sanctioned set uses lowercase,
            // source uses uppercase, the case-insensitive comparison fires the violation only
            // because "Products" isn't a sanctioned table at all (regardless of case).
            new TestScenariosWithExpected
            {
                Sql = "Select * From DBO.Products",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "reference to unsanctioned table: DBO.Products" },
                },
            },
        };

        // Scripts whose every schema-qualified table reference is in the sanctioned set — the
        // rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SanctionedSchemaQualifiedTablesSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // Each sanctioned tuple referenced on its own.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.orders" },
            new TestScenariosWithExpected { Sql = "Select * From audit.events" },

            // Two sanctioned tuples joined.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Inner Join dbo.orders On 1=1" },

            // Sanctioned tuples in subquery and outer.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Where id In (Select aid From dbo.orders)" },

            // Case-insensitive matching — schema variation.
            new TestScenariosWithExpected { Sql = "Select * From DBO.users" },

            // Case-insensitive matching — table variation.
            new TestScenariosWithExpected { Sql = "Select * From dbo.USERS" },

            // Case-insensitive matching — both varied.
            new TestScenariosWithExpected { Sql = "Select * From DBO.USERS" },

            // Bracket-delimited identifiers.
            new TestScenariosWithExpected { Sql = "Select * From [dbo].[users]" },

            // Three-part name — extra database qualifier doesn't affect schema/table
            // extraction; "dbo.users" still matches.
            new TestScenariosWithExpected { Sql = "Select * From mydb.dbo.users" },

            // Sanctioned target with qualified DML.
            new TestScenariosWithExpected { Sql = "Update dbo.users Set x = 1" },
            new TestScenariosWithExpected { Sql = "Delete From dbo.users Where x = 1" },
            new TestScenariosWithExpected { Sql = "Insert Into dbo.users (x) Values (1)" },

            // Bare reference — silently skipped by this rule; compose with
            // SchemaQualifiedTableReferences to enforce qualification.
            new TestScenariosWithExpected { Sql = "Select * From t" },

            // Mix of bare (skipped) and sanctioned — passes.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Inner Join t On 1=1" },

            // Temp tables — exempt regardless.
            new TestScenariosWithExpected { Sql = "Select * From #temp_t" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.users Inner Join #temp On 1=1" },

            // Table variable — different AST node, automatically out of scope.
            new TestScenariosWithExpected { Sql = "Select * From @tv" },

            // No tables referenced at all.
            new TestScenariosWithExpected { Sql = "Select 1" },

            // DDL target uses SchemaObjectName, not NamedTableReference, so an unsanctioned
            // schema.table in a CREATE TABLE statement is out of this rule's scope.  (Apply a
            // DDL-targeted rule separately if that matters.)
            new TestScenariosWithExpected { Sql = "Create Table other.unsanctioned (x Int)" },

            // Multi-statement, every reference sanctioned.
            new TestScenariosWithExpected { Sql = "Select * From dbo.users; Select * From dbo.orders" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SanctionedSchemaQualifiedTablesSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SanctionedSchemaQualifiedTablesSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SanctionedSchemaQualifiedTablesSqlScriptValidationRule(
                SanctionedSchemaQualifiedTablesSqlScriptValidationRuleSanctioned);

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
        public static void Execute___Should_return_no_violations___When_SanctionedSchemaQualifiedTablesSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SanctionedSchemaQualifiedTablesSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SanctionedSchemaQualifiedTablesSqlScriptValidationRule(
                SanctionedSchemaQualifiedTablesSqlScriptValidationRuleSanctioned);

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
