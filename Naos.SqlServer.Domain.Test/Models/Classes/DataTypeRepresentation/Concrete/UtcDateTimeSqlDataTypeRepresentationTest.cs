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
    using OBeautifulCode.CodeAnalysis.Recipes;
    using Xunit;

    using static System.FormattableString;

    public static partial class UtcDateTimeSqlDataTypeRepresentationTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static UtcDateTimeSqlDataTypeRepresentationTest()
        {
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_ArgumentException___When_objectValue_is_not_UTC()
        {
            // Arrange, Act
            var now = DateTime.UtcNow;
            var nonUtcDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Local);
            var actual = Record.Exception(() => new UtcDateTimeSqlDataTypeRepresentation().ValidateObjectTypeIsCompatible(typeof(DateTime), nonUtcDateTime, true));

            // Act, Assert
            actual.AsTest().Must().BeOfType<ArgumentException>();
            actual.Message.AsTest().Must().ContainString(Invariant($"Provided value (name: 'value') is of a Kind that is not DateTimeKind.Utc.  Kind is DateTimeKind.Local."));
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_throw_InvalidOperationException___When_objectType_is_not_compatible()
        {
            // Arrange, Act
            var actual = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(int), default(int), false));

            // Act, Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().ContainString("Supported object types: DateTime, DateTime?; provided type: int");
        }

        [Fact]
        public static void ValidateObjectTypeIsCompatible___Should_not_throw___When_objectType_is_compatible()
        {
            // Arrange, Act
            var actual1 = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(DateTime), A.Dummy<DateTime>(), true));
            var actual2 = Record.Exception(() => A.Dummy<UtcDateTimeSqlDataTypeRepresentation>().ValidateObjectTypeIsCompatible(typeof(DateTime?), A.Dummy<DateTime?>(), true));

            // Act, Assert
            actual1.AsTest().Must().BeNull();
            actual2.AsTest().Must().BeNull();
        }
    }
}