// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteStoredProcedureOpTest.cs" company="Naos Project">
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
    using Xunit;
    using static System.FormattableString;

    public static partial class ExecuteStoredProcedureOpTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static ExecuteStoredProcedureOpTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<ExecuteStoredProcedureOp>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'name' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<ExecuteStoredProcedureOp>();

                            var result = new ExecuteStoredProcedureOp(
                                                 null,
                                                 referenceObject.Parameters);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "name", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<ExecuteStoredProcedureOp>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is white space scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<ExecuteStoredProcedureOp>();

                            var result = new ExecuteStoredProcedureOp(
                                                 Invariant($"  {Environment.NewLine}  "),
                                                 referenceObject.Parameters);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "white space", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<ExecuteStoredProcedureOp>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'parameters' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<ExecuteStoredProcedureOp>();

                            var result = new ExecuteStoredProcedureOp(
                                                 referenceObject.Name,
                                                 null);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "parameters", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<ExecuteStoredProcedureOp>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'parameters' contains a null element scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<ExecuteStoredProcedureOp>();

                            var result = new ExecuteStoredProcedureOp(
                                                 referenceObject.Name,
                                                 new ParameterDefinitionBase[0].Concat(referenceObject.Parameters).Concat(new ParameterDefinitionBase[] { null }).Concat(referenceObject.Parameters).ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "parameters", "contains at least one null element", },
                    });
        }
    }
}