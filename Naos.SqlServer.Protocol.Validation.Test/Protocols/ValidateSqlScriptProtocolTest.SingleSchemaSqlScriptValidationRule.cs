// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SingleSchemaSqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts that reference more than one schema — the rule fires once per non-canonical
        // reference.  The "canonical" schema is whichever the visitor sees first.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SingleSchemaSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // Two schemas joined within one statement — first schema seen is "dbo", second is
            // the violation.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1 a Inner Join my_schema.t2 b On a.id = b.id",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 34, Details = "reference to additional schema: my_schema" },
                },
            },

            // Three different schemas — two violations (every reference different from "a").
            new TestScenariosWithExpected
            {
                Sql = "Select * From a.t1 Cross Join b.t2 Cross Join c.t3",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 30, Details = "reference to additional schema: b" },
                    new ExpectedViolation { Offset = 46, Details = "reference to additional schema: c" },
                },
            },

            // Different schemas across semicolon-separated statements.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1; Select * From my_schema.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 36, Details = "reference to additional schema: my_schema" },
                },
            },

            // Different schemas across GO-separated batches.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1\r\nGO\r\nSelect * From my_schema.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 40, Details = "reference to additional schema: my_schema" },
                },
            },

            // CTE with one schema, outer SELECT with another.  Visit order: the outer FROM is
            // visited BEFORE the CTE body, so "my_schema" is canonical and "dbo" (inside the
            // CTE body) is the one flagged.  This is the same visit-order quirk documented on
            // the rule's <remarks/>.
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select id From dbo.t1) Select * From my_schema.t2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 28, Details = "reference to additional schema: dbo" },
                },
            },

            // Subquery in WHERE references a different schema than the outer FROM.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where x In (Select id From my_schema.t)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 47, Details = "reference to additional schema: my_schema" },
                },
            },

            // CREATE SCHEMA introduces a schema reference (via the bare Identifier on Name),
            // then a subsequent SELECT against a different schema is flagged.  GO is required
            // here as the batch separator — T-SQL's CREATE SCHEMA grammar can greedily consume
            // following statements as schema_elements (CREATE TABLE, CREATE VIEW, GRANT, etc.),
            // so a plain ";" does not cleanly terminate it before a subsequent SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Create Schema my_schema\r\nGO\r\nSelect * From dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 43, Details = "reference to additional schema: dbo" },
                },
            },

            // Three different schemas across three statements — two violations.
            new TestScenariosWithExpected
            {
                Sql = "Select * From a.t1; Select * From b.t2; Select * From c.t3",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 34, Details = "reference to additional schema: b" },
                    new ExpectedViolation { Offset = 54, Details = "reference to additional schema: c" },
                },
            },

            // GRANT ON SCHEMA::<one> followed by a SELECT against a different schema.
            // The Schema-kind security target counts as a schema reference.
            new TestScenariosWithExpected
            {
                Sql = "Grant Select On Schema::my_schema To my_user; Select * From dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 60, Details = "reference to additional schema: dbo" },
                },
            },

            // Repeated reference to the same non-canonical schema produces multiple violations
            // — one per reference site, not de-duplicated.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t1 Inner Join my_schema.t2 On 1=1 Inner Join my_schema.t3 On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 32, Details = "reference to additional schema: my_schema" },
                    new ExpectedViolation { Offset = 63, Details = "reference to additional schema: my_schema" },
                },
            },
        };

        // Scripts that reference at most one schema (case-insensitively) — the rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SingleSchemaSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No schema references at all.
            new TestScenariosWithExpected { Sql = "Select 1" },

            // Single explicit reference.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },

            // Same schema multiple times.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1 Inner Join dbo.t2 On 1=1" },

            // Same schema across statements.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1; Select * From dbo.t2" },

            // Same schema across GO batches.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1\r\nGO\r\nSelect * From dbo.t2" },

            // Same schema in CTE body and outer SELECT.
            new TestScenariosWithExpected { Sql = "With cte As (Select id From dbo.t1) Select * From dbo.t2" },

            // Same schema in subquery and outer.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where x In (Select id From dbo.t2)" },

            // Case-insensitive match — "DBO" and "dbo" treated as the same schema.
            new TestScenariosWithExpected { Sql = "Select * From DBO.t1 Inner Join dbo.t2 On 1=1" },

            // Case-insensitive match with three case variations.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1 Inner Join Dbo.t2 On 1=1 Inner Join DBO.t3 On 1=1" },

            // Bare references only — no schemas counted, rule has nothing to compare.
            new TestScenariosWithExpected { Sql = "Select * From t1 Inner Join t2 On 1=1" },

            // Mix of bare and schema-qualified — only one explicit schema, bare ignored.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1 Inner Join t2 On 1=1" },

            // Three-part name — only the schema identifier counts, not the database.
            // Both rows reference schema "dbo" (different databases, same schema name).
            new TestScenariosWithExpected { Sql = "Select * From mydb.dbo.t1 Inner Join db2.dbo.t2 On 1=1" },

            // Schema-level statement alone — single schema, no violation.
            new TestScenariosWithExpected { Sql = "Create Schema my_schema" },

            // Non-SELECT statement that uses a single schema.
            new TestScenariosWithExpected { Sql = "Update dbo.t Set x = 1" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SingleSchemaSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SingleSchemaSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SingleSchemaSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_SingleSchemaSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SingleSchemaSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SingleSchemaSqlScriptValidationRule();

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
