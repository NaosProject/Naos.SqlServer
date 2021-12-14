// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDefinitionTest.cs" company="Naos Project">
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
    public static partial class TableDefinitionTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static TableDefinitionTest()
        {
            ConstructorArgumentValidationTestScenarios
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<TableDefinition>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'name' is not alphanumeric nor space or underscore",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<TableDefinition>();

                            var result = new TableDefinition(
                                referenceObject.Name + "^",
                                referenceObject.Columns);

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "name", "alphanumeric" },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<TableDefinition>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'columns' contains case-insensitive duplicate names",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<TableDefinition>();

                            var result = new TableDefinition(
                                referenceObject.Name,
                                referenceObject.Columns.Concat(referenceObject.Columns.Select(_ => _.DeepCloneWithName(_.Name.ToUpperInvariant()))).ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "case-insensitive column names" },
                    })
                .AddScenario(() =>
                    new ConstructorArgumentValidationTestScenario<TableDefinition>
                    {
                        Name = "constructor should throw ArgumentException when parameter 'columns' contains case-insensitive duplicate names",
                        ConstructionFunc = () =>
                        {
                            var referenceObject = A.Dummy<TableDefinition>();

                            var result = new TableDefinition(
                                referenceObject.Name,
                                referenceObject.Columns.Concat(referenceObject.Columns.Select(_ => _.DeepCloneWithName(_.Name.ToLowerInvariant()))).ToList());

                            return result;
                        },
                        ExpectedExceptionType = typeof(ArgumentException),
                        ExpectedExceptionMessageContains = new[] { "case-insensitive column names" },
                    });
        }

        [Fact]
        public static void Constructor___Should_not_throw___When_parameter_name_contains_space_or_underscore_characters()
        {
            // Arrange
            var referenceObject = A.Dummy<TableDefinition>();

            // Act
            var actual = Record.Exception(() => new TableDefinition(
                referenceObject.Name + " _",
                referenceObject.Columns));

            // Act, Assert
            actual.AsTest().Must().BeNull();
        }
    }
}