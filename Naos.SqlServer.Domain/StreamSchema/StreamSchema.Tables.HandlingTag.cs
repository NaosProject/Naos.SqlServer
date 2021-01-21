// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.HandlingTag.cs" company="Naos Project">
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
            /// Class HandlingTag.
            /// </summary>
            public static class HandlingTag
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                /// <value>The identifier.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the handling entry identifier.
                /// </summary>
                /// <value>The handling entry identifier.</value>
                public static ColumnRepresentation HandlingId => new ColumnRepresentation(nameof(HandlingId), Tables.HandlingHistory.Id.DataType);

                /// <summary>
                /// Gets the tag identifier.
                /// </summary>
                /// <value>The tag identifier.</value>
                public static ColumnRepresentation TagId => new ColumnRepresentation(nameof(TagId), Tables.Tag.Id.DataType);

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                /// <value>The record created UTC.</value>
                public static ColumnRepresentation RecordCreatedUtc => new ColumnRepresentation(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                /// <value>The table.</value>
                public static TableRepresentation Table => new TableRepresentation(
                    nameof(HandlingTag),
                    new[]
                    {
                        Id,
                        HandlingId,
                        TagId,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for HandlingTag table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for HandlingTag.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[{nameof(HandlingTag)}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(HandlingId)}] {HandlingId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagId)}] {TagId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(HandlingTag)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(HandlingTag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(HandlingHistory)}] FOREIGN KEY([{nameof(HandlingId)}])
REFERENCES [{streamName}].[{nameof(HandlingHistory)}] ([{nameof(HandlingHistory.Id)}])

ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] CHECK CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(HandlingHistory)}]


ALTER TABLE [{streamName}].[{nameof(HandlingTag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Tag)}] FOREIGN KEY([{nameof(TagId)}])
REFERENCES [{streamName}].[{nameof(Tag)}] ([{nameof(Tag.Id)}])

ALTER TABLE [{streamName}].[{nameof(HandlingTag)}] CHECK CONSTRAINT [FK_{nameof(HandlingTag)}_{nameof(Tag)}]

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
