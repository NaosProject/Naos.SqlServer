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
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Equality.Recipes;
    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
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

            EquatableTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new EquatableTestScenario<RestoreSqlServerDatabaseDetails>
                    {
                        Name = "Default Code Generated Scenario",
                        ReferenceObject = ReferenceObjectForEquatableTestScenarios,
                        ObjectsThatAreEqualToButNotTheSameAsReferenceObject = new RestoreSqlServerDatabaseDetails[]
                        {
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                        },
                        ObjectsThatAreNotEqualToReferenceObject = new RestoreSqlServerDatabaseDetails[]
                        {
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.Credential.IsEqualTo(ReferenceObjectForEquatableTestScenarios.Credential)).Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.DataFilePath.IsEqualTo(ReferenceObjectForEquatableTestScenarios.DataFilePath)).DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.Device.IsEqualTo(ReferenceObjectForEquatableTestScenarios.Device)).Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.LogFilePath.IsEqualTo(ReferenceObjectForEquatableTestScenarios.LogFilePath)).LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.RecoveryOption.IsEqualTo(ReferenceObjectForEquatableTestScenarios.RecoveryOption)).RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.ReplaceOption.IsEqualTo(ReferenceObjectForEquatableTestScenarios.ReplaceOption)).ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.RestoreFrom.IsEqualTo(ReferenceObjectForEquatableTestScenarios.RestoreFrom)).RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    ReferenceObjectForEquatableTestScenarios.RestrictedUserOption),
                            new RestoreSqlServerDatabaseDetails(
                                    ReferenceObjectForEquatableTestScenarios.DataFilePath,
                                    ReferenceObjectForEquatableTestScenarios.LogFilePath,
                                    ReferenceObjectForEquatableTestScenarios.Device,
                                    ReferenceObjectForEquatableTestScenarios.RestoreFrom,
                                    ReferenceObjectForEquatableTestScenarios.Credential,
                                    ReferenceObjectForEquatableTestScenarios.ChecksumOption,
                                    ReferenceObjectForEquatableTestScenarios.ErrorHandling,
                                    ReferenceObjectForEquatableTestScenarios.RecoveryOption,
                                    ReferenceObjectForEquatableTestScenarios.ReplaceOption,
                                    A.Dummy<RestoreSqlServerDatabaseDetails>().Whose(_ => !_.RestrictedUserOption.IsEqualTo(ReferenceObjectForEquatableTestScenarios.RestrictedUserOption)).RestrictedUserOption),
                        },
                        ObjectsThatAreNotOfTheSameTypeAsReferenceObject = new object[]
                        {
                            A.Dummy<object>(),
                            A.Dummy<string>(),
                            A.Dummy<int>(),
                            A.Dummy<int?>(),
                            A.Dummy<Guid>(),
                        },
                    });
        }
    }
}