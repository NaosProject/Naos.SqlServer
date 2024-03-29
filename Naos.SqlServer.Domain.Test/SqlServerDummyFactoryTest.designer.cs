﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.181.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test
{
    using global::System.CodeDom.Compiler;
    using global::System.Diagnostics.CodeAnalysis;

    using global::FakeItEasy;

    using global::OBeautifulCode.Assertion.Recipes;

    using global::Xunit;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("OBeautifulCode.CodeGen.ModelObject", "1.0.181.0")]
    public static partial class SqlServerDummyFactoryTest
    {
        [Fact]
        public static void SqlServerDummyFactory___Should_derive_from_DefaultSqlServerDummyFactory___When_reflecting()
        {
            // Arrange
            var dummyFactoryType = typeof(SqlServerDummyFactory);

            var defaultDummyFactoryType = typeof(DefaultSqlServerDummyFactory);

            // Act, Assert
            defaultDummyFactoryType.GetInterface(nameof(IDummyFactory)).AsTest().Must().NotBeNull();

            dummyFactoryType.BaseType.AsTest().Must().BeEqualTo(defaultDummyFactoryType);
        }
    }
}