// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDescriptionTest.cs" company="Naos Project">
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

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class ColumnDescriptionTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static ColumnDescriptionTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<ColumnDescription>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'columnName' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<ColumnDescription>();

                                                   var result = new ColumnDescription(
                                                       null,
                                                       referenceObject.OrdinalPosition,
                                                       referenceObject.ColumnDefault,
                                                       referenceObject.IsNullable,
                                                       referenceObject.SqlDataType);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "columnName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<ColumnDescription>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'columnName' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<ColumnDescription>();

                                                   var result = new ColumnDescription(
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.OrdinalPosition,
                                                       referenceObject.ColumnDefault,
                                                       referenceObject.IsNullable,
                                                       referenceObject.SqlDataType);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "columnName",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<ColumnDescription>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'dataType' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<ColumnDescription>();

                                                   var result = new ColumnDescription(
                                                       referenceObject.ColumnName,
                                                       referenceObject.OrdinalPosition,
                                                       referenceObject.ColumnDefault,
                                                       referenceObject.IsNullable,
                                                       null);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataType",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<ColumnDescription>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataType' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<ColumnDescription>();

                                                   var result = new ColumnDescription(
                                                       referenceObject.ColumnName,
                                                       referenceObject.OrdinalPosition,
                                                       referenceObject.ColumnDefault,
                                                       referenceObject.IsNullable,
                                                       Invariant($"  {Environment.NewLine}  "));

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataType",
                                                                   "white space",
                                                               },
                        });
        }
    }
}