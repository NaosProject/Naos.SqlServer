﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.RecordTag.cs" company="Naos Project">
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
            /// Record tag table schema.
            /// </summary>
            public static class RecordTag
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the record identifier.
                /// </summary>
                public static ColumnDefinition RecordId => new ColumnDefinition(nameof(RecordId), Tables.Record.Id.SqlDataType);

                /// <summary>
                /// Gets the tag identifier.
                /// </summary>
                public static ColumnDefinition TagId => new ColumnDefinition(nameof(TagId), Tables.Tag.Id.SqlDataType);

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(RecordTag),
                    new[]
                    {
                        Id,
                        RecordId,
                        TagId,
                        RecordCreatedUtc,
                    });

                /// <summary>
                /// Builds the creation script for RecordTag table.
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


CREATE TABLE [{streamName}].[{nameof(RecordTag)}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordId)}] {RecordId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagId)}] {TagId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(RecordTag)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [{streamName}].[{nameof(RecordTag)}] WITH NOCHECK ADD CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Record)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Record)}] ([{nameof(Record.Id)}])

-- Relax to NoCheck
ALTER TABLE [{streamName}].[{nameof(RecordTag)}] NOCHECK CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Record)}]

ALTER TABLE [{streamName}].[{nameof(RecordTag)}] WITH NOCHECK ADD CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Tag)}] FOREIGN KEY([{nameof(TagId)}])
REFERENCES [{streamName}].[{nameof(Tag)}] ([{nameof(Tag.Id)}])

-- Relax to NoCheck
ALTER TABLE [{streamName}].[{nameof(RecordTag)}] NOCHECK CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Tag)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(RecordTag)}_{nameof(RecordId)}_Desc] ON [{streamName}].[{nameof(RecordTag)}]
(
	[{nameof(RecordId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(RecordTag)}_{nameof(TagId)}_Desc] ON [{streamName}].[{nameof(RecordTag)}]
(
	[{nameof(TagId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

		");

                    return result;
                }
            }
        }
    }
}
