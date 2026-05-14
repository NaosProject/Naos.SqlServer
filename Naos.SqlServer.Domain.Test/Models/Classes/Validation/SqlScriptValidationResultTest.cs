// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationResultTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class SqlScriptValidationResultTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlScriptValidationResultTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlScriptValidationResult>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'targetSqlServerVersion' is SqlServerVersion.Unknown",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptValidationResult>();

                            var result = new SqlScriptValidationResult(
                                SqlServerVersion.Unknown,
                                referenceObject.ParsingErrors,
                                referenceObject.RuleViolations);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "targetSqlServerVersion", "Unknown", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlScriptValidationResult>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'parsingErrors' contains a null element scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptValidationResult>();

                            var result = new SqlScriptValidationResult(
                                referenceObject.TargetSqlServerVersion,
                                new SqlScriptParsingError[0].Concat(referenceObject.ParsingErrors).Concat(new SqlScriptParsingError[] { null }).Concat(referenceObject.ParsingErrors).ToList(),
                                referenceObject.RuleViolations);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "parsingErrors", "contains at least one null element", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlScriptValidationResult>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'ruleViolations' contains a null element scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptValidationResult>();

                            var result = new SqlScriptValidationResult(
                                referenceObject.TargetSqlServerVersion,
                                referenceObject.ParsingErrors,
                                new SqlScriptValidationRuleViolation[0].Concat(referenceObject.RuleViolations).Concat(new SqlScriptValidationRuleViolation[] { null }).Concat(referenceObject.RuleViolations).ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "ruleViolations", "contains at least one null element", },
                    });
        }

        [Fact]
        public static void IsValid___Should_return_false___When_there_are_some_parsing_errors()
        {
            // Arrange
            var systemUnderTest1 = new SqlScriptValidationResult(
                A.Dummy<SqlServerVersion>(),
                Some.ReadOnlyDummies<SqlScriptParsingError>().ToList(),
                null);

            var systemUnderTest2 = new SqlScriptValidationResult(
                A.Dummy<SqlServerVersion>(),
                Some.ReadOnlyDummies<SqlScriptParsingError>().ToList(),
                new SqlScriptValidationRuleViolation[0]);

            // Act
            var actual1 = systemUnderTest1.IsValid();
            var actual2 = systemUnderTest2.IsValid();

            // Assert
            actual1.AsTest().Must().BeFalse();
            actual2.AsTest().Must().BeFalse();
        }

        [Fact]
        public static void IsValid___Should_return_false___When_there_are_some_rule_violations()
        {
            // Arrange
            var systemUnderTest1 = new SqlScriptValidationResult(
                A.Dummy<SqlServerVersion>(),
                null,
                Some.ReadOnlyDummies<SqlScriptValidationRuleViolation>().ToList());

            var systemUnderTest2 = new SqlScriptValidationResult(
                A.Dummy<SqlServerVersion>(),
                new SqlScriptParsingError[0],
                Some.ReadOnlyDummies<SqlScriptValidationRuleViolation>().ToList());

            // Act
            var actual1 = systemUnderTest1.IsValid();
            var actual2 = systemUnderTest2.IsValid();

            // Assert
            actual1.AsTest().Must().BeFalse();
            actual2.AsTest().Must().BeFalse();
        }

        [Fact]
        public static void IsValid___Should_return_true___When_there_are_no_parsing_errors_and_no_rule_violations()
        {
            // Arrange
            var systemUnderTest = new[]
            {
                new SqlScriptValidationResult(
                    A.Dummy<SqlServerVersion>(),
                    null,
                    null),
                new SqlScriptValidationResult(
                    A.Dummy<SqlServerVersion>(),
                    new SqlScriptParsingError[0],
                    null),
                new SqlScriptValidationResult(
                    A.Dummy<SqlServerVersion>(),
                    null,
                    new SqlScriptValidationRuleViolation[0]),
                new SqlScriptValidationResult(
                    A.Dummy<SqlServerVersion>(),
                    new SqlScriptParsingError[0],
                    new SqlScriptValidationRuleViolation[0]),
            };

            // Act
            var actual = systemUnderTest.Select(_ => _.IsValid()).ToArray();

            // Assert
            actual.AsTest().Must().Each().BeTrue();
        }
    }
}