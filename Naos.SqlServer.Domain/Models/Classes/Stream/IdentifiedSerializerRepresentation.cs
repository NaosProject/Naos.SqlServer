// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentifiedSerializerRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type;
    using SerializationFormat = OBeautifulCode.Serialization.SerializationFormat;

    /// <summary>
    /// Existing serializer with database ID.
    /// </summary>
    public partial class IdentifiedSerializerRepresentation : IHaveId<int>, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiedSerializerRepresentation"/> class.
        /// </summary>
        /// <param name="id">The serializer representation identifier.</param>
        /// <param name="serializerRepresentation">The serializer description.</param>
        /// <param name="serializationFormat">The serialization format.</param>
        public IdentifiedSerializerRepresentation(
            int id,
            SerializerRepresentation serializerRepresentation,
            SerializationFormat serializationFormat)
        {
            serializerRepresentation.MustForArg(nameof(serializerRepresentation)).NotBeNull();
            serializationFormat.MustForArg(nameof(serializationFormat)).NotBeEqualTo(SerializationFormat.Invalid);

            this.Id = id;
            this.SerializerRepresentation = serializerRepresentation;
            this.SerializationFormat = serializationFormat;
        }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <summary>
        /// Gets the serializer description.
        /// </summary>
        public SerializerRepresentation SerializerRepresentation { get; private set; }

        /// <summary>
        /// Gets the serialization format.
        /// </summary>
        public SerializationFormat SerializationFormat { get; private set; }
    }
}
