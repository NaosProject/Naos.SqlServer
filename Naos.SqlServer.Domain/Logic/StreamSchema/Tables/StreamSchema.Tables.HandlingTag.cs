﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.HandlingTag.cs" company="Naos Project">
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
            /// Handling tag table schema.
            /// </summary>
            public static class HandlingTag
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the handling entry identifier.
                /// </summary>
                public static ColumnDefinition HandlingId => new ColumnDefinition(nameof(HandlingId), Tables.Handling.Id.SqlDataType);

                /// <summary>
                /// Gets the tag identifier.
                /// </summary>
                public static ColumnDefinition TagId => new ColumnDefinition(nameof(TagId), Tag.Id.SqlDataType);

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(HandlingTag),
                    new[]
                    {
                        Id,
                        HandlingId,
                        TagId,
                        RecordCreatedUtc,
                    });

                /// <summary>
                /// Builds the creation script for HandlingTag table.
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


CREATE TABLE [{streamName}].[{nameof(HandlingTag)}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(HandlingId)}] {HandlingId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagId)}] {TagId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(HandlingTag)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] WITH NOCHECK ADD CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Handling)}] FOREIGN KEY([{nameof(HandlingId)}])
REFERENCES [{streamName}].[{nameof(Handling)}] ([{nameof(Handling.Id)}])

-- Relax to NoCheck
ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] NOCHECK CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Handling)}]

ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] WITH NOCHECK ADD CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Tag)}] FOREIGN KEY([{nameof(TagId)}])
REFERENCES [{streamName}].[{nameof(Tag)}] ([{nameof(Tag.Id)}])

-- Relax to NoCheck
ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] NOCHECK CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Tag)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingTag)}_{nameof(HandlingId)}_Desc] ON [{streamName}].[{nameof(HandlingTag)}]
(
	[{nameof(HandlingId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingTag)}_{nameof(TagId)}_Desc] ON [{streamName}].[{nameof(HandlingTag)}]
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
