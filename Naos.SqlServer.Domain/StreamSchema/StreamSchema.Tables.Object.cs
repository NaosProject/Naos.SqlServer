// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Object.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Linq;

    /// <summary>
    /// Object table schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Table schema.
        /// </summary>
        public static partial class Tables
        {
            /// <summary>
            /// Object table.
            /// </summary>
            public static class Object
            {
                /// <summary>
                /// Gets the identifier column.
                /// </summary>
                /// <value>The identifier column.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the identifier type without version identifier.
                /// </summary>
                /// <value>The identifier type without version identifier.</value>
                public static ColumnRepresentation IdentifierTypeWithoutVersionId => new ColumnRepresentation(nameof(IdentifierTypeWithoutVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the identifier type without version identifier.
                /// </summary>
                /// <value>The identifier type without version identifier.</value>
                public static ColumnRepresentation IdentifierTypeWithVersionId => new ColumnRepresentation(nameof(IdentifierTypeWithVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the object type without version identifier.
                /// </summary>
                /// <value>The object type without version identifier.</value>
                public static ColumnRepresentation ObjectTypeWithoutVersionId => new ColumnRepresentation(nameof(ObjectTypeWithoutVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the object type without version identifier.
                /// </summary>
                /// <value>The object type without version identifier.</value>
                public static ColumnRepresentation ObjectTypeWithVersionId => new ColumnRepresentation(nameof(ObjectTypeWithVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serializer description identifier.
                /// </summary>
                /// <value>The serializer description identifier.</value>
                public static ColumnRepresentation SerializerRepresentationId => new ColumnRepresentation(nameof(SerializerRepresentationId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the string serialized object identifier.
                /// </summary>
                /// <value>The string serialized object identifier.</value>
                public static ColumnRepresentation StringSerializedId => new ColumnRepresentation(nameof(StringSerializedId), new StringSqlDataTypeRepresentation(true, 450));

                /// <summary>
                /// Gets the string serialized object string.
                /// </summary>
                /// <value>The string serialized object string.</value>
                public static ColumnRepresentation StringSerializedObject => new ColumnRepresentation(nameof(StringSerializedObject), new StringSqlDataTypeRepresentation(true, -1));

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                /// <value>The record created UTC.</value>
                public static ColumnRepresentation RecordCreatedUtc => new ColumnRepresentation(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the date time from object.
                /// </summary>
                /// <value>The object date time UTC.</value>
                public static ColumnRepresentation ObjectDateTimeUtc => new ColumnRepresentation(nameof(ObjectDateTimeUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                /// <value>The table.</value>
                public static TableRepresentation Table => new TableRepresentation(
                    nameof(Object),
                    new[]
                    {
                        Id,
                        IdentifierTypeWithoutVersionId,
                        IdentifierTypeWithVersionId,
                        ObjectTypeWithoutVersionId,
                        ObjectTypeWithVersionId,
                        SerializerRepresentationId,
                        StringSerializedId,
                        StringSerializedObject,
                        RecordCreatedUtc,
                        ObjectDateTimeUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for object table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for object table.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [{streamName}].[{nameof(Object)}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(IdentifierTypeWithoutVersionId)}] {IdentifierTypeWithoutVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(IdentifierTypeWithVersionId)}] {IdentifierTypeWithVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ObjectTypeWithoutVersionId)}] {ObjectTypeWithoutVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ObjectTypeWithVersionId)}] {ObjectTypeWithVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializerRepresentationId)}] {SerializerRepresentationId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(StringSerializedId)}] {StringSerializedId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(StringSerializedObject)}] {StringSerializedObject.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(ObjectDateTimeUtc)}] {ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Object)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [{streamName}].[{nameof(Object)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Object)}Id_{nameof(TypeWithoutVersion)}] FOREIGN KEY([{nameof(IdentifierTypeWithoutVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithoutVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Object)}] CHECK CONSTRAINT [FK_{nameof(Object)}Id_{nameof(TypeWithoutVersion)}]

ALTER TABLE [{streamName}].[{nameof(Object)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Object)}Id_{nameof(TypeWithVersion)}] FOREIGN KEY([{nameof(IdentifierTypeWithVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Object)}] CHECK CONSTRAINT [FK_{nameof(Object)}Id_{nameof(TypeWithVersion)}]

ALTER TABLE [{streamName}].[{nameof(Object)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Object)}_{nameof(TypeWithoutVersion)}] FOREIGN KEY([{nameof(ObjectTypeWithoutVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithoutVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Object)}] CHECK CONSTRAINT [FK_{nameof(Object)}_{nameof(TypeWithoutVersion)}]

ALTER TABLE [{streamName}].[{nameof(Object)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Object)}_{nameof(TypeWithVersion)}] FOREIGN KEY([{nameof(ObjectTypeWithVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Object)}] CHECK CONSTRAINT [FK_{nameof(Object)}_{nameof(TypeWithVersion)}]

ALTER TABLE [{streamName}].[{nameof(Object)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Object)}_{nameof(SerializerRepresentation)}] FOREIGN KEY([{nameof(SerializerRepresentationId)}])
REFERENCES [{streamName}].[{nameof(SerializerRepresentation)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Object)}] CHECK CONSTRAINT [FK_{nameof(Object)}_{nameof(SerializerRepresentation)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(Id)}_Asc] ON [{streamName}].[{nameof(Object)}]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(IdentifierTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Object)}]
(
	[{nameof(IdentifierTypeWithoutVersionId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(ObjectTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Object)}]
(
	[{nameof(ObjectTypeWithoutVersionId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(StringSerializedId)}_Asc] ON [{streamName}].[{nameof(Object)}]
(
	[{nameof(StringSerializedId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(ObjectDateTimeUtc)}] ON [{streamName}].[{nameof(Object)}]
(
	[{nameof(ObjectDateTimeUtc)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Object)}_{nameof(RecordCreatedUtc)}] ON [{streamName}].[{nameof(Object)}]
(
	[{nameof(RecordCreatedUtc)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


			");

                    return result;
                }
            }
        }
    }
}
