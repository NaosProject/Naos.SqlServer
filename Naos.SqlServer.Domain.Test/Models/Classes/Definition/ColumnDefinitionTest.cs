// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDefinitionTest.cs" company="Naos Project">
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
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class ColumnDefinitionTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static ColumnDefinitionTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<ColumnDefinition>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is not alphanumeric nor _",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<ColumnDefinition>();

                            var result = new ColumnDefinition(
                                referenceObject.Name + "^",
                                referenceObject.SqlDataType);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "alphanumeric" },
                    });
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_name_contains_underscore_character()
        {
            // Arrange
            var referenceObject = A.Dummy<ColumnDefinition>();

            // Act
            var actual = Record.Exception(() => new ColumnDefinition(
                referenceObject.Name + "_",
                referenceObject.SqlDataType));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}