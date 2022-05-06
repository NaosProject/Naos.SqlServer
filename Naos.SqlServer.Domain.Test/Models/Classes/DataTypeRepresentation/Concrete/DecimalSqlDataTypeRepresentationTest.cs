// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalSqlDataTypeRepresentationTest.cs" company="Naos Project">
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

    public static partial class DecimalSqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static DecimalSqlDataTypeRepresentationTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<DecimalSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'precision' is 0",
                        ConstructionFunc = () =>
                        {
                            var result = new DecimalSqlDataTypeRepresentation(0);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "precision", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<DecimalSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'precision' is greater than 38",
                        ConstructionFunc = () =>
                        {
                            var result = new DecimalSqlDataTypeRepresentation(A.Dummy<byte>().ThatIs(_ => _ > 38));

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "precision", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<DecimalSqlDataTypeRepresentation>
                    {
                        Name = "constructor should throw ArgumentOutOfRangeException when parameter 'scale' is greater than parameter 'precision'",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<DecimalSqlDataTypeRepresentation>();

                            var result = new DecimalSqlDataTypeRepresentation(referenceObject.Precision, A.Dummy<byte>().ThatIs(_ => _ > referenceObject.Precision));

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentOutOfRangeException),
                        ExpectedExceptionMessageContains = new[] { "scale", },
                    });
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<DecimalSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int), default(int), false));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("Supported object types: decimal, decimal?; provided type: int");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual1 = Record.Exception(() => A.Dummy<DecimalSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(decimal), A.Dummy<decimal>(), true));
            var actual2 = Record.Exception(() => A.Dummy<DecimalSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(decimal?), A.Dummy<decimal?>(), true));

            // Act, Assert
            actual1.AsTest().Must().BeNull();
            actual2.AsTest().Must().BeNull();
        }
    }
}