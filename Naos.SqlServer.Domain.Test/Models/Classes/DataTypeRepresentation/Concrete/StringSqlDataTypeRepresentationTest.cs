// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringSqlDataTypeRepresentationTest.cs" company="Naos Project">
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

    using static System.FormattableString;

    public static partial class StringSqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static StringSqlDataTypeRepresentationTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StringSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportedLength' is 0",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<StringSqlDataTypeRepresentation>();

                            var result = new StringSqlDataTypeRepresentation(referenceObject.SupportUnicode, 0);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StringSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportedLength' is negative",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<StringSqlDataTypeRepresentation>();

                            var result = new StringSqlDataTypeRepresentation(referenceObject.SupportUnicode, A.Dummy<NegativeInteger>());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StringSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportUnicode' is false and 'supportedLength' is greater than MaxNonUnicodeLengthConstant",
                        ConstructionFunc = () =>
                        {
                            var result = new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant + 1);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<StringSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'supportUnicode' is true and 'supportedLength' is greater than MaxUnicodeLengthConstant",
                        ConstructionFunc = () =>
                        {
                            var result = new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant + 1);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "supportedLength", },
                    });
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_ArgumentException___When_objectValue_is_too_large()
        {
            // Arrange, Act
            var max = 10;
            var actual = Record.Exception(() => new StringSqlDataTypeRepresentation(true, max).ValidateObjectTypeIsCompatible(typeof(string), new string('a', max + 1), true));

            // Act, Assert
            actual.AsTest().Must().BeOfType<ArgumentException>();
            actual.Message.AsTest().Must().ContainString(Invariant($"Provided value has length {max + 1} exceeds maximum allowed value of {max}."));
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<StringSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int), default(int), false));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("String data can only be used for strings and enums, objectType int is not supported");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual1 = Record.Exception(() => A.Dummy<StringSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(string), A.Dummy<string>(), true));
            var actual2 = Record.Exception(() => A.Dummy<StringSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(Cipher), A.Dummy<Cipher>(), true));

            // Act, Assert
            actual1.AsTest().Must().BeNull();
            actual2.AsTest().Must().BeNull();
        }
    }
}