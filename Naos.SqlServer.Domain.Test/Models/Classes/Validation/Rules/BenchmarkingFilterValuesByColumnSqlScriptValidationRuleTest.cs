// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BenchmarkingFilterValuesByColumnSqlScriptValidationRuleTest.cs" company="Naos Project">
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
    public static partial class BenchmarkingFilterValuesByColumnSqlScriptValidationRuleTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static BenchmarkingFilterValuesByColumnSqlScriptValidationRuleTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<BenchmarkingFilterValuesByColumnSqlScriptValidationRule>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'minimumDistinctPeerValues' is 0",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<BenchmarkingFilterValuesByColumnSqlScriptValidationRule>();

                            var result = new BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
                                referenceObject.Column,
                                referenceObject.OwnedValues,
                                0,
                                referenceObject.RequireFilterOnConstrainedColumn,
                                referenceObject.Id);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "minimumDistinctPeerValues", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<BenchmarkingFilterValuesByColumnSqlScriptValidationRule>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'minimumDistinctPeerValues' is negative",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<BenchmarkingFilterValuesByColumnSqlScriptValidationRule>();

                            var result = new BenchmarkingFilterValuesByColumnSqlScriptValidationRule(
                                referenceObject.Column,
                                referenceObject.OwnedValues,
                                A.Dummy<NegativeInteger>(),
                                referenceObject.RequireFilterOnConstrainedColumn,
                                referenceObject.Id);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "minimumDistinctPeerValues", },
                    });
        }
    }
}