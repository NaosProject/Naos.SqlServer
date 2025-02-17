// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.Tag.cs" company="Naos Project">
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
            /// Tag table schema.
            /// </summary>
            public static class Tag
            {
                /// <summary>
                /// The maximum length of a tag key.
                /// </summary>
                /// <remarks>
                /// SQL Server cannot add to a index that's greater than 1700 bytes.
                /// When using nvarchar, which uses 2 bytes per character, it means that that index cannot
                /// be greater than 850 characters.  So we split that 850 up between the tag key and tag value,
                /// which are both used in the index.
                /// </remarks>
                public const int TagKeyMaxLength = 300;

                /// <summary>
                /// The maximum length of a tag value.
                /// </summary>
                /// <remarks>
                /// See remarks for <see cref="TagKeyMaxLength"/>.
                /// </remarks>
                public const int TagValueMaxLength = 550;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnDefinition Id => new ColumnDefinition(nameof(Id), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the tag string identifier.
                /// </summary>
                public static ColumnDefinition TagKey => new ColumnDefinition(nameof(TagKey), new StringSqlDataTypeRepresentation(true, TagKeyMaxLength));

                /// <summary>
                /// Gets the tag value.
                /// </summary>
                public static ColumnDefinition TagValue => new ColumnDefinition(nameof(TagValue), new StringSqlDataTypeRepresentation(true, TagValueMaxLength));

                /// <summary>
                /// Gets the record created UTC.
                /// </summary>
                public static ColumnDefinition RecordCreatedUtc => new ColumnDefinition(nameof(RecordCreatedUtc), new UtcDateTimeSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the table.
                /// </summary>
                public static TableDefinition Table => new TableDefinition(
                    nameof(Tag),
                    new[]
                    {
                        Id,
                        TagKey,
                        TagValue,
                        RecordCreatedUtc,
                    });

                /// <summary>
                /// Builds the creation script for tag table.
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


CREATE TABLE [{streamName}].[Tag](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(TagKey)}] {TagKey.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagValue)}] {TagValue.SqlDataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NOT NULL,
-- Got this warning trying to add that constraint...
-- Warning! The maximum key length for a nonclustered index is 1700 bytes. The index 'UQ_TagKey_TagValue' has maximum length of 8900 bytes. For some combination of large values, the insert/update operation will fail.
-- CONSTRAINT [UQ_{Tables.Tag.TagKey.Name}_{Tables.Tag.TagValue.Name}] UNIQUE([{Tables.Tag.TagKey.Name}],[{Tables.Tag.TagValue.Name}])

 CONSTRAINT [PK_{nameof(Tag)}] PRIMARY KEY CLUSTERED
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING ON

CREATE UNIQUE NONCLUSTERED INDEX [IX_{nameof(Tag)}_{nameof(TagKey)}_{nameof(TagValue)}_Asc] ON [{streamName}].[Tag]
(
	[{nameof(TagKey)}] ASC,
	[{nameof(TagValue)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			");

                    return result;
                }
            }
        }
    }
}
