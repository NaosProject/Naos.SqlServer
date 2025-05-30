﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetDistinctStringSerializedIds.cs" company="Naos Project">
//     Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Class StreamSchema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Class Sprocs.
        /// </summary>
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: StandardGetDistinctStringSerializedIds.
            /// </summary>
            public static class GetDistinctStringSerializedIds
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(GetDistinctStringSerializedIds);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
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
                    /// The string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy" />).
                    /// </summary>
                    StringIdentifiersXml,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsToMatchCsv,

                    /// <summary>
                    /// The <see cref="RecordFilter.TagMatchStrategy" />.
                    /// </summary>
                    TagMatchStrategy,

                    /// <summary>
                    /// The <see cref="RecordFilter.VersionMatchStrategy" />.
                    /// </summary>
                    VersionMatchStrategy,

                    /// <summary>
                    /// The deprecated identifier event type identifiers as CSV.
                    /// </summary>
                    DeprecatedIdEventTypeIdsCsv,

                    /// <summary>
                    /// The <see cref="RecordsToFilterCriteria.RecordsToFilterSelectionStrategy"/>.
                    /// </summary>
                    RecordsToFilterSelectionStrategy,

                    /// <summary>
                    /// The <see cref="RecordsToFilterCriteria.VersionMatchStrategy"/>.
                    /// </summary>
                    RecordsToFilterVersionMatchStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The distinct string identifiers in XML of the discovered matching records.
                    /// </summary>
                    StringIdentifiersOutputXml,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="convertedRecordFilter">Converted form of <see cref="RecordFilter" />.</param>
                /// <param name="recordsToFilterCriteria">Specifies how to determine the records that are input into a <see cref="RecordFilter"/>.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter,
                    RecordsToFilterCriteria recordsToFilterCriteria)
                {
                    var sprocName = Invariant($"[{streamName}].[{nameof(GetDistinctStringSerializedIds)}]");

                    var parameters = new List<ParameterDefinitionBase>
                    {
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
                        new InputParameterDefinition<string>(
                            nameof(InputParamName.RecordsToFilterSelectionStrategy),
                            new StringSqlDataTypeRepresentation(false, 50),
                            recordsToFilterCriteria.RecordsToFilterSelectionStrategy.ToString()),
                        new InputParameterDefinition<string>(
                            nameof(InputParamName.RecordsToFilterVersionMatchStrategy),
                            new StringSqlDataTypeRepresentation(false, 20),
                            recordsToFilterCriteria.VersionMatchStrategy.ToString()),
                        new OutputParameterDefinition<string>(
                            nameof(OutputParamName.StringIdentifiersOutputXml),
                            new XmlSqlDataTypeRepresentation()),
                    };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put sproc.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForGet", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sproc", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";

                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";

                    const string resultTableName = "ResultsTable";

                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetDistinctStringSerializedIds.Name}](
    @{InputParamName.InternalRecordIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.IdentifierTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.ObjectTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.StringIdentifiersXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
 ,  @{InputParamName.TagIdsToMatchCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.TagMatchStrategy} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
 ,  @{InputParamName.VersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
 ,  @{InputParamName.DeprecatedIdEventTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
 ,  @{InputParamName.RecordsToFilterSelectionStrategy} {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax} = '{RecordsToFilterSelectionStrategy.All}' -- This parameter was introduced after the stored procedure shipped, so we are defaulting here for backward compatibility.  Existing clients that have not taken package updates will get the default value for this parameter.
 ,  @{InputParamName.RecordsToFilterVersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax} = '{VersionMatchStrategy.Any}' -- This parameter was introduced after the stored procedure shipped, so we are defaulting here for backward compatibility.  Existing clients that have not taken package updates will get the default value for this parameter.
 ,  @{OutputParamName.StringIdentifiersOutputXml} AS {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{resultTableName} TABLE ([{Tables.Record.StringSerializedId.Name}] {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} NULL, [{Tables.TypeWithVersion.Id.Name}] {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)

    {RecordFilterLogic.BuildRecordFilterToBuildRecordsToConsiderTable(streamName, recordIdsToConsiderTable, includeHandlingTags: false, includeRecordsToFilterCriteria: true)}

    INSERT INTO @{resultTableName} ([{Tables.TypeWithVersion.Id.Name}], [{Tables.Record.StringSerializedId.Name}])
    SELECT DISTINCT
          r.[{Tables.Record.IdentifierTypeWithVersionId.Name}]
        , r.[{Tables.Record.StringSerializedId.Name}]
	FROM [{streamName}].[{Tables.Record.Table.Name}] r WITH (NOLOCK)
    INNER JOIN @{recordIdsToConsiderTable} rtc ON r.[{Tables.Record.Id.Name}] = rtc.[{Tables.Record.Id.Name}]
    LEFT OUTER JOIN [{streamName}].[{Tables.Record.Table.Name}] r1 WITH (NOLOCK)
        ON r.[{Tables.Record.StringSerializedId.Name}] = r1.[{Tables.Record.StringSerializedId.Name}] AND r.[{Tables.Record.Id.Name}] < r1.[{Tables.Record.Id.Name}] AND r1.[{Tables.Record.Id.Name}] = rtc.[{Tables.Record.Id.Name}]
	WHERE
          r1.[{Tables.Record.Id.Name}] IS NULL
      AND rtc.[{Tables.Record.Id.Name}] IS NOT NULL

    SELECT @{OutputParamName.StringIdentifiersOutputXml} = (SELECT
          e.[{Tables.TypeWithVersion.Id.Name}] AS [@{XmlConversionTool.TagEntryKeyAttributeName}]
        , ISNULL([{streamName}].[{Funcs.AdjustForGetStringSerializedId.Name}](e.[{Tables.Record.StringSerializedId.Name}]), '{XmlConversionTool.NullCanaryValue}') AS [@{XmlConversionTool.TagEntryValueAttributeName}]
    FROM @{resultTableName} e
    FOR XML PATH ('{XmlConversionTool.TagEntryElementName}'), ROOT('{XmlConversionTool.TagSetElementName}'))
END

			");

                    return result;
                }
            }
        }
    }
}
