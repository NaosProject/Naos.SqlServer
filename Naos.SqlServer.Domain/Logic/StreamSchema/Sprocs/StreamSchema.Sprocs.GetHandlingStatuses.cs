// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetHandlingStatuses.cs" company="Naos Project">
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
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagsIdsCsv,

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
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetHandlingStatuses)}]");
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
                                             nameof(InputParamName.TagsIdsCsv),
                                             new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant),
                                             convertedRecordFilter.TagIdsCsv),
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
                    const string identifierTypesTable = "IdTypeIdentifiersTable";
                    const string objectTypesTable = "ObjectTypeIdentifiersTable";
                    const string stringSerializedIdsTable = "StringSerializedIdentifiersTable";
                    const string tagIdsTable = "TagIdsTable";
                    const string deprecatedTypesTable = "DeprecatedIdEventIdsTable";

                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{Name}](
    @{InputParamName.Concern} {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
 ,  @{InputParamName.InternalRecordIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.IdentifierTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.ObjectTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.StringIdentifiersXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
 ,  @{InputParamName.TagsIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.TagMatchStrategy} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
 ,  @{InputParamName.VersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
 ,  @{InputParamName.DeprecatedIdEventTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{OutputParamName.RecordIdHandlingStatusXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
  )
AS
BEGIN
    -- START RECORD FILTER QUERYING
    DECLARE @{recordIdsToConsiderTable} TABLE([{Tables.Record.Id.Name}] {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{recordIdsToConsiderTable} ([{Tables.Record.Id.Name}])
    SELECT value FROM STRING_SPLIT(@{InputParamName.InternalRecordIdsCsv}, ',')

    DECLARE @{identifierTypesTable} TABLE([{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{identifierTypesTable} ([{Tables.TypeWithVersion.Id.Name}])
    SELECT value FROM STRING_SPLIT(@{InputParamName.IdentifierTypeIdsCsv}, ',')

    DECLARE @{objectTypesTable} TABLE([{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{objectTypesTable} ([{Tables.TypeWithVersion.Id.Name}])
    SELECT value FROM STRING_SPLIT(@{InputParamName.ObjectTypeIdsCsv}, ',')

    DECLARE @{stringSerializedIdsTable} TABLE([{Tables.Record.StringSerializedId.Name}] {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} NOT NULL, [{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{stringSerializedIdsTable} ([{Tables.Record.StringSerializedId.Name}], [{Tables.TypeWithVersion.Id.Name}])
    SELECT 
         [{Tables.Tag.TagKey.Name}]
	   , [{Tables.Tag.TagValue.Name}]
    FROM [{streamName}].[{Funcs.GetTagsTableVariableFromTagsXml.Name}](@{InputParamName.StringIdentifiersXml}) 

    DECLARE @{tagIdsTable} TABLE([{Tables.Tag.Id.Name}] {Tables.Tag.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{tagIdsTable} ([{Tables.Tag.Id.Name}])
    SELECT value FROM STRING_SPLIT(@{InputParamName.TagsIdsCsv}, ',')

    DECLARE @{deprecatedTypesTable} TABLE([{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    INSERT INTO @{deprecatedTypesTable} ([{Tables.TypeWithVersion.Id.Name}])
    SELECT value FROM STRING_SPLIT(@{InputParamName.DeprecatedIdEventTypeIdsCsv}, ',')

    INSERT INTO @{recordIdsToConsiderTable}
    SELECT DISTINCT r.[{Tables.Record.Id.Name}]
	FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
    LEFT JOIN @{recordIdsToConsiderTable} ir ON
        r.[{Tables.Record.Id.Name}] =  ir.[{Tables.Record.Id.Name}]
    LEFT JOIN @{identifierTypesTable} itwith ON
        r.[{Tables.Record.IdentifierTypeWithVersionId.Name}] = itwith.[{Tables.TypeWithVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
    LEFT JOIN @{identifierTypesTable} itwithout ON
        r.[{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = itwithout.[{Tables.TypeWithoutVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
    LEFT JOIN @{objectTypesTable} otwith ON
        r.[{Tables.Record.ObjectTypeWithVersionId.Name}] = otwith.[{Tables.TypeWithVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
    LEFT JOIN @{objectTypesTable} otwithout ON
        r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = otwithout.[{Tables.TypeWithoutVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
    WHERE
        r.[{Tables.Record.Id.Name}] IS NOT NULL

	IF ((EXISTS (SELECT TOP 1 [{Tables.Tag.Id.Name}] FROM @{tagIdsTable})) AND @TagMatchStrategy = '{TagMatchStrategy.RecordContainsAllQueryTags}')
	BEGIN
        DECLARE @TagCount INT
        SELECT @TagCount = COUNT([{Tables.Tag.Id.Name}]) FROM @{tagIdsTable}
        INSERT INTO @{recordIdsToConsiderTable}
        SELECT DISTINCT rt.[{Tables.RecordTag.RecordId.Name}] AS [{Tables.Record.Id.Name}]
        FROM [{streamName}].[{Tables.RecordTag.Table.Name}] rt WITH (NOLOCK)
        JOIN @{tagIdsTable} tids ON
            tids.[{Tables.Tag.Id.Name}] = rt.[{Tables.RecordTag.RecordId.Name}]
        WHERE rt.[{Tables.RecordTag.RecordId.Name}] NOT IN (SELECT [{Tables.Record.Id.Name}] FROM @{recordIdsToConsiderTable})
        GROUP BY rt.[{Tables.RecordTag.RecordId.Name}]
        HAVING COUNT(rt.[{Tables.RecordTag.RecordId.Name}]) = @TagCount
    END

	IF ((EXISTS (SELECT TOP 1 [{Tables.TypeWithVersion.Id.Name}] FROM @{deprecatedTypesTable})))
	BEGIN
        DELETE FROM @{recordIdsToConsiderTable}
        WHERE [{Tables.Record.Id.Name}] IN
        (
            SELECT DISTINCT r.[{Tables.Record.Id.Name}]
            FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
		    LEFT JOIN [{streamName}].[{Tables.Record.Table.Name}] r1 WITH (NOLOCK) -- the most recent record type is the deprecated
		        ON r.[{Tables.Record.StringSerializedId.Name}] = r1.[{Tables.Record.StringSerializedId.Name}] AND r.[{Tables.Record.Id.Name}] < r1.[{Tables.Record.Id.Name}]
            LEFT JOIN @{deprecatedTypesTable} dtwith ON
                r.[{Tables.Record.ObjectTypeWithVersionId.Name}] = dtwith.[{Tables.TypeWithVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
            LEFT JOIN @{deprecatedTypesTable} dtwithout ON
                r.[{Tables.Record.ObjectTypeWithoutVersionId.Name}] = dtwithout.[{Tables.TypeWithoutVersion.Id.Name}] AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
            WHERE
                    r1.[{Tables.Record.Id.Name}] IS NULL
                AND (
                       (dtwith.[{Tables.Record.Id.Name}] IS NOT NULL AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}')
                       OR
                       (dtwithout.[{Tables.Record.Id.Name}] IS NOT NULL AND @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}')
                     )
        )
    END
    -- END RECORD FILTER QUERYING

    DECLARE @{streamBlockedStatus} {Tables.Handling.Status.SqlDataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{streamBlockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.StreamHandlingDisabledConcern}'

	IF(@{streamBlockedStatus} = '{HandlingStatus.DisabledForStream}')
	BEGIN
        SELECT @{OutputParamName.RecordIdHandlingStatusXml} = (
            SELECT
                  rids.[{Tables.Record.Id.Name}] AS [@{TagConversionTool.TagEntryKeyAttributeName}]
                , '{HandlingStatus.DisabledForStream}' AS [@{TagConversionTool.TagEntryValueAttributeName}]
            FROM @{recordIdsToConsiderTable} rids
            ORDER BY rids.[{Tables.Record.Id.Name}]
            FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}')
        )
    END
	ELSE
	BEGIN
        SELECT @{OutputParamName.RecordIdHandlingStatusXml} = (
            SELECT
                  rids.[{Tables.Record.Id.Name}] AS [@{TagConversionTool.TagEntryKeyAttributeName}]
                , ISNULL(h.[{Tables.Handling.Status.Name}], '{HandlingStatus.AvailableByDefault}') AS [@{TagConversionTool.TagEntryValueAttributeName}]
            FROM @{recordIdsToConsiderTable} rids
            LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
                ON rids.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND (h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern} OR h.[{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingDisabledConcern}')
	        LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
	            ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}] AND (h1.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern} OR h1.[{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingDisabledConcern}')
            WHERE h1.[{Tables.Handling.Id.Name}] IS NULL
            ORDER BY h.[{Tables.Handling.Id.Name}]
            FOR XML PATH ('{TagConversionTool.TagEntryElementName}'), ROOT('{TagConversionTool.TagSetElementName}')
        )
	END
END");

                    return result;
                }
            }
        }
    }
}
