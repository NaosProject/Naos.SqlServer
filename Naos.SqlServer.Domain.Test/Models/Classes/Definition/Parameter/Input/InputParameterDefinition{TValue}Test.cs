// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputParameterDefinition{TValue}Test.cs" company="Naos Project">
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
    public static partial class InputParameterDefinitionTValueTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static InputParameterDefinitionTValueTest()
        {
            ConstructorArgumentValidationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<InputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'name' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

                            var result = new InputParameterDefinition<int?>(
                                                 null,
                                                 referenceObject.SqlDataType,
                                                 referenceObject.Value);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "name", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<InputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is white space scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

                            var result = new InputParameterDefinition<int?>(
                                                 Invariant($"  {Environment.NewLine}  "),
                                                 referenceObject.SqlDataType,
                                                 referenceObject.Value);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "white space", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<InputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is not alphanumeric nor @ nor _",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

                            var result = new InputParameterDefinition<int?>(
                                referenceObject.Name + "^",
                                referenceObject.SqlDataType,
                                referenceObject.Value);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "alphanumeric", },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<InputParameterDefinition<int?>>
                    {
                        Name = "constructor should throw ArgumentNullException when parameter 'sqlDataType' is null scenario",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

                            var result = new InputParameterDefinition<int?>(
                                                 referenceObject.Name,
                                                 null,
                                                 referenceObject.Value);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentNullException),
                        ExpectedExceptionMessageContains = new[] { "sqlDataType", },
                    });

            EquatableTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new EquatableTestScenario<InputParameterDefinition<int?>>
                    {
                        Name = "Default Code Generated Scenario",
                        ReferenceObject = ReferenceObjectForEquatableTestScenarios,
                        ObjectsThatAreEqualToButNotTheSameAsReferenceObject = new InputParameterDefinition<int?>[]
                        {
                            new InputParameterDefinition<int?>(
                                    ReferenceObjectForEquatableTestScenarios.Name,
                                    ReferenceObjectForEquatableTestScenarios.SqlDataType,
                                    ReferenceObjectForEquatableTestScenarios.Value),
                        },
                        ObjectsThatAreNotEqualToReferenceObject = new InputParameterDefinition<int?>[]
                        {
                            new InputParameterDefinition<int?>(
                                    A.Dummy<InputParameterDefinition<int?>>().Whose(_ => !_.Name.IsEqualTo(ReferenceObjectForEquatableTestScenarios.Name)).Name,
                                    ReferenceObjectForEquatableTestScenarios.SqlDataType,
                                    ReferenceObjectForEquatableTestScenarios.Value),
                            new InputParameterDefinition<int?>(
                                    ReferenceObjectForEquatableTestScenarios.Name,
                                    ReferenceObjectForEquatableTestScenarios.SqlDataType,
                                    A.Dummy<InputParameterDefinition<int?>>().Whose(_ => !_.Value.IsEqualTo(ReferenceObjectForEquatableTestScenarios.Value)).Value),
                        },
                        ObjectsThatAreNotOfTheSameTypeAsReferenceObject = new object[]
                        {
                            A.Dummy<object>(),
                            A.Dummy<string>(),
                            A.Dummy<int>(),
                            A.Dummy<int?>(),
                            A.Dummy<Guid>(),
                            A.Dummy<OutputParameterDefinition<int?>>(),
                        },
                    });
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_sqlDataType_is_not_compatible_with_TValue()
        {
            // Arrange
            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

            // Act
            var actual = Record.Exception(() => new InputParameterDefinition<int?>(
                referenceObject.Name,
                A.Dummy<SqlDataTypeRepresentationBase>().Whose(_ => _.GetType() != typeof(IntSqlDataTypeRepresentation)),
                referenceObject.Value));

            // Act, Assert
            actual.AsTest().Must().BeOfType<ArgumentException>();
            actual.Message.AsTest().Must().ContainString("The specified sqlDataType is not compatible with the specified dotNetDataType.  See inner exception.");
            actual.InnerException.AsTest().Must().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_name_contains_at_character_or_underscore_character()
        {
            // Arrange
            var referenceObject = A.Dummy<InputParameterDefinition<int?>>();

            // Act
            var actual = Record.Exception(() => new InputParameterDefinition<int?>(
                "@" + referenceObject.Name + "_",
                referenceObject.SqlDataType,
                referenceObject.Value));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}