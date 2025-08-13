// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamBuilder.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Extension methods on <see cref="SqlServerStreamConfig"/>.
    /// </summary>
    public static class SqlStreamBuilder
    {
        /// <summary>
        /// Builds a <see cref="SqlServerStreamConfig"/> from config.
        /// </summary>
        /// <param name="streamConfig">The stream configuration object.</param>
        /// <param name="serializerFactory">The serializer factory.</param>
        /// <returns>A <see cref="SqlStream"/>.</returns>
        public static SqlStream ToStream(
            this SqlServerStreamConfig streamConfig,
            ISerializerFactory serializerFactory)
        {
            streamConfig.MustForArg(nameof(streamConfig)).NotBeNull();
            serializerFactory.MustForArg(nameof(serializerFactory)).NotBeNull();

            if (streamConfig.AllLocators.Count != 1)
            {
                throw new NotSupportedException(Invariant($"One single resource locators are currently supported and '{streamConfig.AllLocators.Count}' were provided."));
            }

            var singleLocator = streamConfig.AllLocators.Single();

            if (!(singleLocator is SqlServerLocator))
            {
                throw new NotSupportedException(Invariant($"Locator is expected to be of type '{typeof(SqlServerLocator).ToStringReadable()}' but found locator of type '{singleLocator.GetType().ToStringReadable()}'."));
            }

            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(singleLocator);

            var result = new SqlStream(
                streamConfig.Name,
                streamConfig.DefaultConnectionTimeout,
                streamConfig.DefaultCommandTimeout,
                streamConfig.DefaultSerializerRepresentation,
                streamConfig.DefaultSerializationFormat,
                serializerFactory,
                resourceLocatorProtocol);

            return result;
        }
    }
}
