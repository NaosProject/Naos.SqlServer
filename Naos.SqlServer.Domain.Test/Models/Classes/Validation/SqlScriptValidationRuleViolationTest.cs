// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptValidationRuleViolationTest.cs" company="Naos Project">
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

    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class SqlScriptValidationRuleViolationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlScriptValidationRuleViolationTest()
        {
            StringRepresentationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new StringRepresentationTestScenario<SqlScriptValidationRuleViolation>
                    {
                        Name = "Default Code Generated Scenario",
                        SystemUnderTestExpectedStringRepresentationFunc = () =>
                        {
                            var systemUnderTest = new SqlScriptValidationRuleViolation(
                                new DisallowSystemSchemasSqlScriptValidationRule(),
                                45,
                                "some-details-here");

                            var result = new SystemUnderTestExpectedStringRepresentation<SqlScriptValidationRuleViolation>
                            {
                                SystemUnderTest = systemUnderTest,
                                ExpectedStringRepresentation = Invariant($"[DisallowSystemSchemasSqlScriptValidationRule] (45): some-details-here"),
                            };

                            return result;
                        },
                    });

            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlScriptValidationRuleViolation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'offset' is negative scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptValidationRuleViolation>();

                            var result = new SqlScriptValidationRuleViolation(
                                                 referenceObject.Rule,
                                                 A.Dummy<NegativeInteger>(),
                                                 referenceObject.Details);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "offset", },
                    });
        }
    }
}