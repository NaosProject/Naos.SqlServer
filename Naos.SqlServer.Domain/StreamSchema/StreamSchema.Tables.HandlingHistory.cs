// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.HandlingHistory.cs" company="Naos Project">
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
        /// Class Tables.
        /// </summary>
        public static partial class Tables
        {
            /// <summary>
            /// Class Handling.
            /// </summary>
            public static class HandlingHistory
            {
                /// <summary>
                /// The invalid identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const long NullId = -1L;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                /// <value>The identifier.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the record identifier.
                /// </summary>
                /// <value>The record identifier.</value>
                public static ColumnRepresentation RecordId => new ColumnRepresentation(nameof(RecordId), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the concern.
                /// </summary>
                /// <value>The concern.</value>
                public static ColumnRepresentation Concern => new ColumnRepresentation(nameof(Concern), new StringSqlDataTypeRepresentation(true, 450));

                /// <summary>
                /// Gets the status.
                /// </summary>
                /// <value>The status.</value>
                public static ColumnRepresentation Status => new ColumnRepresentation(nameof(Status), new StringSqlDataTypeRepresentation(false, 50));

                /// <summary>
                /// Gets the details.
                /// </summary>
                /// <value>The details.</value>
                public static ColumnRepresentation Details => new ColumnRepresentation(nameof(Details), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant));

                /// <summary>
                /// Gets the entry created in UTC.
                /// </summary>
                /// <value>The entry created in UTC.</value>
                public static ColumnRepresentation RecordCreatedUtc => new ColumnRepresentation(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                /// <value>The table.</value>
                public static TableRepresentation Table => new TableRepresentation(
                    nameof(HandlingHistory),
                    new[]
                    {
                        Id,
                        RecordId,
                        Concern,
                        Status,
                        Details,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for Handling table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for Handling.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[{Table.Name}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordId)}] {RecordId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Concern)}] {Concern.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Status)}] {Status.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Details)}] {Details.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(HandlingHistory)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(HandlingHistory)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(HandlingHistory)}_{nameof(Record)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Record)}] ([{nameof(Record.Id)}])

ALTER TABLE [{streamName}].[{nameof(HandlingHistory)}] CHECK CONSTRAINT [FK_{nameof(HandlingHistory)}_{nameof(Record)}]


SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingHistory)}_{nameof(RecordId)}_Desc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(RecordId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingHistory)}_{nameof(Concern)}_Asc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(Concern)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingHistory)}_{nameof(Status)}_Asc] ON [{streamName}].[{Table.Name}]
(
	[{nameof(Status)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(HandlingHistory)}_{nameof(RecordId)}_{nameof(Concern)}_Asc] ON [{streamName}].[{Table.Name}]
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
