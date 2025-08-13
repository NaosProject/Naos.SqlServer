// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerStandardStreamTest.Schema.cs" company="Naos Project">
//     Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client.Test
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using Xunit;
    using static System.FormattableString;

    public static partial class SqlServerStandardStreamTest
    {
        private static readonly SqlServerStandardStream SharedStream = BuildCreatedStream();

        [Fact]
        public static void PutWithId___Should_throw_InvalidOperationException___When_tag_key_is_too_long()
        {
            // Arrange

            var tags = new[]
            {
                new NamedValue<string>(new string('a', StreamSchema.Tables.Tag.TagKeyMaxLength + 1), A.Dummy<string>()),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.PutWithId(A.Dummy<string>(), A.Dummy<MyObject>(), tags));

            // Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().BeEqualTo(Invariant($"Tag Name exceeds the maximum allowed length of {StreamSchema.Tables.Tag.TagKeyMaxLength}."));
        }

        [Fact]
        public static void PutWithId___Should_throw_InvalidOperationException___When_tag_value_is_too_long()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>(A.Dummy<string>(), new string('a', StreamSchema.Tables.Tag.TagValueMaxLength + 1)),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.PutWithId(A.Dummy<string>(), A.Dummy<MyObject>(), tags));

            // Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().BeEqualTo(Invariant($"Tag Value exceeds the maximum allowed length of {StreamSchema.Tables.Tag.TagValueMaxLength}."));
        }

        [Fact]
        public static void GetLatestObjectById___Should_throw_InvalidOperationException___When_tag_key_is_too_long()
        {
            // Arrange

            var tags = new[]
            {
                new NamedValue<string>(new string('a', StreamSchema.Tables.Tag.TagKeyMaxLength + 1), A.Dummy<string>()),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.GetLatestObjectById<string, MyObject>(A.Dummy<string>(), tagsToMatch: tags));

            // Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().BeEqualTo(Invariant($"Tag Name exceeds the maximum allowed length of {StreamSchema.Tables.Tag.TagKeyMaxLength}."));
        }

        [Fact]
        public static void GetLatestObjectById___Should_throw_InvalidOperationException___When_tag_value_is_too_long()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>(A.Dummy<string>(), new string('a', StreamSchema.Tables.Tag.TagValueMaxLength + 1)),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.GetLatestObjectById<string, MyObject>(A.Dummy<string>(), tagsToMatch: tags));

            // Assert
            actual.AsTest().Must().BeOfType<InvalidOperationException>();
            actual.Message.AsTest().Must().BeEqualTo(Invariant($"Tag Value exceeds the maximum allowed length of {StreamSchema.Tables.Tag.TagValueMaxLength}."));
        }

        [Fact]
        public static void PutWithId___Should_not_throw___When_tag_key_is_max_length()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>(new string('a', StreamSchema.Tables.Tag.TagKeyMaxLength), A.Dummy<string>()),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.PutWithId(A.Dummy<string>(), A.Dummy<MyObject>(), tags));

            // Assert
            actual.AsTest().Must().BeNull();
        }

        [Fact]
        public static void PutWithId___Should_not_throw___When_tag_value_is_max_length()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>(A.Dummy<string>(), new string('a', StreamSchema.Tables.Tag.TagValueMaxLength)),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.PutWithId(A.Dummy<string>(), A.Dummy<MyObject>(), tags));

            // Assert
            actual.AsTest().Must().BeNull();
        }

        [Fact]
        public static void GetLatestObjectById___Should_not_throw___When_tag_key_is_max_length()
        {
            // Arrange

            var tags = new[]
            {
                new NamedValue<string>(new string('a', StreamSchema.Tables.Tag.TagKeyMaxLength), A.Dummy<string>()),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.GetLatestObjectById<string, MyObject>(A.Dummy<string>(), tagsToMatch: tags));

            // Assert
            actual.AsTest().Must().BeNull();
        }

        [Fact]
        public static void GetLatestObjectById___Should_not_throw___When_tag_value_is_max_length()
        {
            // Arrange
            var tags = new[]
            {
                new NamedValue<string>(A.Dummy<string>(), new string('a', StreamSchema.Tables.Tag.TagValueMaxLength)),
            };

            // Act
            var actual = Record.Exception(() => SharedStream.GetLatestObjectById<string, MyObject>(A.Dummy<string>(), tagsToMatch: tags));

            // Assert
            actual.AsTest().Must().BeNull();
        }

        [Fact]
        public static void GetLatestObjectById___Should_return_expected_object____When_matching_on_tag_within_a_large_set_of_tags()
        {
            // Arrange
            var tags = Some.ReadOnlyDummies<Guid>(1000)
                .Select(_ => new NamedValue<string>("some-tag-key", _.ToStringInvariantPreferred()))
                .ToList();

            var expected = new MyObject(A.Dummy<string>(), A.Dummy<string>());

            SharedStream.PutWithId("some-id", expected, tags);

            // Act
            var actual = SharedStream.GetLatestObjectById<string, MyObject>("some-id", tagsToMatch: new[] { tags[500] });

            // Assert
            actual.Id.AsTest().Must().BeEqualTo(expected.Id);
            actual.Field.AsTest().Must().BeEqualTo(expected.Field);
        }
    }
}