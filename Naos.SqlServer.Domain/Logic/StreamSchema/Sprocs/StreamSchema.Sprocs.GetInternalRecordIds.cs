// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetInternalRecordIds.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetInternalRecordIds.
            /// </summary>
            public static class GetInternalRecordIds
            {
                /// <summary>
                /// Gets the name of the stored procedure.
                /// </summary>
                public static string Name => nameof(GetInternalRecordIds);

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
                    /// The string identifiers to match as XML (key is string identifier and value is the appropriate type identifier per the <see cref="OBeautifulCode.Type.VersionMatchStrategy"/>).
                    /// </summary>
                    StringIdentifiersXml,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsToMatchCsv,

                    /// <summary>
                    /// The <see cref="RecordFilter.TagMatchStrategy"/>.
                    /// </summary>
                    TagMatchStrategy,

                    /// <summary>
                    /// The <see cref="RecordFilter.VersionMatchStrategy"/>.
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
                    /// The internal record identifiers as CSV.
                    /// </summary>
                    InternalRecordIdsCsvOutput,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="convertedRecordFilter">Converted form of <see cref="RecordFilter"/>.</param>
                /// <param name="recordsToFilterCriteria">Specifies how to determine the records that are input into a <see cref="RecordFilter"/>.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter,
                    RecordsToFilterCriteria recordsToFilterCriteria)
                {
                    var sprocName = Invariant($"[{streamName}].{Name}");
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
                            nameof(OutputParamName.InternalRecordIdsCsvOutput),
                            new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant)),
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
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";

                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";

                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetInternalRecordIds.Name}](
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
 ,  @FilteredRecordsSelectionStrategy {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax} = 'IGNORE' -- This parameter was introduced after the stored procedure shipped, but then it was replaced by @{InputParamName.RecordsToFilterSelectionStrategy}.  None of the deployed clients used any value other than 'All'.  This parameter exists so that those deployed clients don't throw an exception because the parameter is missing.
 ,  @{OutputParamName.InternalRecordIdsCsvOutput} AS {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    {RecordFilterLogic.BuildRecordFilterToBuildRecordsToConsiderTable(streamName, recordIdsToConsiderTable, includeHandlingTags: false, includeRecordsToFilterCriteria: true)}

    SELECT @{OutputParamName.InternalRecordIdsCsvOutput} = STRING_AGG(CONVERT({new StringSqlDataTypeRepresentation(true, StringSqlDataTypeRepresentation.MaxUnicodeLengthConstant).DeclarationInSqlSyntax}, [{Tables.Record.Id.Name}]), ',') FROM @{recordIdsToConsiderTable}
END");

                    return result;
                }
            }
        }
    }
}