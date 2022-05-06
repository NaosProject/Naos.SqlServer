// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntSqlDataTypeRepresentationTest.cs" company="Naos Project">
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

    using Xunit;

    public static partial class IntSqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static IntSqlDataTypeRepresentationTest()
        {
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<IntSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(decimal), default(decimal), false));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("Supported object types: int, int?; provided type: decimal");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual1 = Record.Exception(() => A.Dummy<IntSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int), A.Dummy<int>(), true));
            var actual2 = Record.Exception(() => A.Dummy<IntSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int?), A.Dummy<int?>(), true));

            // Act, Assert
            actual1.AsTest().Must().BeNull();
            actual2.AsTest().Must().BeNull();
        }
    }
}