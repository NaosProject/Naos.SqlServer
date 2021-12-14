// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputParameterDefinition{TValue}Test.cs" company="Naos Project">
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
    using OBeautifulCode.Equality.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class OutputParameterDefinitionTValueTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static OutputParameterDefinitionTValueTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<OutputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'name' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

                            var result = new OutputParameterDefinition<int?>(
                                                 null,
                                                 referenceObject.SqlDataType);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "name", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<OutputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is white space scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

                            var result = new OutputParameterDefinition<int?>(
                                                 Invariant($"  {Environment.NewLine}  "),
                                                 referenceObject.SqlDataType);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "white space", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<OutputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is not alphanumeric nor @ nor _",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

                            var result = new OutputParameterDefinition<int?>(
                                referenceObject.Name + "^",
                                referenceObject.SqlDataType);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "alphanumeric", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<OutputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'sqlDataType' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

                            var result = new OutputParameterDefinition<int?>(
                                                 referenceObject.Name,
                                                 null);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "sqlDataType", },
                    });

            EquatableTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new EquatableTestScenario<OutputParameterDefinition<int?>>
                    {
                        Name = "Default Code Generated Scenario",
                        ReferenceObject = ReferenceObjectForEquatableTestScenarios,
                        ObjectsThatAreEqualToButNotTheSameAsReferenceObject = new OutputParameterDefinition<int?>[]
                        {
                            new OutputParameterDefinition<int?>(
                                ReferenceObjectForEquatableTestScenarios.Name,
                                ReferenceObjectForEquatableTestScenarios.SqlDataType),
                        },
                        ObjectsThatAreNotEqualToReferenceObject = new OutputParameterDefinition<int?>[]
                        {
                            new OutputParameterDefinition<int?>(
                                A.Dummy<OutputParameterDefinition<int?>>().Whose(_ => !_.Name.IsEqualTo(ReferenceObjectForEquatableTestScenarios.Name)).Name,
                                ReferenceObjectForEquatableTestScenarios.SqlDataType),
                        },
                        ObjectsThatAreNotOfTheSameTypeAsReferenceObject = new object[]
                        {
                            A.Dummy<object>(),
                            A.Dummy<string>(),
                            A.Dummy<int>(),
                            A.Dummy<int?>(),
                            A.Dummy<Guid>(),
                            A.Dummy<InputParameterDefinition<int?>>(),
                        },
                    });
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_sqlDataType_is_not_compatible_with_TValue()
        {
            // Arrange
            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

            // Act
            var actual = Record.Exception(() => new OutputParameterDefinition<int?>(
                referenceObject.Name,
                A.Dummy<SqlDataTypeRepresentationBase>().Whose(_ => _.GetType() != typeof(IntSqlDataTypeRepresentation))));

            // Act, Assert
            actual.AsTest().Must().BeOfType<ArgumentException>();
            actual.Message.AsTest().Must().ContainString("The specified sqlDataType is not compatible with the specified dotNetDataType.  See inner exception.");
            actual.InnerException.AsTest().Must().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_name_contains_at_character_or_underscore_character()
        {
            // Arrange
            var referenceObject = A.Dummy<OutputParameterDefinition<int?>>();

            // Act
            var actual = Record.Exception(() => new OutputParameterDefinition<int?>(
                "@" + referenceObject.Name + "_",
                referenceObject.SqlDataType));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}