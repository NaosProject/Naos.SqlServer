// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStreamStoredProceduresOpTest.cs" company="Naos Project">
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
    public static partial class UpdateStreamStoredProceduresOpTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static UpdateStreamStoredProceduresOpTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<UpdateStreamStoredProceduresOp>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'maxConcurrentHandlingCount' is 0 scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<UpdateStreamStoredProceduresOp>();

                            var result = new UpdateStreamStoredProceduresOp(
                                referenceObject.RecordTagAssociationManagementStrategy,
                                0);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "maxConcurrentHandlingCount" },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<UpdateStreamStoredProceduresOp>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'maxConcurrentHandlingCount' is negative scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<UpdateStreamStoredProceduresOp>();

                            var result = new UpdateStreamStoredProceduresOp(
                                referenceObject.RecordTagAssociationManagementStrategy,
                                A.Dummy<NegativeInteger>());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "maxConcurrentHandlingCount" },
                    });
        }
    }
}