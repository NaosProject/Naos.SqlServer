// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureExecutionResultTest.cs" company="Naos Project">
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

    public static partial class StoredProcedureExecutionResultTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static StoredProcedureExecutionResultTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StoredProcedureExecutionResult>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'operation' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<StoredProcedureExecutionResult>();

                            var result = new StoredProcedureExecutionResult(
                                                 null,
                                                 referenceObject.OutputParameters);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "operation", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StoredProcedureExecutionResult>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'outputParameters' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<StoredProcedureExecutionResult>();

                            var result = new StoredProcedureExecutionResult(
                                                 referenceObject.Operation,
                                                 null);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "outputParameters", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StoredProcedureExecutionResult>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'outputParameters' contains a key-value pair with a null value scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<StoredProcedureExecutionResult>();

                            var dictionaryWithNullValue = referenceObject.OutputParameters.ToDictionary(_ => _.Key, _ => _.Value);

                            var randomKey = dictionaryWithNullValue.Keys.ElementAt(ThreadSafeRandom.Next(0, dictionaryWithNullValue.Count));

                            dictionaryWithNullValue[randomKey] = null;

                            var result = new StoredProcedureExecutionResult(
                                                 referenceObject.Operation,
                                                 dictionaryWithNullValue);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "outputParameters", "contains at least one key-value pair with a null value", },
                    });
        }
    }
}