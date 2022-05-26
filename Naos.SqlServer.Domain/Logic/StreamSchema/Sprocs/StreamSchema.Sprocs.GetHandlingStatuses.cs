// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetHandlingStatuses.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetHandlingStatuses.
            /// </summary>
            public static class GetHandlingStatuses
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetHandlingStatuses);

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
                    /// The internal record identifiers as CSV.
                    /// </summary>
                    InternalRecordIdsCsv,

                    /// <summary>
                    /// The identifier type identifiers as CSV.
                    /// </summary>
                    IdentifierTypeIdsCsv,

                    /// <summary>
                    /// The object type identifiers as CSV.
                    /// </summary>
                    ObjectTypeIdsCsv,

                    /// <summary>
                    /// The string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>).
                    /// </summary>
                    StringIdentifiersXml,

                    /// <summary>
                    /// The record tag identifiers as CSV.
                    /// </summary>
                    TagIdsToMatchCsv,

                    /// <summary>
                    /// The current statuses
                    /// </summary>
                    CurrentStatusesCsv,

                    /// <summary>
                    /// The handling tag identifiers as CSV.
                    /// </summary>
                    HandlingTagIdsToMatchCsv,

                    /// <summary>
                    /// The <see cref="Naos.Database.Domain.TagMatchStrategy"/>.
                    /// </summary>
                    TagMatchStrategy,

                    /// <summary>
                    /// The <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>.
                    /// </summary>
                    VersionMatchStrategy,

                    /// <summary>
                    /// The deprecated identifier event type identifiers as CSV.
                    /// </summary>
                    DeprecatedIdEventTypeIdsCsv,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The internal record identifier to handling status map as XML.
                    /// </summary>
                    RecordIdHandlingStatusXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">Handling concern.</param>
                /// <param name="convertedRecordFilter">Converted form of <see cref="RecordFilter"/>.</param>
                /// <param name="currentHandlingStatuses">Current handling statuses to match.</param>
                /// <param name="handlingTagIdsCsv">Handling tag identifiers to filter on in CSV format.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter,
                    IReadOnlyCollection<HandlingStatus> currentHandlingStatuses,
                    string handlingTagIdsCsv)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetHandlingStatuses)}]");
                    var currentStatusesCsv = currentHandlingStatuses?.Select(_ => _.ToString()).ToCsv();

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.Concern),
                                             Tables.Handling.Concern.SqlDataType,
                                             concern),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.InternalRecordIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.InternalRecordIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.IdentifierTypeIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.IdentifierTypeIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.ObjectTypeIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.ObjectTypeIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.StringIdentifiersXml),
                                             new XmlSqlDataTypeRepresentation(),
                                             convertedRecordFilter.StringIdsToMatchXml),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.TagIdsToMatchCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.TagIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.CurrentStatusesCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             currentStatusesCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.HandlingTagIdsToMatchCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             handlingTagIdsCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.TagMatchStrategy),
                                             new StringSqlDataTypeRepresentation(false, 40),
                                             convertedRecordFilter.TagMatchStrategy.ToString()),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.VersionMatchStrategy),
                                             new StringSqlDataTypeRepresentation(false, 20),
                                             convertedRecordFilter.VersionMatchStrategy.ToString()),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.DeprecatedIdEventTypeIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.DeprecatedIdEventTypeIdsCsv),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.RecordIdHandlingStatusXml),
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
                    const string streamBlockedStatus = "StreamBlockedStatus";

                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";

                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
    @{InputParamName.Concern} {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
 ,  @{InputParamName.InternalRecordIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.IdentifierTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.ObjectTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.StringIdentifiersXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
 ,  @{InputParamName.TagIdsToMatchCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.CurrentStatusesCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.HandlingTagIdsToMatchCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.TagMatchStrategy} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
 ,  @{InputParamName.VersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
 ,  @{InputParamName.DeprecatedIdEventTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{OutputParamName.RecordIdHandlingStatusXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    {RecordFilterLogic.BuildRecordFilterToBuildRecordsToConsiderTable(streamName, recordIdsToConsiderTable, true)}

    DECLARE @{streamBlockedStatus} {Tables.Handling.Status.SqlDataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{streamBlockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.StreamHandlingDisabledConcern}'
    ORDER BY [{Tables.Handling.Id.Name}] DESC

	IF(@{streamBlockedStatus} = '{HandlingStatus.DisabledForStream}')
	BEGIN
        DECLARE @disabledStatusInFilter BIT
		SET @disabledStatusInFilter = 
			CASE
				WHEN (CASE WHEN @{InputParamName.CurrentStatusesCsv} IS NULL THEN 0 ELSE 1 END) = 1 AND 
						EXISTS (SELECT value FROM STRING_SPLIT(@{InputParamName.CurrentStatusesCsv}, ',') WHERE value = '{HandlingStatus.DisabledForStream}') THEN 1 ELSE 0 END

        SELECT @{OutputParamName.RecordIdHandlingStatusXml} = (
            SELECT
                  rids.[{Tables.Record.Id.Name}] AS [@{XmlConversionTool.TagEntryKeyAttributeName}]
                , '{HandlingStatus.DisabledForStream}' AS [@{XmlConversionTool.TagEntryValueAttributeName}]
            FROM @{recordIdsToConsiderTable} rids
            WHERE @disabledStatusInFilter = 1 OR @{InputParamName.CurrentStatusesCsv} IS NULL
            ORDER BY rids.[{Tables.Record.Id.Name}]
            FOR XML PATH ('{XmlConversionTool.TagEntryElementName}'), ROOT('{XmlConversionTool.TagSetElementName}')
        )
    END
	ELSE
	BEGIN
        SELECT @{OutputParamName.RecordIdHandlingStatusXml} = (
            SELECT
                  rids.[{Tables.Record.Id.Name}] AS [@{XmlConversionTool.TagEntryKeyAttributeName}]
                , ISNULL(h.[{Tables.Handling.Status.Name}], '{HandlingStatus.AvailableByDefault}') AS [@{XmlConversionTool.TagEntryValueAttributeName}]
            FROM @{recordIdsToConsiderTable} rids
            LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
                ON rids.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND (h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern} OR h.[{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingDisabledConcern}')
	        LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
	            ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}] AND (h1.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern} OR h1.[{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingDisabledConcern}')
            WHERE h1.[{Tables.Handling.Id.Name}] IS NULL
            AND ((@{InputParamName.CurrentStatusesCsv} IS NULL) OR (h.[{Tables.Handling.Status.Name}] IN (SELECT value FROM STRING_SPLIT(@{InputParamName.CurrentStatusesCsv}, ','))))
            ORDER BY h.[{Tables.Handling.Id.Name}]
            FOR XML PATH ('{XmlConversionTool.TagEntryElementName}'), ROOT('{XmlConversionTool.TagSetElementName}')
        )
	END
END");

                    return result;
                }
            }
        }
    }
}
