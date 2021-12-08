// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Tables.CompositeHandlingStatusSortOrder.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

////namespace Naos.SqlServer.Domain
////{
////    using System;
////    using System.Collections.Generic;
////    using System.Linq;

////    /// <summary>
////    /// Container for schema.
////    /// </summary>
////    public static partial class StreamSchema
////    {
////        /// <summary>
////        /// Class Tables.
////        /// </summary>
////        public static partial class Tables
////        {
////            /// <summary>
////            /// CompositeHandlingStatusSortOrder table.
////            /// </summary>
////            public static class CompositeHandlingStatusSortOrder
////            {
////                /// <summary>
////                /// Gets the sort order.
////                /// </summary>
////                /// <value>The sort order.</value>
////                public static ColumnRepresentation SortOrder => new ColumnRepresentation(nameof(SortOrder), new IntSqlDataTypeRepresentation());

////                /// <summary>
////                /// Gets the status.
////                /// </summary>
////                /// <value>The status.</value>
////                public static ColumnRepresentation Status => Tables.Handling.Status;

////                /// <summary>
////                /// Gets the table.
////                /// </summary>
////                /// <value>The table.</value>
////                public static TableRepresentation Table => new TableRepresentation(
////                    nameof(CompositeHandlingStatusSortOrder),
////                    new[]
////                    {
////                        SortOrder,
////                        Status,
////                    }.ToDictionary(k => k.Name, v => v));

////                /// <summary>
////                /// Builds the creation script for type With version table.
////                /// </summary>
////                /// <param name="streamName">Name of the stream.</param>
////                /// <returns>Creation script for the type With version table.</returns>
////                public static string BuildCreationScript(
////                    string streamName)
////                {
////                    var result = FormattableString.Invariant(
////                        $@"
////SET ANSI_NULLS ON


////SET QUOTED_IDENTIFIER ON


////CREATE TABLE [{streamName}].[{Table.Name}](
////	[{nameof(SortOrder)}] {SortOrder.DataType.DeclarationInSqlSyntax} IDENTITY(1,1) NOT NULL,
////	[{nameof(Status)}] {Status.DataType.DeclarationInSqlSyntax} UNIQUE NOT NULL,
//// CONSTRAINT [PK_{nameof(CompositeHandlingStatusSortOrder)}] PRIMARY KEY CLUSTERED
////(
////	[{nameof(SortOrder)}] DESC
////)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
////) ON [PRIMARY]

////SET ANSI_PADDING ON

////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Completed}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Unknown}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.AvailableAfterFailure}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.AvailableByDefault}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Unknown}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.AvailableAfterSelfCancellation}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.AvailableAfterExternalCancellation}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Canceled}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Running}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.Failed}')
////INSERT INTO [{streamName}].[{Table.Name}] ([{nameof(Status)}]) VALUES ('{Database.Domain.HandlingStatus.DisabledForStream}')
////			");

////                    return result;
////                }
////            }
////        }
////    }
////}