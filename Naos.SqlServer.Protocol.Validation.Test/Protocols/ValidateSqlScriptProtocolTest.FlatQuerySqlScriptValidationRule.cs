// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.FlatQuerySqlScriptValidationRule.cs" company="Naos Project">
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
        // Scripts that introduce more than one QuerySpecification — the rule fires.  Offsets
        // point at the SECOND QuerySpecification encountered during visitor traversal (not
        // necessarily source-order; see the CTE scenario in particular).
        private static readonly IReadOnlyList<TestScenariosWithExpected> FlatQuerySqlScriptValidationRuleTestScenariosWithExpected = new[]
        {
            // Derived table in FROM — inner SELECT is its own QuerySpecification.
            new TestScenariosWithExpected
            {
                Sql = "Select * From (Select 1 As x) y",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 15, Details = "query is not flat (nested query scope)" },
                },
            },

            // Scalar subquery in WHERE clause.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where x = (Select Max(y) From dbo.s)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 31, Details = "query is not flat (nested query scope)" },
                },
            },

            // Scalar subquery in the SELECT list.
            new TestScenariosWithExpected
            {
                Sql = "Select x, (Select Max(y) From dbo.s) From dbo.t",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 11, Details = "query is not flat (nested query scope)" },
                },
            },

            // EXISTS predicate.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where Exists (Select 1 From dbo.s)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 34, Details = "query is not flat (nested query scope)" },
                },
            },

            // IN-subquery (as opposed to IN with a value list).
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where x In (Select y From dbo.s)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 32, Details = "query is not flat (nested query scope)" },
                },
            },

            // ANY / SOME quantified comparison.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t Where x = Any (Select y From dbo.s)",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 35, Details = "query is not flat (nested query scope)" },
                },
            },

            // CTE — note that the CTE body is visited BEFORE the outer SELECT, so the second
            // QuerySpecification encountered is the outer SELECT, at offset 28.
            new TestScenariosWithExpected
            {
                Sql = "With cte As (Select 1 As x) Select * From cte",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 13, Details = "query is not flat (nested query scope)" },
                },
            },

            // UNION.
            new TestScenariosWithExpected
            {
                Sql = "Select 1 As x Union Select 2 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 20, Details = "query is not flat (nested query scope)" },
                },
            },

            // UNION ALL.
            new TestScenariosWithExpected
            {
                Sql = "Select 1 As x Union All Select 2 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "query is not flat (nested query scope)" },
                },
            },

            // INTERSECT.
            new TestScenariosWithExpected
            {
                Sql = "Select 1 As x Intersect Select 1 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 24, Details = "query is not flat (nested query scope)" },
                },
            },

            // EXCEPT.
            new TestScenariosWithExpected
            {
                Sql = "Select 1 As x Except Select 2 As x",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 21, Details = "query is not flat (nested query scope)" },
                },
            },

            // CROSS APPLY with a subquery body.
            new TestScenariosWithExpected
            {
                Sql = "Select * From dbo.t a Cross Apply (Select Top 1 * From dbo.s) b",
                ExpectedViolations = new[]
                {
                    new ExpectedViolation { Offset = 35, Details = "query is not flat (nested query scope)" },
                },
            },
        };

        // Scripts that are a single QuerySpecification (or have none at all) — the rule passes.
        private static readonly IReadOnlyList<TestScenariosWithExpected> FlatQuerySqlScriptValidationRuleNoViolationScenarios = new[]
        {
            // Trivial flat SELECTs.
            new TestScenariosWithExpected { Sql = "Select 1" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.t Where x = 1" },

            // JOINs of every flavor — same query scope, just more FROM-clause structure.
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Left Join dbo.b On a.id = b.id" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Right Join dbo.b On a.id = b.id" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Full Outer Join dbo.b On a.id = b.id" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Cross Join dbo.b" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a Inner Join dbo.b On a.id = b.id Inner Join dbo.c On b.id = c.id" },
            new TestScenariosWithExpected { Sql = "Select * From dbo.a x Inner Join dbo.a y On x.id = y.id" },

            // Aggregation / filtering modifiers — all within one query scope.
            new TestScenariosWithExpected { Sql = "Select x, Count(*) From dbo.t Group By x" },
            new TestScenariosWithExpected { Sql = "Select x, Count(*) From dbo.t Group By x Having Count(*) > 1" },
            new TestScenariosWithExpected { Sql = "Select Top 10 * From dbo.t Order By x" },

            // Window function — OVER is a modifier, not a new scope.
            new TestScenariosWithExpected { Sql = "Select Row_Number() Over (Order By x) As rn From dbo.t" },

            // Scalar expressions — CASE/IIF/function calls don't create scopes.
            new TestScenariosWithExpected { Sql = "Select Case When x = 1 Then 'a' Else 'b' End From dbo.t" },

            // VALUES constructor as a table source — InlineDerivedTable, not a query.
            new TestScenariosWithExpected { Sql = "Select * From (Values (1), (2)) As v(x)" },

            // View reference — the rule does not crack open the view body, so this stays flat
            // regardless of what the view's definition contains.  (Useful escape hatch for
            // non-flattenable patterns like UNION.)
            new TestScenariosWithExpected { Sql = "Select * From dbo.my_view" },

            // Statements with no QuerySpecification at all (count stays at 0) — the rule has
            // nothing to fire on.  These would typically be caught by ReadOnlySelect; included
            // here to confirm FlatQuery alone is well-behaved on them.
            new TestScenariosWithExpected { Sql = "Update dbo.t Set x = 1" },
            new TestScenariosWithExpected { Sql = "Insert Into dbo.t (x) Values (1)" },
        };

        [Fact]
        public static void Execute___Should_return_violations___When_FlatQuerySqlScriptValidationRule_has_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = FlatQuerySqlScriptValidationRuleTestScenariosWithExpected;

            var rule = new FlatQuerySqlScriptValidationRule();

            // The evaluator carries per-script state (the QuerySpecification counter), so a
            // fresh ValidateSqlScriptProtocol instance is created for each scenario — matching
            // how the protocol is used in real callers.
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
        public static void Execute___Should_return_no_violations___When_FlatQuerySqlScriptValidationRule_has_not_been_violated()
        {
            // Arrange
            var testScenariosWithExpected = FlatQuerySqlScriptValidationRuleNoViolationScenarios;

            var rule = new FlatQuerySqlScriptValidationRule();

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
