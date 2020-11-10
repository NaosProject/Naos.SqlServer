// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableRepresentationTest.cs" company="Naos Project">
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

    public static partial class TableRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static TableRepresentationTest()
        {
            ConstructorArgumentValidationTestScenarios
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<TableRepresentation>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'name' is 'non-alphanumeric' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<TableRepresentation>();

                                                   var result = new TableRepresentation(
                                                       "not-alpha*numeric",
                                                       referenceObject.ColumnNameToRepresentationMap);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "name",
                                                                   "alphanumeric",
                                                               },
                        });
        }
    }
}