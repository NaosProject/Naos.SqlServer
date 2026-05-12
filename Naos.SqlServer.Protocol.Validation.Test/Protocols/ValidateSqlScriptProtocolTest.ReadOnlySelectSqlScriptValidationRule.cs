// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.ReadOnlySelectSqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts that violate the rule — either they contain a non-SELECT top-level statement,
        // or they contain a SELECT ... INTO (which writes).
        private static readonly IReadOnlyList<TestScenariosWithExpected> ReadOnlySelectSqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // INSERT (top-level InsertStatement) — not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Insert Into dbo.t (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // UPDATE — not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Update dbo.t Set x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // DELETE — not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Delete From dbo.t Where x = 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // MERGE — not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Merge Into dbo.t As tgt Using dbo.src As src On tgt.id = src.id When Matched Then Update Set x = src.x;",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // CREATE TABLE — DDL, not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Create Table dbo.t (x Int)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // DROP TABLE — DDL, not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Drop Table dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // EXEC — ExecuteStatement, not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Exec dbo.my_sproc",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // SET — PredicateSetStatement / SetCommandStatement, not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Set NoCount On",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // DECLARE — DeclareVariableStatement, not a SELECT.
            new TestScenariosWithExpected
            {
                Sql = "Declare @x Int",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // IF wrapper — top-level is IfStatement, NOT a SelectStatement, even though the body
            // is itself a read-only SELECT.  Strict interpretation of the rule.
            new TestScenariosWithExpected
            {
                Sql = "If 1 = 1 Select 1",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // WHILE wrapper — same reasoning.
            new TestScenariosWithExpected
            {
                Sql = "While 1 = 0 Begin Break End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // BEGIN/END wrapper — top-level is BeginEndBlockStatement, NOT a SelectStatement.
            new TestScenariosWithExpected
            {
                Sql = "Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // TRY/CATCH wrapper — top-level is TryCatchStatement.
            new TestScenariosWithExpected
            {
                Sql = "Begin Try Select 1 End Try Begin Catch Select Error_Message() End Catch",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // CREATE PROCEDURE — DDL.
            new TestScenariosWithExpected
            {
                Sql = "Create Procedure dbo.sp As Begin Select 1 End",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "statement is not a read-only SELECT statement" },
                },
            },

            // SELECT ... INTO — IS a SelectStatement but is NOT read-only (it creates / writes to
            // the target table).  Different violation message.
            new TestScenariosWithExpected
            {
                Sql = "Select 1 As x Into dbo.new_t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "SELECT INTO is not read-only" },
                },
            },

            // SELECT ... INTO ... FROM — same SELECT INTO violation.
            new TestScenariosWithExpected
            {
                Sql = "Select * Into dbo.new_t From dbo.src",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 0, Details = "SELECT INTO is not read-only" },
                },
            },

            // Mixed multi-statement: leading SELECT is fine, trailing INSERT violates.  Only the
            // INSERT fires; the SELECT does not.
            new TestScenariosWithExpected
            {
                Sql = "Select 1; Insert Into dbo.t (x) Values (1)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 10, Details = "statement is not a read-only SELECT statement" },
                },
            },
        };

        // Scripts whose top-level statements are all read-only SELECTs — the rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> ReadOnlySelectSqlScriptValidationRuleNoViolationScenarios = new[]
        {
            new TestScenariosWithExpected { Sql = "Select 1" },
            new TestScenariosWithExpected { Sql = "Select 1;" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where x = 1" },
            new TestScenariosWithExpected { Sql = "Select a.x From dbo.t a Inner Join dbo.s b On a.id = b.id" },
            new TestScenariosWithExpected { Sql = "Select * From (Select * From dbo.t) x" },
            new TestScenariosWithExpected { Sql = "With cte As (Select 1 As x) Select * From cte" },
            new TestScenariosWithExpected { Sql = "Select 1 As x Union Select 2 As x" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Cross Apply (Select 1 As x) y" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t For Xml Path('')" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t For Json Auto" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t With (NoLock)" },
            new TestScenariosWithExpected { Sql = "Select @@Version" },
            new TestScenariosWithExpected { Sql = "Select Top 10 * From dbo.t Order By x" },
            new TestScenariosWithExpected { Sql = "Select Count(*) From dbo.t" },

            // Multi-statement script where every statement is a read-only SELECT — this rule
            // does not enforce single-statement; that's a separate rule.
            new TestScenariosWithExpected { Sql = "Select 1; Select 2" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_ReadOnlySelectSqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ReadOnlySelectSqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new ReadOnlySelectSqlScriptValidationRule();

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
        public static void Execute___Should_return_no_violations___When_ReadOnlySelectSqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = ReadOnlySelectSqlScriptValidationRuleNoViolationScenarios;

            var rule = new ReadOnlySelectSqlScriptValidationRule();

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
