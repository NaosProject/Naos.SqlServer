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
                        Name = "constructor should throw ArgumentException when parameter 'violations' contains a null element scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptValidationResult>();

                            var result = new SqlScriptValidationResult(
                                referenceObject.TargetSqlServerVersion,
                                new SqlScriptValidationRuleViolation[0].Concat(referenceObject.Violations).Concat(new SqlScriptValidationRuleViolation[] { null }).Concat(referenceObject.Violations).ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "violations", "contains at least one null element", },
                    });
        }

        [Fact]
        public static void HasAnyRuleViolation___Should_return_false___When_there_are_no_rule_violations()
        {
            // Arrange
            var systemUnderTest1 = new SqlScriptValidationResult(A.Dummy<SqlServerVersion>(), null);
            var systemUnderTest2 = new SqlScriptValidationResult(A.Dummy<SqlServerVersion>(), new SqlScriptValidationRuleViolation[0]);

            // Act
            var actual1 = systemUnderTest1.HasAnyRuleViolation();
            var actual2 = systemUnderTest2.HasAnyRuleViolation();

            // Assert
            actual1.AsTest().Must().BeFalse();
            actual2.AsTest().Must().BeFalse();
        }

        [Fact]
        public static void HasAnyRuleViolation___Should_return_true___When_there_are_some_rule_violations()
        {
            // Arrange
            var systemUnderTest = new SqlScriptValidationResult(A.Dummy<SqlServerVersion>(), Some.ReadOnlyDummies<SqlScriptValidationRuleViolation>().ToList());

            // Act
            var actual = systemUnderTest.HasAnyRuleViolation();

            // Assert
            actual.AsTest().Must().BeTrue();
        }
    }
}