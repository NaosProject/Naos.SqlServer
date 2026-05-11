// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlScriptParsingErrorTest.cs" company="Naos Project">
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
    public static partial class SqlScriptParsingErrorTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlScriptParsingErrorTest()
        {
            StringRepresentationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new StringRepresentationTestScenario<SqlScriptParsingError>
                    {
                        Name = "Default Code Generated Scenario",
                        SystemUnderTestExpectedStringRepresentationFunc = () =>
                        {
                            var systemUnderTest = new SqlScriptParsingError(
                                45,
                                "some-details-here");

                            var result = new SystemUnderTestExpectedStringRepresentation<SqlScriptParsingError>
                            {
                                SystemUnderTest = systemUnderTest,
                                ExpectedStringRepresentation = Invariant($"[45]: some-details-here"),
                            };

                            return result;
                        },
                    });

            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlScriptParsingError>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'offset' is negative scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlScriptParsingError>();

                            var result = new SqlScriptParsingError(
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