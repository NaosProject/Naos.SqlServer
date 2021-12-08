// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamBuilder.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Extensions to <see cref="SqlServerLocator"/>.
    /// </summary>
    public static class SqlStreamBuilder
    {
        /// <summary>
        /// Extension on <see cref="SqlStreamConfigObject"/> to build a <see cref="SqlStream"/>.
        /// </summary>
        /// <param name="streamConfigObject">The stream configuration object.</param>
        /// <param name="serializerFactory">The serializer factory.</param>
        /// <param name="configSerializerFactoryVersionMatchStrategy">Optional type version match strategy to use to confirm the <paramref name="serializerFactory"/>'s type matches the one specified <paramref name="streamConfigObject"/>; DEFAULT is Any.</param>
        /// <returns>A <see cref="SqlStream"/>.</returns>
        public static SqlStream ToStream(
            this SqlStreamConfigObject streamConfigObject,
            ISerializerFactory serializerFactory,
            VersionMatchStrategy configSerializerFactoryVersionMatchStrategy = VersionMatchStrategy.Any)
        {
            streamConfigObject.MustForArg(nameof(streamConfigObject)).NotBeNull();
            serializerFactory.MustForArg(nameof(serializerFactory)).NotBeNull();

            if (streamConfigObject.AllLocators.Count != 1)
            {
                throw new NotSupportedException(Invariant($"One single resource locators are currently supported and '{streamConfigObject.AllLocators.Count}' were provided."));
            }

            var serializerFactoryTypeRepresentation = serializerFactory.GetType().ToRepresentation();
            if (!serializerFactoryTypeRepresentation
                                  .EqualsAccordingToStrategy(streamConfigObject.SerializerFactoryTypeRepresentation, configSerializerFactoryVersionMatchStrategy))
            {
                throw new ArgumentException(Invariant($"The provided {nameof(serializerFactory)} ({serializerFactoryTypeRepresentation}) is not the same type as specified in the {nameof(streamConfigObject)} ({streamConfigObject.SerializerFactoryTypeRepresentation})."));
            }

            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(streamConfigObject.AllLocators.Single());

            var result = new SqlStream(
                streamConfigObject.Name,
                streamConfigObject.DefaultConnectionTimeout,
                streamConfigObject.DefaultCommandTimeout,
                streamConfigObject.DefaultSerializerRepresentation,
                streamConfigObject.DefaultSerializationFormat,
                serializerFactory,
                resourceLocatorProtocol);

            return result;
        }
    }
}
