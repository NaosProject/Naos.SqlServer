// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.RecordTag.cs" company="Naos Project">
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
            /// Class RecordTag.
            /// </summary>
            public static class RecordTag
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                /// <value>The identifier.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the record identifier.
                /// </summary>
                /// <value>The record identifier.</value>
                public static ColumnRepresentation RecordId => new ColumnRepresentation(nameof(RecordId), Tables.Record.Id.DataType);

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
                    nameof(RecordTag),
                    new[]
                    {
                        Id,
                        RecordId,
                        TagId,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for RecordTag table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for RecordTag.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[{nameof(RecordTag)}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordId)}] {RecordId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagId)}] {TagId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(RecordTag)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(RecordTag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Record)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Record)}] ([{nameof(Record.Id)}])

ALTER TABLE [{streamName}].[{nameof(RecordTag)}] CHECK CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Record)}]


ALTER TABLE [{streamName}].[{nameof(RecordTag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Tag)}] FOREIGN KEY([{nameof(TagId)}])
REFERENCES [{streamName}].[{nameof(Tag)}] ([{nameof(Tag.Id)}])

ALTER TABLE [{streamName}].[{nameof(RecordTag)}] CHECK CONSTRAINT [FK_{nameof(RecordTag)}_{nameof(Tag)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(RecordTag)}_{nameof(Id)}_Asc] ON [{streamName}].[{nameof(RecordTag)}]
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
