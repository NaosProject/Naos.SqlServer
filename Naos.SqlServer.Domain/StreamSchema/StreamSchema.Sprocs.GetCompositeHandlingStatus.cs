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
                    /// The tag identifiers as XML.
                    /// </summary>
                    TagIdsXml,
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
                /// <param name="tagIdsXml">The optional tag identifiers in xml.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string tagIdsXml = null)
                {
                    var sprocName = FormattableString.Invariant($"[{streamName}].[{nameof(GetCompositeHandlingStatus)}]");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.HandlingHistory.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsXml), new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxLengthConstant), tagIdsXml),
                                         new SqlOutputParameterRepresentation<HandlingStatus>(nameof(OutputParamName.Status), Tables.HandlingHistory.Status.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <returns>Name of the put stored procedure.</returns>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName)
                {
                    return Invariant(
                        $@"
CREATE PROCEDURE [{streamName}].[{GetCompositeHandlingStatus.Name}](
  @{InputParamName.Concern} AS {Tables.HandlingHistory.Status.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TagIdsXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.Status} AS {Tables.HandlingHistory.Status.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    SELECT TOP 1
        @{OutputParamName.Status} = h1.[{Tables.HandlingHistory.Status.Name}]
    FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] ht
    INNER JOIN [{streamName}].[{Tables.HandlingHistory.Table.Name}] h1
        ON h1.[{Tables.HandlingHistory.Id.Name}] = ht.[{Tables.HandlingTag.HandlingId.Name}]
    LEFT OUTER JOIN [{streamName}].[{Tables.HandlingHistory.Table.Name}] h2
        ON h1.[{Tables.HandlingHistory.RecordId.Name}] = h2.[{Tables.HandlingHistory.RecordId.Name}] AND h1.[{Tables.HandlingHistory.Id.Name}] < h2.[{Tables.HandlingHistory.Id.Name}]
    LEFT JOIN [{streamName}].[{Funcs.GetStatusSortOrderTableVariable.Name}]() s
        ON s.[{Funcs.GetStatusSortOrderTableVariable.OutputColumnName.Status}] = h1.[{Tables.HandlingHistory.Status.Name}]
    WHERE h2.[{Tables.HandlingHistory.Id.Name}] IS NULL AND h1.[{Tables.HandlingHistory.Concern.Name}] = @{InputParamName.Concern}
        AND ht.[{Tables.HandlingTag.TagId.Name}] IN (SELECT [{Tables.Tag.TagValue.Name}] FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagIdsXml.Name}](@{InputParamName.TagIdsXml}))
    ORDER BY s.[{Funcs.GetStatusSortOrderTableVariable.OutputColumnName.SortOrder}] DESC

    IF (@{OutputParamName.Status} IS NULL)
    BEGIN
        SET @{OutputParamName.Status} = '{HandlingStatus.None}'
    END
END");
                }
            }
        }
    }
}
