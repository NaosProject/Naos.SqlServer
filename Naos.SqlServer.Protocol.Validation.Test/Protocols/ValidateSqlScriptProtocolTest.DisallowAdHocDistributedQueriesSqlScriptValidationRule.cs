// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.DisallowAdHocDistributedQueriesSqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts that use OPENROWSET (any variant), OPENQUERY, or OPENDATASOURCE — all fire.
        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowAdHocDistributedQueriesSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // OPENROWSET with OLE DB provider — the classic dynamic-SQL back door.
            new TestScenariosWithExpected
            {
                Sql = "Select * From OpenRowset('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'Select 1')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "ad-hoc distributed query is not allowed: OPENROWSET" },
                },
            },

            // OPENROWSET BULK — reads arbitrary file contents from the server file system.
            new TestScenariosWithExpected
            {
                Sql = "Select * From OpenRowset(Bulk 'C:\\file.csv', Single_Blob) As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "ad-hoc distributed query is not allowed: OPENROWSET BULK" },
                },
            },

            // OPENQUERY against a (notional) linked server — opaque payload sent remotely.
            new TestScenariosWithExpected
            {
                Sql = "Select * From OpenQuery(MY_LINKED, 'Select 1')",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "ad-hoc distributed query is not allowed: OPENQUERY" },
                },
            },

            // OPENDATASOURCE four-part name — ad-hoc remote source.
            new TestScenariosWithExpected
            {
                Sql = "Select * From OpenDataSource('SQLNCLI', 'Data Source=.').mydb.dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "ad-hoc distributed query is not allowed: OPENDATASOURCE" },
                },
            },

            // OPENROWSET as the target of INSERT — sends DML to a remote server via ad-hoc query.
            new TestScenariosWithExpected
            {
                Sql = "Insert Into OpenRowset('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'Select x From dbo.t') Select 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 12, Details = "ad-hoc distributed query is not allowed: OPENROWSET" },
                },
            },

            // OPENROWSET inside a CTE body — the visitor still descends and flags it.
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select * From OpenRowset('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'Select 1')) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 27, Details = "ad-hoc distributed query is not allowed: OPENROWSET" },
                },
            },

            // Mixed: OPENROWSET joined with OPENQUERY — two distinct violations.
            new TestScenariosWithExpected
            {
                Sql = "Select * From OpenRowset('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'Select 1') a Inner Join OpenQuery(MY_LINKED, 'Select 1') b On 1 = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "ad-hoc distributed query is not allowed: OPENROWSET" },
                    new ExpectedViolation { Offset = 97, Details = "ad-hoc distributed query is not allowed: OPENQUERY" },
                },
            },
        };

        // Scripts that are syntactically similar but are NOT ad-hoc distributed queries — the
        // rule must leave these alone.
        private static readonly IReadOnlyList<TestScenariosWithExpected> DisallowAdHocDistributedQueriesSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // Plain SELECT — nothing remotely related.
            new TestScenariosWithExpected { Sql = "Select 1" },

            // Regular table reference.
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },

            // Four-part name to a real linked server (no OPENDATASOURCE) — not ad-hoc, the
            // linked server is pre-configured.  This rule does not block four-part names.
            new TestScenariosWithExpected { Sql = "Select * From MY_LINKED.mydb.dbo.t" },

            // OPENJSON — in-memory JSON parsing, NOT a distributed query.
            new TestScenariosWithExpected { Sql = "Select * From OpenJson(@json)" },

            // OPENJSON with WITH clause — same, still in-memory.
            new TestScenariosWithExpected { Sql = "Select * From OpenJson(@json) With (x Int '$.x')" },

            // OPENXML — in-memory XML parsing against a prepared document handle.
            new TestScenariosWithExpected { Sql = "Select * From OpenXml(@idoc, '/Root/Item')" },

            // User-defined table-valued function call — not an ad-hoc distributed query.
            // (The rule does not attempt to inspect what a UDF does internally.)
            new TestScenariosWithExpected { Sql = "Select * From dbo.my_tvf()" },

            // CTE built from a regular table.
            new TestScenariosWithExpected { Sql = "With cte As (Select 1 As x) Select * From cte" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_DisallowAdHocDistributedQueriesSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowAdHocDistributedQueriesSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new DisallowAdHocDistributedQueriesSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_DisallowAdHocDistributedQueriesSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = DisallowAdHocDistributedQueriesSqlScriptValidationRuleNoViolationScenarios;

            var rule = new DisallowAdHocDistributedQueriesSqlScriptValidationRule();

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
