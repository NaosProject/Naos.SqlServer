// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerJobInformationTest.cs" company="Naos Project">
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
    public static partial class SqlServerJobInformationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlServerJobInformationTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerJobInformation>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'id' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerJobInformation>();

                                                   var result = new SqlServerJobInformation(
                                                       null,
                                                       referenceObject.JobName,
                                                       referenceObject.LatestStepName,
                                                       referenceObject.JobStatus,
                                                       referenceObject.LatestStepRunTime,
                                                       referenceObject.LatestStepRunDuration);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "id",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerJobInformation>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'id' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerJobInformation>();

                                                   var result = new SqlServerJobInformation(
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.JobName,
                                                       referenceObject.LatestStepName,
                                                       referenceObject.JobStatus,
                                                       referenceObject.LatestStepRunTime,
                                                       referenceObject.LatestStepRunDuration);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "id",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerJobInformation>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'jobName' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerJobInformation>();

                                                   var result = new SqlServerJobInformation(
                                                       referenceObject.Id,
                                                       null,
                                                       referenceObject.LatestStepName,
                                                       referenceObject.JobStatus,
                                                       referenceObject.LatestStepRunTime,
                                                       referenceObject.LatestStepRunDuration);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "jobName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<SqlServerJobInformation>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'jobName' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<SqlServerJobInformation>();

                                                   var result = new SqlServerJobInformation(
                                                       referenceObject.Id,
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.LatestStepName,
                                                       referenceObject.JobStatus,
                                                       referenceObject.LatestStepRunTime,
                                                       referenceObject.LatestStepRunDuration);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "jobName",
                                                                   "white space",
                                                               },
                        });
        }
    }
}