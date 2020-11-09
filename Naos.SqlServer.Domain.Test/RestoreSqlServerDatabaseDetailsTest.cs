// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreSqlServerDatabaseDetailsTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy;

    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using Xunit;

    using static System.FormattableString;

    public static partial class RestoreSqlServerDatabaseDetailsTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static RestoreSqlServerDatabaseDetailsTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid for FileInfo' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       "C:::::/not-possible-to-new-up-FileInfo-constructor",
                                                       referenceObject.LogFilePath,
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with single quote' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       "path-with-single-quote'",
                                                       referenceObject.LogFilePath,
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with double quote' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       "path-with-double-quote\"",
                                                       referenceObject.LogFilePath,
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'dataFilePath' is 'invalid with semicolon' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       "path-with-semicolon;",
                                                       referenceObject.LogFilePath,
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with FileInfo' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       referenceObject.DataFilePath,
                                                       "C:::::/not-possible-to-new-up-FileInfo-constructor",
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with single quote' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       referenceObject.DataFilePath,
                                                       "path-with-single-quote'",
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with double quote' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       referenceObject.DataFilePath,
                                                       "path-with-double-quote\"",
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'logFilePath' is 'invalid with semicolon' when specified scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                                   var result = new RestoreSqlServerDatabaseDetails(
                                                       referenceObject.DataFilePath,
                                                       "path-with-semicolon;",
                                                       referenceObject.Device,
                                                       referenceObject.RestoreFrom,
                                                       referenceObject.Credential,
                                                       referenceObject.ChecksumOption,
                                                       referenceObject.ErrorHandling,
                                                       referenceObject.RecoveryOption,
                                                       referenceObject.ReplaceOption,
                                                       referenceObject.RestrictedUserOption);

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
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'restoreFrom' is null scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                var result = new RestoreSqlServerDatabaseDetails(
                                    referenceObject.DataFilePath,
                                    referenceObject.LogFilePath,
                                    referenceObject.Device,
                                    null,
                                    referenceObject.Credential,
                                    referenceObject.ChecksumOption,
                                    referenceObject.ErrorHandling,
                                    referenceObject.RecoveryOption,
                                    referenceObject.ReplaceOption,
                                    referenceObject.RestrictedUserOption);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "restoreFrom",
                                                               },
                        })
                              .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'credential' is null and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                var result = new RestoreSqlServerDatabaseDetails(
                                    referenceObject.DataFilePath,
                                    referenceObject.LogFilePath,
                                    Device.Url,
                                    referenceObject.RestoreFrom,
                                    null,
                                    referenceObject.ChecksumOption,
                                    referenceObject.ErrorHandling,
                                    referenceObject.RecoveryOption,
                                    referenceObject.ReplaceOption,
                                    referenceObject.RestrictedUserOption);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'credential' is white space and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                var result = new RestoreSqlServerDatabaseDetails(
                                    referenceObject.DataFilePath,
                                    referenceObject.LogFilePath,
                                    Device.Url,
                                    referenceObject.RestoreFrom,
                                    Invariant($"   {Environment.NewLine}   "),
                                    referenceObject.ChecksumOption,
                                    referenceObject.ErrorHandling,
                                    referenceObject.RecoveryOption,
                                    referenceObject.ReplaceOption,
                                    referenceObject.RestrictedUserOption);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'credential' is not-alphanumeric and Device is 'Url' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                var result = new RestoreSqlServerDatabaseDetails(
                                    referenceObject.DataFilePath,
                                    referenceObject.LogFilePath,
                                    Device.Url,
                                    referenceObject.RestoreFrom,
                                    "not-valid-path",
                                    referenceObject.ChecksumOption,
                                    referenceObject.ErrorHandling,
                                    referenceObject.RecoveryOption,
                                    referenceObject.ReplaceOption,
                                    referenceObject.RestrictedUserOption);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "credential",
                                                                   "alphanumeric",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<RestoreSqlServerDatabaseDetails>
                        {
                            Name =
                                "constructor should throw ArgumentException when parameter 'errorHandling' is 'None' and ChecksumOption is 'Checksum' scenario",
                            ConstructionFunc = () =>
                            {
                                var referenceObject = A.Dummy<RestoreSqlServerDatabaseDetails>();

                                var result = new RestoreSqlServerDatabaseDetails(
                                    referenceObject.DataFilePath,
                                    referenceObject.LogFilePath,
                                    referenceObject.Device,
                                    referenceObject.RestoreFrom,
                                    referenceObject.Credential,
                                    ChecksumOption.Checksum,
                                    ErrorHandling.None,
                                    referenceObject.RecoveryOption,
                                    referenceObject.ReplaceOption,
                                    referenceObject.RestrictedUserOption);

                                return result;
                            },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "ErrorHandling cannot be None when using checksum.",
                                                               },
                        });
        }
    }
}