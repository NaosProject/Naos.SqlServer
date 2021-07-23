// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetIdAddIfNecessarySerializerRepresentationOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using Naos.Database.Domain;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;

    /// <summary>
    /// Find the identity of a <see cref="SerializerRepresentation"/>.
    /// </summary>
    public partial class GetIdAddIfNecessarySerializerRepresentationOp : ReturningOperationBase<int>, ISpecifyResourceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetIdAddIfNecessarySerializerRepresentationOp"/> class.
        /// </summary>
        /// <param name="specifiedResourceLocator">Stream locator to inspect.</param>
        /// <param name="serializerRepresentation">The serialization description.</param>
        /// <param name="serializationFormat">The serialization format.</param>
        /// <exception cref="System.ArgumentNullException">serializerRepresentation.</exception>
        public GetIdAddIfNecessarySerializerRepresentationOp(
            IResourceLocator specifiedResourceLocator,
            SerializerRepresentation serializerRepresentation,
            SerializationFormat serializationFormat)
        {
            this.SpecifiedResourceLocator = specifiedResourceLocator ?? throw new ArgumentNullException(nameof(specifiedResourceLocator));
            this.SerializerRepresentation = serializerRepresentation ?? throw new ArgumentNullException(nameof(serializerRepresentation));

            if (serializationFormat == SerializationFormat.Invalid)
            {
                throw new ArgumentException("Format cannot be 'Invalid'.", nameof(serializationFormat));
            }

            this.SerializationFormat = serializationFormat;
        }

        /// <summary>
        /// Gets the serialization description.
        /// </summary>
        /// <value>The serialization description.</value>
        public SerializerRepresentation SerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the serialization format.
        /// </summary>
        /// <value>The serialization format.</value>
        public SerializationFormat SerializationFormat { get; private set; }

        /// <inheritdoc />
        public IResourceLocator SpecifiedResourceLocator { get; private set; }
    }
}