// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Record.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;

    public static partial class StreamSchema
    {
        public static partial class Tables
        {
            /// <summary>
            /// Record table schema.
            /// </summary>
            public static class Record
            {
                /// <summary>
                /// The invalid record identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const long NullId = -1L;

                /// <summary>
                /// The token to substitute for a null string serialized identifier.
                /// </summary>
                public const string NullStringSerializedId = "<<<NULL>>>";

                /// <summary>
                /// Gets the identifier column.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the identifier type without version identifier.
                /// </summary>
                public static ColumnDefinition IdentifierTypeWithoutVersionId => new ColumnDefinition(nameof(IdentifierTypeWithoutVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the identifier type without version identifier.
                /// </summary>
                public static ColumnDefinition IdentifierTypeWithVersionId => new ColumnDefinition(nameof(IdentifierTypeWithVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the object type without version identifier.
                /// </summary>
                public static ColumnDefinition ObjectTypeWithoutVersionId => new ColumnDefinition(nameof(ObjectTypeWithoutVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the object type without version identifier.
                /// </summary>
                public static ColumnDefinition ObjectTypeWithVersionId => new ColumnDefinition(nameof(ObjectTypeWithVersionId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serializer description identifier.
                /// </summary>
                public static ColumnDefinition SerializerRepresentationId => new ColumnDefinition(nameof(SerializerRepresentationId), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the string serialized object identifier.
                /// </summary>
                public static ColumnDefinition StringSerializedId => new ColumnDefinition(nameof(StringSerializedId), new StringSqlDataTypeRepresentation(true, 450));

                /// <summary>
                /// Gets the string serialized object string.
                /// </summary>
                public static ColumnDefinition StringSerializedObject => new ColumnDefinition(nameof(StringSerializedObject), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant));

                /// <summary>
                /// Gets the binary serialized object string.
                /// </summary>
                public static ColumnDefinition BinarySerializedObject => new ColumnDefinition(nameof(BinarySerializedObject), new BinarySqlDataTypeRepresentation(BinarySqlDataTypeRepresentation.MaxLengthConstant));

                /// <summary>
                /// Gets the tag identifiers as CSV.
                /// </summary>
                public static ColumnDefinition TagIdsCsv => new ColumnDefinition(
                    nameof(TagIdsCsv),
                    new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant));

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the date time from object.
                /// </summary>
                public static ColumnDefinition ObjectDateTimeUtc => new ColumnDefinition(nameof(ObjectDateTimeUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(Record),
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
                    });

                /// <summary>
                /// Builds the creation script for object table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for creating the table.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [{streamName}].[{nameof(Record)}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(IdentifierTypeWithoutVersionId)}] {IdentifierTypeWithoutVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(IdentifierTypeWithVersionId)}] {IdentifierTypeWithVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ObjectTypeWithoutVersionId)}] {ObjectTypeWithoutVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ObjectTypeWithVersionId)}] {ObjectTypeWithVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializerRepresentationId)}] {SerializerRepresentationId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(StringSerializedId)}] {StringSerializedId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(StringSerializedObject)}] {StringSerializedObject.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(BinarySerializedObject)}] {BinarySerializedObject.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(TagIdsCsv)}] {TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(ObjectDateTimeUtc)}] {ObjectDateTimeUtc.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Record)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [{streamName}].[{nameof(Record)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Record)}Id_{nameof(TypeWithoutVersion)}] FOREIGN KEY([{nameof(IdentifierTypeWithoutVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithoutVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Record)}] CHECK CONSTRAINT [FK_{nameof(Record)}Id_{nameof(TypeWithoutVersion)}]

ALTER TABLE [{streamName}].[{nameof(Record)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Record)}Id_{nameof(TypeWithVersion)}] FOREIGN KEY([{nameof(IdentifierTypeWithVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Record)}] CHECK CONSTRAINT [FK_{nameof(Record)}Id_{nameof(TypeWithVersion)}]

ALTER TABLE [{streamName}].[{nameof(Record)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Record)}_{nameof(TypeWithoutVersion)}] FOREIGN KEY([{nameof(ObjectTypeWithoutVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithoutVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Record)}] CHECK CONSTRAINT [FK_{nameof(Record)}_{nameof(TypeWithoutVersion)}]

ALTER TABLE [{streamName}].[{nameof(Record)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Record)}_{nameof(TypeWithVersion)}] FOREIGN KEY([{nameof(ObjectTypeWithVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithVersion)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Record)}] CHECK CONSTRAINT [FK_{nameof(Record)}_{nameof(TypeWithVersion)}]

ALTER TABLE [{streamName}].[{nameof(Record)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Record)}_{nameof(SerializerRepresentation)}] FOREIGN KEY([{nameof(SerializerRepresentationId)}])
REFERENCES [{streamName}].[{nameof(SerializerRepresentation)}] ([Id])

ALTER TABLE [{streamName}].[{nameof(Record)}] CHECK CONSTRAINT [FK_{nameof(Record)}_{nameof(SerializerRepresentation)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(StringSerializedId)}_Asc] ON [{streamName}].[{nameof(Record)}]
(
	[{nameof(StringSerializedId)}] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(IdentifierTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Record)}]
(
	[{nameof(IdentifierTypeWithoutVersionId)}] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(ObjectTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Record)}]
(
	[{nameof(ObjectTypeWithoutVersionId)}] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(StringSerializedId)}_{nameof(ObjectTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Record)}]
(
	[{nameof(StringSerializedId)}] ASC,
	[{nameof(ObjectTypeWithoutVersionId)}] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(StringSerializedId)}_{nameof(IdentifierTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(Record)}]
(
	[{nameof(StringSerializedId)}] ASC,
	[{nameof(IdentifierTypeWithoutVersionId)}] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


-- -- Other indexes below cause slow downs on high throughput - need a strategy for apply these so keeping commented out for now.
--CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(ObjectDateTimeUtc)}] ON [{streamName}].[{nameof(Record)}]
--(
--	[{nameof(ObjectDateTimeUtc)}] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

--CREATE NONCLUSTERED INDEX [IX_{nameof(Record)}_{nameof(RecordCreatedUtc)}] ON [{streamName}].[{nameof(Record)}]
--(
--	[{nameof(RecordCreatedUtc)}] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			");

                    return result;
                }
            }
        }
    }
}
