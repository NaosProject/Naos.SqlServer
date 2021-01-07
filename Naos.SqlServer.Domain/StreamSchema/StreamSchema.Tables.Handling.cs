﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Handling.cs" company="Naos Project">
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
            public static class Handling
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
                public static ColumnRepresentation Details => new ColumnRepresentation(nameof(Details), new StringSqlDataTypeRepresentation(true, -1));

                /// <summary>
                /// Gets the resource identifier.
                /// </summary>
                /// <value>The resource identifier.</value>
                public static ColumnRepresentation ResourceId => new ColumnRepresentation(nameof(ResourceId), new IntSqlDataTypeRepresentation());

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
                    nameof(Handling),
                    new[]
                    {
                        Id,
                        RecordId,
                        Concern,
                        Status,
                        Details,
                        ResourceId,
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


CREATE TABLE [{streamName}].[Handling](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordId)}] {RecordId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ResourceId)}] {ResourceId.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(Concern)}] {Concern.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Status)}] {Status.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(Details)}] {Details.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Handling)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(Handling)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Handling)}_{nameof(Record)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Record)}] ([{nameof(Record.Id)}])


ALTER TABLE [{streamName}].[Handling] CHECK CONSTRAINT [FK_{nameof(Handling)}_{nameof(Record)}]

ALTER TABLE [{streamName}].[{nameof(Handling)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Handling)}_{nameof(Resource)}] FOREIGN KEY([{nameof(ResourceId)}])
REFERENCES [{streamName}].[{nameof(Resource)}] ([{nameof(Resource.Id)}])


ALTER TABLE [{streamName}].[Handling] CHECK CONSTRAINT [FK_{nameof(Handling)}_{nameof(Resource)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(Id)}_Asc] ON [{streamName}].[{nameof(Handling)}]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(RecordId)}_Desc] ON [{streamName}].[Handling]
(
	[{nameof(RecordId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(Concern)}_Asc] ON [{streamName}].[Handling]
(
	[{nameof(Concern)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(Status)}_Asc] ON [{streamName}].[Handling]
(
	[{nameof(Status)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Handling)}_{nameof(ResourceId)}_Desc] ON [{streamName}].[Handling]
(
	[{nameof(ResourceId)}] DesC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			");

                    return result;
                }
            }
        }
    }
}
