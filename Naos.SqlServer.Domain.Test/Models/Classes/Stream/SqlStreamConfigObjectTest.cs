// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamConfigObjectTest.cs" company="Naos Project">
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
    using OBeautifulCode.Serialization;
    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class SqlStreamConfigObjectTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SqlStreamConfigObjectTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlStreamConfigObject>
                    {
                        Name = "constructor should throw ArgumentOutRangeException when parameter 'defaultConnectionTimeout' is negative",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlStreamConfigObject>();

                            var result = new SqlStreamConfigObject(
                                referenceObject.Name,
                                referenceObject.SerializerFactoryTypeRepresentation,
                                referenceObject.DefaultConnectionTimeout.Negate(),
                                referenceObject.DefaultCommandTimeout,
                                referenceObject.DefaultSerializerRepresentation,
                                referenceObject.DefaultSerializationFormat,
                                referenceObject.AllLocators);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "defaultConnectionTimeout.TotalMilliseconds" },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlStreamConfigObject>
                    {
                        Name = "constructor should throw ArgumentOutRangeException when parameter 'defaultConnectionTimeout' is negative",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlStreamConfigObject>();

                            var result = new SqlStreamConfigObject(
                                referenceObject.Name,
                                referenceObject.SerializerFactoryTypeRepresentation,
                                referenceObject.DefaultConnectionTimeout,
                                referenceObject.DefaultCommandTimeout.Negate(),
                                referenceObject.DefaultSerializerRepresentation,
                                referenceObject.DefaultSerializationFormat,
                                referenceObject.AllLocators);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "defaultCommandTimeout.TotalMilliseconds", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<SqlStreamConfigObject>
                    {
                        Name = "constructor should throw ArgumentOutRangeException when parameter 'defaultSerializationFormat' is SerializationFormat.Invalid",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<SqlStreamConfigObject>();

                            var result = new SqlStreamConfigObject(
                                referenceObject.Name,
                                referenceObject.SerializerFactoryTypeRepresentation,
                                referenceObject.DefaultConnectionTimeout,
                                referenceObject.DefaultCommandTimeout,
                                referenceObject.DefaultSerializerRepresentation,
                                SerializationFormat.Invalid,
                                referenceObject.AllLocators);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "defaultSerializationFormat", "Invalid", },
                    });
        }
    }
}