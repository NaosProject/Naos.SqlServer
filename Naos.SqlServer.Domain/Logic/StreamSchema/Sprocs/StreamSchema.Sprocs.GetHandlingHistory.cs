// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetHandlingHistory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetHandlingHistory.
            /// </summary>
            public static class GetHandlingHistory
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetHandlingHistory);

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
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The handling entries as XML.
                    /// </summary>
                    EntriesXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">Handling concern.</param>
                /// <param name="internalRecordId">The internal record identifier.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    long internalRecordId)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetHandlingHistory)}]");
                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.Concern),
                                             Tables.Handling.Concern.SqlDataType,
                                             concern),
                                         new InputParameterDefinition<long>(
                                             nameof(InputParamName.InternalRecordId),
                                             Tables.Record.Id.SqlDataType,
                                             internalRecordId),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.EntriesXml),
                                             new XmlSqlDataTypeRepresentation()),
                                     };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the name of the put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
    @{InputParamName.Concern} {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
 ,  @{InputParamName.InternalRecordId} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
 ,  @{OutputParamName.EntriesXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    SELECT @{OutputParamName.EntriesXml} = (
        SELECT
                h.[{Tables.Handling.Id.Name}]
              , h.[{Tables.Handling.RecordId.Name}]
              , h.[{Tables.Handling.Concern.Name}]
              , h.[{Tables.Handling.Status.Name}]
              , h.[{Tables.Handling.Details.Name}]
              , h.[{Tables.Handling.RecordCreatedUtc.Name}]
              , (SELECT STRING_AGG({Tables.HandlingTag.TagId.Name}, ',') FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] WHERE [{Tables.HandlingTag.HandlingId.Name}] = h.[{Tables.Handling.Id.Name}]) AS [{nameof(XmlConversionTool.SerializableEntrySetItem.TagIdsCsv)}]
        FROM [{streamName}].[{Tables.Handling.Table.Name}] h
        WHERE  h.[{Tables.Handling.RecordId.Name}] = @{InputParamName.InternalRecordId}
           AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
        ORDER BY h.[{Tables.Handling.Id.Name}]
        FOR XML PATH ('{XmlConversionTool.EntryElementName}'), ROOT('{XmlConversionTool.EntrySetElementName}')
    )
END");

                    return result;
                }
            }
        }
    }
}
