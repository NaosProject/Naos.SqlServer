// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySqlDataTypeRepresentationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using Xunit;

    public static partial class BinarySqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static BinarySqlDataTypeRepresentationTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<BinarySqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportedLength' is 0",
                        ConstructionFunc = () =>
                        {
                            var result = new BinarySqlDataTypeRepresentation(0);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<BinarySqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportedLength' is negative",
                        ConstructionFunc = () =>
                        {
                            var result = new BinarySqlDataTypeRepresentation(A.Dummy<NegativeInteger>());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<BinarySqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportedLength' is > MaxLengthConstant",
                        ConstructionFunc = () =>
                        {
                            var result = new BinarySqlDataTypeRepresentation(BinarySqlDataTypeRepresentation.MaxLengthConstant + 1);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    });
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<BinarySqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int)));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("Supported object types: byte[]; provided type: int");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<BinarySqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(byte[])));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}