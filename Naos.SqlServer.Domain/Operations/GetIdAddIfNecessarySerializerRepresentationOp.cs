// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetIdAddIfNecessarySerializerRepresentationOp.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
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
        /// <param name="specifiedResourceLocator">The stream locator to inspect.</param>
        /// <param name="serializerRepresentation">The serializer representation.</param>
        /// <param name="serializationFormat">The serialization format.</param>
        public GetIdAddIfNecessarySerializerRepresentationOp(
            IResourceLocator specifiedResourceLocator,
            SerializerRepresentation serializerRepresentation,
            SerializationFormat serializationFormat)
        {
            specifiedResourceLocator.MustForArg(nameof(specifiedResourceLocator)).NotBeNull();
            serializerRepresentation.MustForArg(nameof(serializerRepresentation)).NotBeNull();
            serializationFormat.MustForArg(nameof(serializationFormat)).NotBeEqualTo(SerializationFormat.Invalid);

            this.SpecifiedResourceLocator = specifiedResourceLocator;
            this.SerializerRepresentation = serializerRepresentation;
            this.SerializationFormat = serializationFormat;
        }

        /// <inheritdoc />
        public IResourceLocator SpecifiedResourceLocator { get; private set; }

        /// <summary>
        /// Gets the serializer representation.
        /// </summary>
        public SerializerRepresentation SerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the serialization format.
        /// </summary>
        public SerializationFormat SerializationFormat { get; private set; }
    }
}