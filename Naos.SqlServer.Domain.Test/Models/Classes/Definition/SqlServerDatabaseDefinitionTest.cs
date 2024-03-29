﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerDatabaseDefinitionTest.cs" company="Naos Project">
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
    public static partial class SqlServerDatabaseDefinitionTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlServerDatabaseDefinitionTest()
        {
            ConstructorArgumentValidationTestScenarios
            .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'databaseName' is null scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    null,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "databaseName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'databaseName' is white space scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    Invariant($"  {Environment.NewLine}  "),
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "databaseName",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'databaseName' is 'invalid' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    "not-valid",
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

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
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataFileLogicalName' is 'invalid' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    "not-valid",
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataFileLogicalName",
                                                                   "alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid for FileInfo' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFilePath,
                                    "C:::::/not-possible-to-new-up-FileInfo-constructor",
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with single quote' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    "path-with-single-quote'",
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with double quote' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    "path-with-double-quote\"",
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with semicolon' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    "path-with-semicolon;",
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "dataFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'logFileLogicalName' is 'invalid' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFileLogicalName,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    "not-valid",
                                    referenceObject.LogFilePath,
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "logFileLogicalName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with FileInfo' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    "C:::::/not-possible-to-new-up-FileInfo-constructor",
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "logFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with single quote' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    "path-with-single-quote'",
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "logFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with double quote' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    "path-with-double-quote\"",
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "logFilePath",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerDatabaseDefinition>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with semicolon' when specified scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

                                var result = new SqlServerDatabaseDefinition(
                                    referenceObject.DatabaseName,
                                    referenceObject.DatabaseType,
                                    referenceObject.RecoveryMode,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFilePath,
                                    referenceObject.DataFileCurrentSizeInKb,
                                    referenceObject.DataFileMaxSizeInKb,
                                    referenceObject.DataFileGrowthSizeInKb,
                                    referenceObject.LogFileLogicalName,
                                    "path-with-semicolon;",
                                    referenceObject.LogFileCurrentSizeInKb,
                                    referenceObject.LogFileMaxSizeInKb,
                                    referenceObject.LogFileGrowthSizeInKb);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "logFilePath",
                                                               },
                        });
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_names_contain_allowed_non_alphanumeric_characters()
        {
            // Arrange
            var referenceObject = A.Dummy<SqlServerDatabaseDefinition>();

            // Act
            var actual = Record.Exception(() => new SqlServerDatabaseDefinition(
                referenceObject.DatabaseName + " _",
                referenceObject.DatabaseType,
                referenceObject.RecoveryMode,
                referenceObject.DataFileLogicalName + " _",
                referenceObject.DataFilePath,
                referenceObject.DataFileCurrentSizeInKb,
                referenceObject.DataFileMaxSizeInKb,
                referenceObject.DataFileGrowthSizeInKb,
                referenceObject.LogFileLogicalName + " _",
                referenceObject.LogFilePath,
                referenceObject.LogFileCurrentSizeInKb,
                referenceObject.LogFileMaxSizeInKb,
                referenceObject.LogFileGrowthSizeInKb));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}