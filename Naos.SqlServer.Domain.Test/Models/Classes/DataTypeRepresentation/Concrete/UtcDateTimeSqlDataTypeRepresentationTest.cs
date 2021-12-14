// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtcDateTimeSqlDataTypeRepresentationTest.cs" company="Naos Project">
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

    public static partial class UtcDateTimeSqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static UtcDateTimeSqlDataTypeRepresentationTest()
        {
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int)));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("Supported object types: DateTime, DateTime?; provided type: int");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual1 = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(DateTime)));
            var actual2 = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(DateTime?)));

            // Act, Assert
            actual1.AsTest().Must().BeNull();
            actual2.AsTest().Must().BeNull();
        }
    }
}