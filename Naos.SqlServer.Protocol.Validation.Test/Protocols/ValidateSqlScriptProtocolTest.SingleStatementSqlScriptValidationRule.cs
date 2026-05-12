// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.SingleStatementSqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts whose top-level statement count is NOT exactly 1 — the rule fires.  The
        // expected offset points at the second statement (the first "extra" the script
        // shouldn't contain); the message ends with the actual count.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SingleStatementSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // two simple SELECTs, semicolon-separated within a single batch
            new TestScenariosWithExpected
            {
                Sql = "Select 1; Select 2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 10, Details = "script must contain a single SQL statement; found 2" },
                },
            },

            // two simple SELECTs, GO-separated (one statement per batch — still 2 top-level)
            new TestScenariosWithExpected
            {
                Sql = "Select 1\r\nGO\r\nSelect 2",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 14, Details = "script must contain a single SQL statement; found 2" },
                },
            },

            // three statements
            new TestScenariosWithExpected
            {
                Sql = "Select 1; Select 2; Select 3",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 10, Details = "script must contain a single SQL statement; found 3" },
                },
            },

            // SET + SELECT — common real-world "preamble + query" pattern
            new TestScenariosWithExpected
            {
                Sql = "Set NoCount On; Select 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 16, Details = "script must contain a single SQL statement; found 2" },
                },
            },

            // DECLARE + SELECT
            new TestScenariosWithExpected
            {
                Sql = "Declare @x Int = 1; Select @x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "script must contain a single SQL statement; found 2" },
                },
            },

            // INSERT + SELECT
            new TestScenariosWithExpected
            {
                Sql = "Insert Into dbo.t (x) Values (1); Select * From dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 34, Details = "script must contain a single SQL statement; found 2" },
                },
            },

            // multiple statements across multiple batches
            new TestScenariosWithExpected
            {
                Sql = "Select 1; Select 2\r\nGO\r\nSelect 3; Select 4",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 10, Details = "script must contain a single SQL statement; found 4" },
                },
            },
        };

        // Scripts whose top-level statement count IS exactly 1 — the rule passes.
        // These exercise the "inner statements don't count" semantics by including constructs
        // that contain nested statement lists (BEGIN/END, IF/WHILE/TRY-CATCH bodies, stored-
        // procedure/function bodies) — each is one top-level statement regardless of how many
        // statements live inside.
        private static readonly IReadOnlyList<TestScenariosWithExpected> SingleStatementSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // simplest case
            new TestScenariosWithExpected { Sql = "Select 1" },

            // trailing semicolon — still one statement
            new TestScenariosWithExpected { Sql = "Select 1;" },

            // SELECT with a CTE — single SelectStatement
            new TestScenariosWithExpected { Sql = "With cte As (Select 1 As x) Select * From cte" },

            // CREATE PROCEDURE whose body contains multiple inner statements — still one
            // top-level statement (CreateProcedureStatement).
            new TestScenariosWithExpected { Sql = "Create Procedure dbo.sp As Begin Set NoCount On; Select 1; Select 2 End" },

            // CREATE FUNCTION with a multi-statement body — one top-level CreateFunctionStatement.
            new TestScenariosWithExpected { Sql = "Create Function dbo.fn () Returns Int As Begin Return 1 End" },

            // standalone BEGIN/END block with multiple inner statements — one top-level
            // BeginEndBlockStatement.
            new TestScenariosWithExpected { Sql = "Begin Select 1; Select 2 End" },

            // IF with single-statement body — one IfStatement.
            new TestScenariosWithExpected { Sql = "If 1 = 1 Select 1" },

            // IF/ELSE with multi-statement bodies — one IfStatement.
            new TestScenariosWithExpected { Sql = "If 1 = 1 Begin Select 1 End Else Begin Select 2 End" },

            // WHILE with a body — one WhileStatement.
            new TestScenariosWithExpected { Sql = "While 1 = 0 Begin Break End" },

            // TRY/CATCH block — one TryCatchStatement, regardless of how many inner statements.
            new TestScenariosWithExpected { Sql = "Begin Try Select 1 End Try Begin Catch Select Error_Message() End Catch" },

            // single DML statements
            new TestScenariosWithExpected { Sql = "Insert Into dbo.t (x) Values (1)" },
            new TestScenariosWithExpected { Sql = "Update dbo.t Set x = 1" },
            new TestScenariosWithExpected { Sql = "Delete From dbo.t Where x = 1" },

            // DDL
            new TestScenariosWithExpected { Sql = "Create Table dbo.t (x Int)" },

            // MERGE requires a terminating semicolon — that's a terminator, not a second
            // statement.  Still one MergeStatement.
            new TestScenariosWithExpected { Sql = "Merge Into dbo.t As tgt Using dbo.src As src On tgt.id = src.id When Matched Then Update Set x = src.x;" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_SingleStatementSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SingleStatementSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new SingleStatementSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_SingleStatementSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = SingleStatementSqlScriptValidationRuleNoViolationScenarios;

            var rule = new SingleStatementSqlScriptValidationRule();

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
