// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDescriptionTest.cs" company="Naos Project">
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
    public static partial class TableDescriptionTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static TableDescriptionTest()
        {
            ConstructorArgumentValidationTestScenarios
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<TableDescription>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'databaseName' is 'invalid' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<TableDescription>();

                                                   var result = new TableDescription(
                                                       "not-valid",
                                                       referenceObject.TableSchema,
                                                       referenceObject.TableName,
                                                       referenceObject.Columns);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "databaseName",
                                                                   "alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<TableDescription>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'tableSchema' is 'invalid' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<TableDescription>();

                                                   var result = new TableDescription(
                                                       referenceObject.DatabaseName,
                                                       "not-valid",
                                                       referenceObject.TableName,
                                                       referenceObject.Columns);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "tableSchema",
                                                                   "alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<TableDescription>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'tableName' is 'invalid' scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<TableDescription>();

                                                   var result = new TableDescription(
                                                       referenceObject.DatabaseName,
                                                       referenceObject.TableSchema,
                                                       "not-valid",
                                                       referenceObject.Columns);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "tableName",
                                                                   "alphanumeric",
                                                               },
                        });
        }
    }
}