// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Resource.cs" company="Naos Project">
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
            /// Resource table.
            /// </summary>
            public static class Resource
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the serialization kind.
                /// </summary>
                public static ColumnRepresentation Details => new ColumnRepresentation(
                    nameof(Details),
                    new StringSqlDataTypeRepresentation(true, -1));

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
                    nameof(Resource),
                    new[]
                    {
                        Id,
                        Details,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for Resource table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for Resource table.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[{nameof(Resource)}](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(Details)}] {Details.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Resource)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Resource)}_Id_Asc] ON [{streamName}].[{nameof(Resource)}]
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
");

                    return result;
                }
            }
        }
    }
}
