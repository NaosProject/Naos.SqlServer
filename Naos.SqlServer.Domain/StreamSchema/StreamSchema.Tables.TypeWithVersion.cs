﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.TypeWithVersion.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
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
            /// TypeWithVersion table.
            /// </summary>
            public static class TypeWithVersion
            {
                /// <summary>
                /// The invalid identifier that is returned to indicate inaction (the null object pattern of the identifier).
                /// </summary>
                public const int NullId = -1;

                /// <summary>
                /// Gets the identifier.
                /// </summary>
                /// <value>The identifier.</value>
                public static ColumnRepresentation Id => new ColumnRepresentation(nameof(Id), new IntSqlDataTypeRepresentation());

                /// <summary>
                /// Gets the name of the assembly qualified.
                /// </summary>
                /// <value>The name of the assembly qualified.</value>
                public static ColumnRepresentation AssemblyQualifiedName => new ColumnRepresentation(
                    nameof(AssemblyQualifiedName),
                    new StringSqlDataTypeRepresentation(true, 2000));

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
                    nameof(TypeWithVersion),
                    new[]
                    {
                        Id,
                        AssemblyQualifiedName,
                        RecordCreatedUtc,
                    }.ToDictionary(k => k.Name, v => v));

                /// <summary>
                /// Builds the creation script for type With version table.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Creation script for the type With version table.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    var result = FormattableString.Invariant(
                        $@"
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [{streamName}].[TypeWithVersion](
	[{nameof(Id)}] {Id.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
	[{nameof(AssemblyQualifiedName)}] {AssemblyQualifiedName.DataType.DeclarationInSqlSyntax} UNIQUE NOT NULL,
	[{nameof(RecordCreatedUtc)}] {RecordCreatedUtc.DataType.DeclarationInSqlSyntax} NULL,
 CONSTRAINT [PK_{nameof(TypeWithVersion)}] PRIMARY KEY CLUSTERED 
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