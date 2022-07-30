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
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1505:AvoidUnmaintainableCode",
            Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static CreateStreamUserOpTest()
        {
            ConstructorArgumentValidationTestScenarios
               .RemoveAllScenarios()
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'loginName' is null scenario and 'shouldCreateLogin' is false",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       null,
                                                       referenceObject.UserName,
                                                       referenceObject.ClearTextPassword,
                                                       referenceObject.StreamAccessKinds,
                                                       false);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "loginName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'loginName' is white space scenario and 'shouldCreateLogin' is false",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.UserName,
                                                       referenceObject.ClearTextPassword,
                                                       referenceObject.StreamAccessKinds,
                                                       false);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "loginName",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'userName' is null scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       referenceObject.LoginName,
                                                       null,
                                                       referenceObject.ClearTextPassword,
                                                       referenceObject.StreamAccessKinds,
                                                       referenceObject.ShouldCreateLogin);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "userName",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'userName' is white space scenario",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       referenceObject.LoginName,
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.ClearTextPassword,
                                                       referenceObject.StreamAccessKinds,
                                                       referenceObject.ShouldCreateLogin);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "userName",
                                                                   "white space",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentNullException when parameter 'clearTextPassword' is null scenario when 'shouldCreateLogin' is true",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       referenceObject.LoginName,
                                                       referenceObject.UserName,
                                                       null,
                                                       referenceObject.StreamAccessKinds,
                                                       true);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentNullException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "clearTextPassword",
                                                               },
                        })
               .AddScenario(
                    () =>
                        new ConstructorArgumentValidationTestScenario<CreateStreamUserOp>
                        {
                            Name = "constructor should throw ArgumentException when parameter 'clearTextPassword' is white space scenario when 'shouldCreateLogin' is true",
                            ConstructionFunc = () =>
                                               {
                                                   var referenceObject = A.Dummy<CreateStreamUserOp>();

                                                   var result = new CreateStreamUserOp(
                                                       referenceObject.LoginName,
                                                       referenceObject.UserName,
                                                       Invariant($"  {Environment.NewLine}  "),
                                                       referenceObject.StreamAccessKinds,
                                                       true);

                                                   return result;
                                               },
                            ExpectedExceptionType = typeof(ArgumentException),
                            ExpectedExceptionMessageContains = new[]
                                                               {
                                                                   "clearTextPassword",
                                                                   "white space",
                                                               },
                        });
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_username_contains_dash_character()
        {
            // Arrange
            var referenceObject = A.Dummy<CreateStreamUserOp>();

            // Act
            var actual = Record.Exception(
                () => new CreateStreamUserOp(
                    referenceObject.LoginName,
                    referenceObject.UserName + "-",
                    referenceObject.ClearTextPassword,
                    referenceObject.StreamAccessKinds,
                    referenceObject.ShouldCreateLogin));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}