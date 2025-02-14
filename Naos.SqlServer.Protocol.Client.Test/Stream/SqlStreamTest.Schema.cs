// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamTest.Schema.cs" company="Naos Project">
//     Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client.Test
{
    using System;
    using FakeItEasy;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using Xunit;
    using static System.FormattableString;

    public static partial class SqlStreamTest
    {
        private static readonly SqlStream SharedStream = BuildCreatedStream();

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
    }
}