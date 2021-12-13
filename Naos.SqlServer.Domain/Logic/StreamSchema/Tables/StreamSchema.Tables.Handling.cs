// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Handling.cs" company="Naos Project">
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
            /// Handling table schema.
            /// </summary>
            public static class Handling
            {
                /// <summary>
                /// The invalid identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const long NullId = -1L;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the record identifier.
                /// </summary>
                public static ColumnDefinition RecordId => new ColumnDefinition(nameof(RecordId), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the concern.
                /// </summary>
                public static ColumnDefinition Concern => new ColumnDefinition(nameof(Concern), new StringSqlDataTypeRepresentation(true, 450));

                /// <summary>
                /// Gets the status.
                /// </summary>
                public static ColumnDefinition Status => new ColumnDefinition(nameof(Status), new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the details.
                /// </summary>
                public static ColumnDefinition Details => new ColumnDefinition(nameof(Details), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant));

                /// <summary>
                /// Gets the entry created in UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(Handling),
                    new[]
                    {
                        Id,
                        RecordId,
                        Concern,
                        Status,
                        Details,
                        RecordCreatedUtc,
                    });

                /// <summary>
                /// Builds the creation script for Handling table.
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


CREATE TABLE [{streamName}].[{Table.Name}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordId)}] {RecordId.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Concern)}] {Concern.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Status)}] {Status.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Details)}] {Details.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Handling)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(Handling)}] WITH NOCHECK ADD CONSTRAINT [FK_{nameof(Handling)}_{nameof(Record)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Record)}] ([{nameof(Record.Id)}])

-- Relax to NoCheck
ALTER TABLE [{streamName}].[{nameof(Handling)}] NOCHECK CONSTRAINT [FK_{nameof(Handling)}_{nameof(Record)}]

SET ANSI_PADDING ON


CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(RecordId)}_Desc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(RecordId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(Concern)}_Asc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(Concern)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(Status)}_Asc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(Status)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(RecordId)}_{nameof(Concern)}_Asc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(RecordId)}] DESC,
	[{nameof(Concern)}] ASC
)
INCLUDE ([{nameof(Id)}])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

			");

                    return result;
                }
            }
        }
    }
}
