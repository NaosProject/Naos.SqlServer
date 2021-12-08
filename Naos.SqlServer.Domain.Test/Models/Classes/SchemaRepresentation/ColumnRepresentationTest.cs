// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnRepresentationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy;

    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using Xunit;

    using static System.FormattableString;

    public static partial class ColumnRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static ColumnRepresentationTest()
        {
            ConstructorArgumentValidationTestScenarios
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<ColumnRepresentation>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'name' is not alpha-numeric scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<ColumnRepresentation>();

                                                   var result = new ColumnRepresentation(
                                                       "not-alpha-numeric",
                                                       referenceObject.SqlDataType);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "name",
                                                               },
                        });
        }
    }
}