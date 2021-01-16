// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.SerializerDescription.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Linq;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// The tables.
        /// </summary>
        public static partial class Tables
        {
            /// <summary>
            /// SerializerRepresentation table.
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
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serialization kind.
                /// </summary>
                public static ColumnRepresentation SerializationKind => new ColumnRepresentation(
                    nameof(SerializationKind),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the serialization format.
                /// </summary>
                public static ColumnRepresentation SerializationFormat => new ColumnRepresentation(
                    nameof(SerializationFormat),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the serialization configuration type without version identifier.
                /// </summary>
                /// <value>The serialization configuration type without version identifier.</value>
                public static ColumnRepresentation SerializationConfigurationTypeWithoutVersionId => new ColumnRepresentation(
                    nameof(SerializationConfigurationTypeWithoutVersionId),
                    new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serialization configuration type with version identifier.
                /// </summary>
                /// <value>The serialization configuration type with version identifier.</value>
                public static ColumnRepresentation SerializationConfigurationTypeWithVersionId => new ColumnRepresentation(
                    nameof(SerializationConfigurationTypeWithVersionId),
                    new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the kind of the compression.
                /// </summary>
                /// <value>The kind of the compression.</value>
                public static ColumnRepresentation CompressionKind => new ColumnRepresentation(
                    nameof(CompressionKind),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the unregistered type encountered strategy.
                /// </summary>
                /// <value>The unregistered type encountered strategy.</value>
                public static ColumnRepresentation UnregisteredTypeEncounteredStrategy => new ColumnRepresentation(
                    nameof(UnregisteredTypeEncounteredStrategy),
                    new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                /// <value>The record created UTC.</value>
                public static ColumnRepresentation RecordCreatedUtc => new ColumnRepresentation(
                    nameof(RecordCreatedUtc),
                    new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                /// <value>The table.</value>
                public static TableRepresentation Table => new TableRepresentation(
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
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for SerializerRepresentation table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for SerializerRepresentation table.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[{nameof(SerializerRepresentation)}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(SerializationKind)}] {SerializationKind.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationFormat)}] {SerializationFormat.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationConfigurationTypeWithoutVersionId)}] {SerializationConfigurationTypeWithoutVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(SerializationConfigurationTypeWithVersionId)}] {SerializationConfigurationTypeWithVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(CompressionKind)}] {CompressionKind.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(UnregisteredTypeEncounteredStrategy)}] {UnregisteredTypeEncounteredStrategy.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
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

CREATE NONCLUSTERED INDEX [IX_{nameof(SerializerRepresentation)}_Id_Asc] ON [{streamName}].[{nameof(SerializerRepresentation)}]
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
