// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.SerializerDescription.cs" company="Naos Project">
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
            /// Serializer representation table schema.
            /// </summary>
            public static class SerializerRepresentation
            {
                /// <summary>
                /// The invalid identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const int NullId = -1;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serialization kind.
                /// </summary>
                public static ColumnDefinition SerializationKind => new ColumnDefinition(
                    nameof(SerializationKind),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the serialization format.
                /// </summary>
                public static ColumnDefinition SerializationFormat => new ColumnDefinition(
                    nameof(SerializationFormat),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the serialization configuration type without version identifier.
                /// </summary>
                public static ColumnDefinition SerializationConfigurationTypeWithoutVersionId => new ColumnDefinition(
                    nameof(SerializationConfigurationTypeWithoutVersionId),
                    new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serialization configuration type with version identifier.
                /// </summary>
                public static ColumnDefinition SerializationConfigurationTypeWithVersionId => new ColumnDefinition(
                    nameof(SerializationConfigurationTypeWithVersionId),
                    new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the kind of the compression.
                /// </summary>
                public static ColumnDefinition CompressionKind => new ColumnDefinition(
                    nameof(CompressionKind),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the unregistered type encountered strategy.
                /// </summary>
                public static ColumnDefinition UnregisteredTypeEncounteredStrategy => new ColumnDefinition(
                    nameof(UnregisteredTypeEncounteredStrategy),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(
                    nameof(RecordCreatedUtc),
                    new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(SerializerRepresentation),
                    new[]
                    {
                        Id,
                        SerializationKind,
                        SerializationFormat,
                        SerializationConfigurationTypeWithoutVersionId,
                        SerializationConfigurationTypeWithVersionId,
                        CompressionKind,
                        UnregisteredTypeEncounteredStrategy,
                        RecordCreatedUtc,
                    });

                /// <summary>
                /// Builds the creation script for SerializerRepresentation table.
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


CREATE TABLE [{streamName}].[{nameof(SerializerRepresentation)}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(SerializationKind)}] {SerializationKind.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationFormat)}] {SerializationFormat.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationConfigurationTypeWithoutVersionId)}] {SerializationConfigurationTypeWithoutVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationConfigurationTypeWithVersionId)}] {SerializationConfigurationTypeWithVersionId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(CompressionKind)}] {CompressionKind.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(UnregisteredTypeEncounteredStrategy)}] {UnregisteredTypeEncounteredStrategy.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(SerializerRepresentation)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(SerializerRepresentation)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(SerializerRepresentation)}_TypeWithoutVersion] FOREIGN KEY([{nameof(SerializationConfigurationTypeWithoutVersionId)}])
REFERENCES [{streamName}].[TypeWithoutVersion] ([Id])


ALTER TABLE [{streamName}].[{nameof(SerializerRepresentation)}] CHECK CONSTRAINT [FK_{nameof(SerializerRepresentation)}_TypeWithoutVersion]


ALTER TABLE [{streamName}].[{nameof(SerializerRepresentation)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(SerializerRepresentation)}_TypeWithVersion] FOREIGN KEY([{nameof(SerializationConfigurationTypeWithVersionId)}])
REFERENCES [{streamName}].[TypeWithVersion] ([Id])


ALTER TABLE [{streamName}].[{nameof(SerializerRepresentation)}] CHECK CONSTRAINT [FK_{nameof(SerializerRepresentation)}_TypeWithVersion]

ALTER TABLE [{streamName}].[{nameof(SerializerRepresentation)}] ADD CONSTRAINT [UQ_{nameof(SerializerRepresentation)}_All] UNIQUE([{nameof(SerializationKind)}], [{nameof(SerializationFormat)}], [{nameof(SerializationConfigurationTypeWithoutVersionId)}], [{nameof(SerializationConfigurationTypeWithVersionId)}], [{nameof(CompressionKind)}], [{nameof(UnregisteredTypeEncounteredStrategy)}]);

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}__{nameof(SerializationKind)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(SerializationKind)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_{nameof(SerializationFormat)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(SerializationFormat)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_{nameof(SerializationConfigurationTypeWithoutVersionId)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(SerializationConfigurationTypeWithoutVersionId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_{nameof(SerializationConfigurationTypeWithVersionId)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(SerializationConfigurationTypeWithVersionId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_{nameof(CompressionKind)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(CompressionKind)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_{nameof(UnregisteredTypeEncounteredStrategy)}_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(UnregisteredTypeEncounteredStrategy)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
");

                    return result;
                }
            }
        }
    }
}
