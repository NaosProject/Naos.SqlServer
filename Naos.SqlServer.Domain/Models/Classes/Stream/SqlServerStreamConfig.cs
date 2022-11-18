// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerStreamConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Sql Server implementation of <see cref="StreamConfigBase" />.
    /// </summary>
    public partial class SqlServerStreamConfig : StreamConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerStreamConfig"/> class.
        /// </summary>
        /// <param name="name">Name of the stream.</param>
        /// <param name="accessKinds">Access the stream has.</param>
        /// <param name="defaultConnectionTimeout">Default timeout to use when connecting to SQL Server.</param>
        /// <param name="defaultCommandTimeout">Default timeout to use when running a command on SQL Server.</param>
        /// <param name="defaultSerializerRepresentation">Default <see cref="SerializerRepresentation"/> to use (used for identifier serialization).</param>
        /// <param name="defaultSerializationFormat">Default <see cref="SerializationFormat"/> to use.</param>
        /// <param name="allLocators">All <see cref="IResourceLocator"/>'s.</param>
        public SqlServerStreamConfig(
            string name,
            StreamAccessKinds accessKinds,
            TimeSpan defaultConnectionTimeout,
            TimeSpan defaultCommandTimeout,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<IResourceLocator> allLocators)
            : base(name, accessKinds, defaultSerializerRepresentation, defaultSerializationFormat, allLocators)
        {
            defaultConnectionTimeout.TotalMilliseconds.MustForArg(Invariant($"{nameof(defaultConnectionTimeout)}.{nameof(TimeSpan.TotalMilliseconds)}")).BeGreaterThanOrEqualTo(0d);
            defaultCommandTimeout.TotalMilliseconds.MustForArg(Invariant($"{nameof(defaultCommandTimeout)}.{nameof(TimeSpan.TotalMilliseconds)}")).BeGreaterThanOrEqualTo(0d);
            allLocators.ToList().ForEach(_ => _.MustForArg(nameof(allLocators) + "-item").BeOfType<SqlServerLocator>());

            this.DefaultConnectionTimeout = defaultConnectionTimeout;
            this.DefaultCommandTimeout = defaultCommandTimeout;
        }

        /// <summary>
        /// Gets the default connection timeout.
        /// </summary>
        /// <value>The default connection timeout.</value>
        public TimeSpan DefaultConnectionTimeout { get; private set; }

        /// <summary>
        /// Gets the default command timeout.
        /// </summary>
        /// <value>The default command timeout.</value>
        public TimeSpan DefaultCommandTimeout { get; private set; }
    }
}
