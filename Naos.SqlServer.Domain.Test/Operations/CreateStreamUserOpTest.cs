// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateStreamUserOpTest.cs" company="Naos Project">
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
    using OBeautifulCode.Representation.System;
    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class CreateStreamUserOpTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static CreateStreamUserOpTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'protocolsToGrantAccessFor' contains unsupported protocols",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<CreateStreamUserOp>();

                            var result = new CreateStreamUserOp(
                                                 referenceObject.UserName,
                                                 referenceObject.ClearTextPassword,
                                                 Some.ReadOnlyDummies<TypeRepresentation>().ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "Unsupported access type provided; supported:", },
                    });
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_username_contains_dash_character()
        {
            // Arrange
            var referenceObject = A.Dummy<CreateStreamUserOp>();

            // Act
            var actual = Record.Exception(() => new CreateStreamUserOp(
                referenceObject.UserName + "-",
                referenceObject.ClearTextPassword,
                referenceObject.ProtocolsToGrantAccessFor));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}