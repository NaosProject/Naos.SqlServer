// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateSqlScriptProtocolTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Validation.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;

    public static partial class ValidateSqlScriptProtocolTest
    {
        private static readonly SqlServerVersion SqlServerVersion = SqlServerVersion.SqlServer2019;

        private static void MustBeEqualTo(
            this IReadOnlyList<SqlScriptValidationResult> actual,
            IReadOnlyList<TestScenariosWithExpected> testScenariosWithExpected)
        {
            var expectedCounts = testScenariosWithExpected.Select(_ => _.ExpectedViolations.Count).ToList();
            var expectedOffsets = testScenariosWithExpected.SelectMany(_ => _.ExpectedViolations.Select(v => v.Offset)).ToList();
            var expectedDetails = testScenariosWithExpected.SelectMany(_ => _.ExpectedViolations.Select(v => v.Details)).ToList();

            actual.Select(_ => _.ParsingErrors).AsTest().Must().Each().BeNull();
            actual.Select(_ => _.TargetSqlServerVersion).AsTest().Must().Each().BeEqualTo(SqlServerVersion);
            actual.Select(_ => _.RuleViolations?.Count ?? 0).ToList().AsTest().Must().BeEqualTo(expectedCounts);
            actual.SelectMany(_ => _.RuleViolations ?? Enumerable.Empty<SqlScriptValidationRuleViolation>()).Select(_ => _.Offset).ToList().AsTest().Must().BeEqualTo(expectedOffsets);
            actual.SelectMany(_ => _.RuleViolations ?? Enumerable.Empty<SqlScriptValidationRuleViolation>()).Select(_ => _.Details).ToList().AsTest().Must().BeEqualTo(expectedDetails);
        }

        private static void MustNotHaveAnyViolations(
            this IReadOnlyList<SqlScriptValidationResult> actual)
        {
            actual.Select(_ => _.ParsingErrors).AsTest().Must().Each().BeNull();
            actual.Select(_ => _.TargetSqlServerVersion).AsTest().Must().Each().BeEqualTo(SqlServerVersion);
            actual.Select(_ => _.RuleViolations).AsTest().Must().Each().BeNull();
        }

        private class TestScenariosWithExpected
        {
            public string Sql { get; set; }

            public IReadOnlyList<ExpectedViolation> ExpectedViolations { get; set; }
        }

        private class ExpectedViolation
        {
            public int Offset { get; set; }

            public string Details { get; set; }
        }
    }
}
