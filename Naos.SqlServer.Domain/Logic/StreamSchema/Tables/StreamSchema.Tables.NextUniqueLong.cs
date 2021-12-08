// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.NextUniqueLong.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Linq;

    public static partial class StreamSchema
    {
        public static partial class Tables
        {
            /// <summary>
            /// Next unique long table schema.
            /// </summary>
            public static class NextUniqueLong
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnRepresentation RecordCreatedUtc => new ColumnRepresentation(
                    nameof(RecordCreatedUtc),
                    new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableRepresentation Table => new TableRepresentation(
                    nameof(NextUniqueLong),
                    new[]
                    {
                        Id,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for NextUniqueLong table.
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


CREATE TABLE [{streamName}].[{nameof(NextUniqueLong)}](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(NextUniqueLong)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING ON
");

                    return result;
                }
            }
        }
    }
}
