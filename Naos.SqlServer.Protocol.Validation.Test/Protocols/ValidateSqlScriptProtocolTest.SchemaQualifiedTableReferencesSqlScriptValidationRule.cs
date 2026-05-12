// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SchemaQualifiedTableReferencesSqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts with at least one bare (non-schema-qualified) NamedTableReference — the rule
        // fires once per offending reference.  Temp tables (#x, ##x) are exempt; CTE name
        // references (FROM cte) ARE flagged (documented limitation; pair with FlatQuery).
        private static readonly IReadOnlyList<TestScenariosWithExpected> SchemaQualifiedTableReferencesSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // Bare table in SELECT FROM.
            new TestScenariosWithExpected
            {
                Sql = "Select * From t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "table reference is not schema-qualified: t" },
                },
            },

            // Mixed: qualified left, bare right of JOIN.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Inner Join t On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "table reference is not schema-qualified: t" },
                },
            },

            // Two bare tables in JOIN — both flagged.
            new TestScenariosWithExpected
            {
                Sql = "Select * From a Inner Join b On 1=1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "table reference is not schema-qualified: a" },
                    new ExpectedViolation { Offset = 27, Details = "table reference is not schema-qualified: b" },
                },
            },

            // UPDATE target is bare.
            new TestScenariosWithExpected
            {
                Sql = "Update t Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 7, Details = "table reference is not schema-qualified: t" },
                },
            },

            // DELETE target is bare.
            new TestScenariosWithExpected
            {
                Sql = "Delete From t Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "table reference is not schema-qualified: t" },
                },
            },

            // INSERT target is bare.
            new TestScenariosWithExpected
            {
                Sql = "Insert Into t (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "table reference is not schema-qualified: t" },
                },
            },

            // MERGE target bare, USING source qualified — only the bare target fires.
            new TestScenariosWithExpected
            {
                Sql = "Merge Into t As tgt Using dbo.s As src On tgt.id = src.id When Matched Then Update Set x = src.x;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "table reference is not schema-qualified: t" },
                },
            },

            // Bare reference inside a subquery in a WHERE clause.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.a Where x In (Select id From t)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 47, Details = "table reference is not schema-qualified: t" },
                },
            },

            // Bare reference inside a derived table in FROM.
            new TestScenariosWithExpected
            {
                Sql = "Select * From (Select * From t) x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 29, Details = "table reference is not schema-qualified: t" },
                },
            },

            // Bracketed bare identifier — brackets don't add a schema, still flagged.
            new TestScenariosWithExpected
            {
                Sql = "Select * From [my_table]",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "table reference is not schema-qualified: my_table" },
                },
            },

            // KNOWN LIMITATION: CTE name reference (FROM cte) is indistinguishable in the AST
            // from a real bare table reference, so the rule flags it.  Compose with FlatQuery
            // (which blocks CTEs entirely) to avoid the false positive in practice.
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select * From dbo.t) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 48, Details = "table reference is not schema-qualified: cte" },
                },
            },
        };

        // Scripts whose every NamedTableReference is either schema-qualified or exempt — the
        // rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SchemaQualifiedTableReferencesSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // No tables at all.
            new TestScenariosWithExpected { Sql = "Select 1" },

            // Single qualified table.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },

            // Two qualified tables joined.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On 1=1" },

            // Different schemas, both qualified.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join my_schema.b On 1=1" },

            // Three-part name — schema is present (in the middle).
            new TestScenariosWithExpected { Sql = "Select * From mydb.dbo.t" },

            // Bracket-delimited schema + table.
            new TestScenariosWithExpected { Sql = "Select * From [dbo].[t]" },

            // Qualified DML targets.
            new TestScenariosWithExpected { Sql = "Update dbo.t Set x = 1" },
            new TestScenariosWithExpected { Sql = "Delete From dbo.t Where x = 1" },
            new TestScenariosWithExpected { Sql = "Insert Into dbo.t (x) Values (1)" },
            new TestScenariosWithExpected { Sql = "Merge Into dbo.t As tgt Using dbo.s As src On tgt.id = src.id When Matched Then Update Set x = src.x;" },

            // Temp tables — local (#) and global (##).  Exempt by convention.
            new TestScenariosWithExpected { Sql = "Select * From #temp_t" },
            new TestScenariosWithExpected { Sql = "Select * From ##global_temp" },

            // Mixed qualified + temp — temp is exempt.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Inner Join #temp_t On 1=1" },

            // Table variable — parsed as VariableTableReference, not NamedTableReference, so
            // it's outside the rule's scope automatically.
            new TestScenariosWithExpected { Sql = "Select * From @tv" },

            // Mixed qualified + table variable.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Inner Join @tv As v On 1=1" },

            // Qualified view reference — the rule doesn't distinguish tables from views; both
            // are NamedTableReference.  A qualified view passes.
            new TestScenariosWithExpected { Sql = "Select * From dbo.my_view" },

            // Multi-statement, every reference qualified.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t1; Select * From dbo.t2" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SchemaQualifiedTableReferencesSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SchemaQualifiedTableReferencesSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SchemaQualifiedTableReferencesSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_SchemaQualifiedTableReferencesSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SchemaQualifiedTableReferencesSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SchemaQualifiedTableReferencesSqlScriptValidationRule();

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
