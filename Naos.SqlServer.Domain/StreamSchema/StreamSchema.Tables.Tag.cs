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
    /// TODO: Starting point for new project.
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
                /// Gets the record identifier.
                /// </summary>
                /// <value>The record identifier.</value>
                public static ColumnRepresentation RecordId => new ColumnRepresentation(nameof(RecordId), new BigIntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the object type without version identifier.
                /// </summary>
                /// <value>The object type without version identifier.</value>
                public static ColumnRepresentation ObjectTypeWithoutVersionId => new ColumnRepresentation(nameof(ObjectTypeWithoutVersionId), new IntSqlDataTypeRepresentation());

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
                        RecordId,
                        ObjectTypeWithoutVersionId,
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
	[{nameof(RecordId)}] {RecordId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(ObjectTypeWithoutVersionId)}] {ObjectTypeWithoutVersionId.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagKey)}] {TagKey.DataType.DeclarationInSqlSyntax} NOT NULL,
	[{nameof(TagValue)}] {TagValue.DataType.DeclarationInSqlSyntax} NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NOT NULL,
 CONSTRAINT [PK_{nameof(Tag)}] PRIMARY KEY CLUSTERED 
(
	[{nameof(Id)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [{streamName}].[{nameof(Tag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Tag)}_{nameof(Object)}] FOREIGN KEY([{nameof(RecordId)}])
REFERENCES [{streamName}].[{nameof(Object)}] ([{nameof(Object.Id)}])


ALTER TABLE [{streamName}].[Tag] CHECK CONSTRAINT [FK_{nameof(Tag)}_{nameof(Object)}]

ALTER TABLE [{streamName}].[{nameof(Tag)}]  WITH CHECK ADD  CONSTRAINT [FK_{nameof(Tag)}_{nameof(TypeWithoutVersion)}] FOREIGN KEY([{nameof(ObjectTypeWithoutVersionId)}])
REFERENCES [{streamName}].[{nameof(TypeWithoutVersion)}] ([{nameof(TypeWithoutVersion.Id)}])


ALTER TABLE [{streamName}].[Tag] CHECK CONSTRAINT [FK_{nameof(Tag)}_{nameof(TypeWithoutVersion)}]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_{nameof(Tag)}_{nameof(Id)}_Asc] ON [{streamName}].[{nameof(Tag)}]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Tag)}_{nameof(RecordId)}_Desc] ON [{streamName}].[Tag]
(
	[{nameof(RecordId)}] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_{nameof(Tag)}_{nameof(ObjectTypeWithoutVersionId)}_Asc] ON [{streamName}].[Tag]
(
	[{nameof(ObjectTypeWithoutVersionId)}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
