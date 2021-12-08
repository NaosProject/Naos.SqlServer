// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.TypeWithoutVersion.cs" company="Naos Project">
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
            /// Type without version table schema.
            /// </summary>
            public static class TypeWithoutVersion
            {
                /// <summary>
                /// The invalid identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const int NullId = -1;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the name of the assembly qualified.
                /// </summary>
                public static ColumnRepresentation AssemblyQualifiedName => new ColumnRepresentation(
                    nameof(AssemblyQualifiedName),
                    new StringSqlDataTypeRepresentation(true, 2000));

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
                    nameof(TypeWithoutVersion),
                    new[]
                    {
                        Id,
                        AssemblyQualifiedName,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for type without version table.
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


CREATE TABLE [{streamName}].[TypeWithoutVersion](
	[{nameof(Id)}] {Id.SqlDataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(AssemblyQualifiedName)}] {AssemblyQualifiedName.SqlDataType.DeclarationInSqlSyntax} UNIQUE NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} NULL,
 CONSTRAINT [PK_{nameof(TypeWithoutVersion)}] PRIMARY KEY CLUSTERED
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