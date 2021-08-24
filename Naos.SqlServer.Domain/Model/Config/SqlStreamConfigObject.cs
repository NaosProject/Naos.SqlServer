﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// <param name="serializerFactoryTypeRepresentation">Serializer factory type representation.</param>
        /// <param name="defaultConnectionTimeout">Default timeout to use when connection to SQL Server.</param>
        /// <param name="defaultCommandTimeout">Default timeout to use when running a command on SQL Server.</param>
        /// <param name="defaultSerializerRepresentation">Default <see cref="SerializerRepresentation"/> to use (used for identifier serialization).</param>
        /// <param name="defaultSerializationFormat">Default <see cref="SerializationFormat"/> to use.</param>
        /// <param name="allLocators">All <see cref="ISqlServerLocator"/>'s.</param>
        public SqlStreamConfigObject(
            string name,
            TypeRepresentation serializerFactoryTypeRepresentation,
            TimeSpan defaultConnectionTimeout,
            TimeSpan defaultCommandTimeout,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<ISqlServerLocator> allLocators)
        {
            this.Name = name;
            this.SerializerFactoryTypeRepresentation = serializerFactoryTypeRepresentation;
            this.DefaultConnectionTimeout = defaultConnectionTimeout;
            this.DefaultCommandTimeout = defaultCommandTimeout;
            this.DefaultSerializerRepresentation = defaultSerializerRepresentation;
            this.DefaultSerializationFormat = defaultSerializationFormat;
            this.AllLocators = allLocators;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the serializer factory type representation.
        /// </summary>
        /// <value>The serializer factory type representation.</value>
        public TypeRepresentation SerializerFactoryTypeRepresentation { get; private set; }

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

        /// <summary>
        /// Gets the default <see cref="SerializerRepresentation"/> (used for identifier serialization).
        /// </summary>
        /// <value>The default <see cref="SerializerRepresentation"/>.</value>
        public SerializerRepresentation DefaultSerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the default <see cref="SerializationFormat"/>.
        /// </summary>
        /// <value>The default <see cref="SerializationFormat"/>.</value>
        public SerializationFormat DefaultSerializationFormat { get; private set; }

        /// <summary>
        /// Gets all <see cref="ISqlServerLocator"/>'s.
        /// </summary>
        /// <value>All <see cref="ISqlServerLocator"/>'s.</value>
        public IReadOnlyCollection<ISqlServerLocator> AllLocators { get; private set; }
    }
}
