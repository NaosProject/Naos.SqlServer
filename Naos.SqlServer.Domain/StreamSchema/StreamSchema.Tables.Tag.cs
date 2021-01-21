// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Tag.cs" company="Naos Project">
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
            /// Class Tag.
            /// </summary>
            public static class Tag
            {
                /// <summary>
                /// Gets the identifier.
                /// </summary>
                /// <value>The identifier.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the tag string identifier.
                /// </summary>
                /// <value>The tag string identifier.</value>
                public static ColumnRepresentation TagKey => new ColumnRepresentation(nameof(TagKey), new StringSqlDataTypeRepresentation(true, 450));

                /// <summary>
                /// Gets the tag value.
                /// </summary>
                /// <value>The tag value.</value>
                public static ColumnRepresentation TagValue => new ColumnRepresentation(nameof(TagValue), new StringSqlDataTypeRepresentation(true, 4000));

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
                    nameof(Tag),
                    new[]
                    {
                        Id,
                        TagKey,
                        TagValue,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for tag table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for tag.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[Tag](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(TagKey)}] {TagKey.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagValue)}] {TagValue.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
-- Got this warning trying to add that constraint...
-- Warning! The maximum key length for a nonclustered index is 1700 bytes. The index 'UQ_TagKey_TagValue' has maximum length of 8900 bytes. For some combination of large values, the insert/update operation will fail.
-- CONSTRAINT [UQ_{Tables.Tag.TagKey.Name}_{Tables.Tag.TagValue.Name}] UNIQUE([{Tables.Tag.TagKey.Name}],[{Tables.Tag.TagValue.Name}])

 CONSTRAINT [PK_{nameof(Tag)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Tag)}_{nameof(TagKey)}_Asc] ON [{streamName}].[Tag]
(
	[{nameof(TagKey)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			");

                    return result;
                }
            }
        }
    }
}
