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
        /// Extension on <see cref="SqlServerStreamConfig"/> to build a <see cref="SqlStream"/>.
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

            var resourceLocatorProtocol = new SingleResourceLocatorProtocols(streamConfig.AllLocators.Single());

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
