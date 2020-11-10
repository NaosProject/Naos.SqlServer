﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerLocatorTest.cs" company="Naos Project">
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

    public static partial class SqlServerLocatorTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlServerLocatorTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerLocator>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'serverName' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerLocator>();

                                                   var result = new SqlServerLocator(
                                                       null,
                                                       referenceObject.DatabaseName,
                                                       referenceObject.UserName,
                                                       referenceObject.Password,
                                                       referenceObject.InstanceName,
                                                       referenceObject.Port);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "serverName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerLocator>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'serverName' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerLocator>();

                                                   var result = new SqlServerLocator(
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.DatabaseName,
                                                       referenceObject.UserName,
                                                       referenceObject.Password,
                                                       referenceObject.InstanceName,
                                                       referenceObject.Port);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "serverName",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerLocator>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'databaseName' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerLocator>();

                                                   var result = new SqlServerLocator(
                                                       referenceObject.ServerName,
                                                       null,
                                                       referenceObject.UserName,
                                                       referenceObject.Password,
                                                       referenceObject.InstanceName,
                                                       referenceObject.Port);

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
                        new ConstructorArgumentValidationTestScenario<SqlServerLocator>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'databaseName' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerLocator>();

                                                   var result = new SqlServerLocator(
                                                       referenceObject.ServerName,
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.UserName,
                                                       referenceObject.Password,
                                                       referenceObject.InstanceName,
                                                       referenceObject.Port);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "databaseName",
                                                                   "white space",
                                                               },
                        });
        }
    }
}