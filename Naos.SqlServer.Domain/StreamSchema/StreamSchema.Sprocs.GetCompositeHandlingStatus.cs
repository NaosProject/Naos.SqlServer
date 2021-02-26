// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetCompositeHandlingStatus.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        public static partial class Sprocs
        {
            /// <summary>
            /// Container for stored procedure.
            /// </summary>
            public static class GetCompositeHandlingStatus
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                /// <value>The name of the stored procedure.</value>
                public static string Name => nameof(GetCompositeHandlingStatus);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The concern.
                    /// </summary>
                    Concern,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsCsv,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The status.
                    /// </summary>
                    Status,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">The handling concern.</param>
                /// <param name="tagIdsCsv">The optional tag identifiers in CSV.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string tagIdsCsv = null)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].[{nameof(GetCompositeHandlingStatus)}]");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsCsv), Tables.Record.TagIdsCsv.DataType, tagIdsCsv),
                                         new SqlOutputParameterRepresentation<HandlingStatus>(nameof(OutputParamName.Status), Tables.Handling.Status.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Name of the put stored procedure.</returns>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    const string blockedStatus = "BlockedStatus";
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetCompositeHandlingStatus.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax}
, @{OutputParamName.Status} AS {Tables.Handling.Status.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{blockedStatus} {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{blockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingConcern}'
    IF (@{blockedStatus} = '{HandlingStatus.Blocked}')
    BEGIN
        -- Blocked trumps everything...
        SET @{OutputParamName.Status} = '{HandlingStatus.Blocked}'
    END
    ELSE
        BEGIN
        SELECT TOP 1
            @{OutputParamName.Status} = h1.[{Tables.Handling.Status.Name}]
        FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] ht
        INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
            ON h1.[{Tables.Handling.Id.Name}] = ht.[{Tables.HandlingTag.HandlingId.Name}]
        LEFT OUTER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h2
            ON h1.[{Tables.Handling.RecordId.Name}] = h2.[{Tables.Handling.RecordId.Name}] AND h1.[{Tables.Handling.Id.Name}] < h2.[{Tables.Handling.Id.Name}]
        LEFT JOIN [{streamName}].[{Tables.CompositeHandlingStatusSortOrder.Table.Name}] s
            ON s.[{Tables.CompositeHandlingStatusSortOrder.Status.Name}] = h1.[{Tables.Handling.Status.Name}]
        WHERE h2.[{Tables.Handling.Id.Name}] IS NULL AND h1.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
            AND ht.[{Tables.HandlingTag.TagId.Name}] IN (SELECT value AS [{Tables.Tag.Id.Name}] FROM STRING_SPLIT(@{InputParamName.TagIdsCsv}, ','))
        ORDER BY s.[{Tables.CompositeHandlingStatusSortOrder.SortOrder.Name}] DESC

        IF (@{OutputParamName.Status} IS NULL)
        BEGIN
            SET @{OutputParamName.Status} = '{HandlingStatus.None}'
        END
    END
END");
                    return result;
                }
            }
        }
    }
}
