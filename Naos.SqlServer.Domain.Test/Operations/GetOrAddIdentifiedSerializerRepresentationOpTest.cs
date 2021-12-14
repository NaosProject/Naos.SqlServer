// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetOrAddIdentifiedSerializerRepresentationOpTest.cs" company="Naos Project">
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
    public static partial class GetOrAddIdentifiedSerializerRepresentationOpTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static GetOrAddIdentifiedSerializerRepresentationOpTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<GetOrAddIdentifiedSerializerRepresentationOp>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'serializationFormat' is SerializationFormat.Invalid scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<GetOrAddIdentifiedSerializerRepresentationOp>();

                            var result = new GetOrAddIdentifiedSerializerRepresentationOp(
                                referenceObject.SpecifiedResourceLocator,
                                referenceObject.SerializerRepresentation,
                                SerializationFormat.Invalid);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "serializationFormat", "Invalid" },
                    });
        }
    }
}