// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Funcs.GetStatusSortOrderTableVariable.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// User defined functions.
        /// </summary>
        public static partial class Funcs
        {
            /// <summary>
            /// Class GetTagsTableVariableFromTagsXml.
            /// </summary>
            public static class GetStatusSortOrderTableVariable
            {
                /// <summary>
                /// Gets the name of the function.
                /// </summary>
                /// <value>The name of the function.</value>
                public static string Name => nameof(GetStatusSortOrderTableVariable);

                /// <summary>
                /// Output table column names.
                /// </summary>
                public enum OutputColumnName
                {
                    /// <summary>
                    /// The sort order.
                    /// </summary>
                    SortOrder,

                    /// <summary>
                    /// The status.
                    /// </summary>
                    Status,
                }

                /// <summary>
                /// Builds the creation script.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>The creation script.</returns>
                public static string BuildCreationScript(
                    string streamName)
                {
                    return FormattableString.Invariant(
                        $@"
CREATE FUNCTION [{streamName}].[{GetStatusSortOrderTableVariable.Name}]()
    RETURNS TABLE
    AS
    RETURN 
    SELECT [{OutputColumnName.SortOrder}], [{OutputColumnName.Status}] FROM 
(VALUES
	  (0, '{HandlingStatus.Completed}')
	, (1, '{HandlingStatus.None}')
	, (2, '{HandlingStatus.RetryFailed}')
	, (3, '{HandlingStatus.Requested}')
	, (4, '{HandlingStatus.Unknown}')
	, (5, '{HandlingStatus.SelfCanceledRunning}')
	, (6, '{HandlingStatus.CanceledRunning}')
	, (7, '{HandlingStatus.Canceled}')
	, (8, '{HandlingStatus.Running}')
	, (9, '{HandlingStatus.Failed}')
	, (10, '{HandlingStatus.Blocked}')
) x([{OutputColumnName.SortOrder}], [{OutputColumnName.Status}])
");
                }
            }
        }
    }
}