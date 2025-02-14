// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlConversionToolTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain.Test.Logic
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using Xunit;

    public static class XmlConversionToolTest
    {
        [Fact]
        public static void GetTagsXmlString___Should_return_expected_string___When_tag_set_is_not_empty()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>("tag-name-1", "tag-value-1"),
                new NamedValue<string>("tag-name-2", "tag-value-2"),
            };

            var expected = "<Tags><Tag Key=\"tag-name-1\" Value=\"tag-value-1\"/><Tag Key=\"tag-name-2\" Value=\"tag-value-2\"/></Tags>";

            // Act
            var actual = tags.GetTagsXmlString();

            // Assert
            actual.AsTest().Must().BeEqualTo(expected);
        }
    }
}
