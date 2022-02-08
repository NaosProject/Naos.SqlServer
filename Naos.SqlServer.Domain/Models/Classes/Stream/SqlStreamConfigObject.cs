// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStreamConfigObject.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using static System.FormattableString;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Existing serializer with database ID.
    /// </summary>
    public partial class SqlStreamConfigObject : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStreamConfigObject"/> class.
        /// </summary>
        /// <param name="name">Name of the stream.</param>
        /// <param name="defaultConnectionTimeout">Default timeout to use when connecting to SQL Server.</param>
        /// <param name="defaultCommandTimeout">Default timeout to use when running a command on SQL Server.</param>
        /// <param name="defaultSerializerRepresentation">Default <see cref="SerializerRepresentation"/> to use (used for identifier serialization).</param>
        /// <param name="defaultSerializationFormat">Default <see cref="SerializationFormat"/> to use.</param>
        /// <param name="allLocators">All <see cref="ISqlServerLocator"/>'s.</param>
        public SqlStreamConfigObject(
            string name,
            TimeSpan defaultConnectionTimeout,
            TimeSpan defaultCommandTimeout,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<ISqlServerLocator> allLocators)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            defaultConnectionTimeout.TotalMilliseconds.MustForArg(Invariant($"{nameof(defaultConnectionTimeout)}.{nameof(TimeSpan.TotalMilliseconds)}")).BeGreaterThanOrEqualTo(0d);
            defaultCommandTimeout.TotalMilliseconds.MustForArg(Invariant($"{nameof(defaultCommandTimeout)}.{nameof(TimeSpan.TotalMilliseconds)}")).BeGreaterThanOrEqualTo(0d);
            defaultSerializerRepresentation.MustForArg(nameof(defaultSerializerRepresentation)).NotBeNull();
            defaultSerializationFormat.MustForArg(nameof(defaultSerializationFormat)).NotBeEqualTo(SerializationFormat.Invalid);
            allLocators.MustForArg(nameof(allLocators)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();

            this.Name = name;
            this.DefaultConnectionTimeout = defaultConnectionTimeout;
            this.DefaultCommandTimeout = defaultCommandTimeout;
            this.DefaultSerializerRepresentation = defaultSerializerRepresentation;
            this.DefaultSerializationFormat = defaultSerializationFormat;
            this.AllLocators = allLocators;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the default connection timeout.
        /// </summary>
        public TimeSpan DefaultConnectionTimeout { get; private set; }

        /// <summary>
        /// Gets the default command timeout.
        /// </summary>
        public TimeSpan DefaultCommandTimeout { get; private set; }

        /// <summary>
        /// Gets the default <see cref="SerializerRepresentation"/> (used for identifier serialization).
        /// </summary>
        public SerializerRepresentation DefaultSerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the default <see cref="SerializationFormat"/>.
        /// </summary>
        public SerializationFormat DefaultSerializationFormat { get; private set; }

        /// <summary>
        /// Gets all <see cref="ISqlServerLocator"/>'s.
        /// </summary>
        public IReadOnlyCollection<ISqlServerLocator> AllLocators { get; private set; }
    }
}
