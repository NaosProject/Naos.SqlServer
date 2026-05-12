// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaQualifiedTableNameTest.cs" company="Naos Project">
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

    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class SchemaQualifiedTableNameTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static SchemaQualifiedTableNameTest()
        {
            StringRepresentationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(() =>
                    new StringRepresentationTestScenario<SchemaQualifiedTableName>
                    {
                        Name = "Default Code Generated Scenario",
                        SystemUnderTestExpectedStringRepresentationFunc = () =>
                        {
                            var systemUnderTest = A.Dummy<SchemaQualifiedTableName>();

                            var result = new SystemUnderTestExpectedStringRepresentation<SchemaQualifiedTableName>
                            {
                                SystemUnderTest = systemUnderTest,
                                ExpectedStringRepresentation = Invariant($"{systemUnderTest.SchemaName}.{systemUnderTest.TableName}"),
                            };

                            return result;
                        },
                    });
        }
    }
}