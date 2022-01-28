// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.GetLatestRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetLatestRecordById.
            /// </summary>
            public static class GetLatestRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(GetLatestRecord);

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

                    /// <summary>
                    /// Should include the payload.
                    /// </summary>
                    IncludePayload,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,

                    /// <summary>
                    /// The serialization kind.
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The identifier assembly qualified name with version.
                    /// </summary>
                    IdentifierTypeWithVersionId,

                    /// <summary>
                    /// The object assembly qualified name with version.
                    /// </summary>
                    ObjectTypeWithVersionId,

                    /// <summary>
                    /// The serialized identifier string.
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    StringSerializedObject,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    BinarySerializedObject,

                    /// <summary>
                    /// The record's date and time.
                    /// </summary>
                    RecordDateTime,

                    /// <summary>
                    /// The object's date and time if it was a <see cref="IHaveTimestampUtc"/>.
                    /// </summary>
                    ObjectDateTime,

                    /// <summary>
                    /// Any tag ids as CSV.
                    /// </summary>
                    TagIdsCsv,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="convertedRecordFilter">Converted form of <see cref="RecordFilter"/>.</param>
                /// <param name="recordNotFoundStrategy">The existing record not encountered strategy.</param>
                /// <param name="streamRecordItemsToInclude">The items to include.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter,
                    RecordNotFoundStrategy recordNotFoundStrategy,
                    StreamRecordItemsToInclude streamRecordItemsToInclude)
                {
                    streamRecordItemsToInclude
                       .MustForArg(nameof(streamRecordItemsToInclude))
                       .BeElementIn(
                            new[]
                            {
                                StreamRecordItemsToInclude.MetadataAndPayload,
                                StreamRecordItemsToInclude.MetadataOnly,
                            });

                    var sprocName = Invariant($"[{streamName}].[{nameof(GetLatestRecord)}]");

                    var parameters = new List<ParameterDefinitionBase>()
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
                                         new InputParameterDefinition<int>(
                                             nameof(InputParamName.IncludePayload),
                                             new IntSqlDataTypeRepresentation(),
                                             streamRecordItemsToInclude == StreamRecordItemsToInclude.MetadataAndPayload ? 1 : 0),
                                         new OutputParameterDefinition<long>(nameof(OutputParamName.InternalRecordId), Tables.Record.Id.SqlDataType),
                                         new OutputParameterDefinition<int>(
                                             nameof(OutputParamName.SerializerRepresentationId),
                                             Tables.SerializerRepresentation.Id.SqlDataType),
                                         new OutputParameterDefinition<int>(
                                             nameof(OutputParamName.IdentifierTypeWithVersionId),
                                             Tables.TypeWithVersion.Id.SqlDataType),
                                         new OutputParameterDefinition<int>(
                                             nameof(OutputParamName.ObjectTypeWithVersionId),
                                             Tables.TypeWithVersion.Id.SqlDataType),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.StringSerializedId),
                                             Tables.Record.StringSerializedId.SqlDataType),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.StringSerializedObject),
                                             Tables.Record.StringSerializedObject.SqlDataType),
                                         new OutputParameterDefinition<byte[]>(
                                             nameof(OutputParamName.BinarySerializedObject),
                                             Tables.Record.BinarySerializedObject.SqlDataType),
                                         new OutputParameterDefinition<DateTime>(
                                             nameof(OutputParamName.RecordDateTime),
                                             Tables.Record.RecordCreatedUtc.SqlDataType),
                                         new OutputParameterDefinition<DateTime?>(
                                             nameof(OutputParamName.ObjectDateTime),
                                             Tables.Record.ObjectDateTimeUtc.SqlDataType),
                                         new OutputParameterDefinition<string>(
                                             nameof(OutputParamName.TagIdsCsv),
                                             Tables.Record.TagIdsCsv.SqlDataType),
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
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1702:CompoundWordsShouldBeCasedCorrectly",
                    MessageId = "ForGet",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Sproc",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public static string BuildCreationScript(
                    string streamName,
                    bool asAlter = false)
                {
                    var createOrModify = asAlter ? "ALTER" : "CREATE";

                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";
                    const string mostRecentMatchingRecordId = "MostRecentMatchingRecordId";

                    var result = Invariant(
                        $@"
{createOrModify} PROCEDURE [{streamName}].[{GetLatestRecord.Name}](
  @{InputParamName.InternalRecordIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.StringIdentifiersXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.TagsIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.TagMatchStrategy} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
, @{InputParamName.VersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
, @{InputParamName.DeprecatedIdEventTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.IncludePayload} {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.BinarySerializedObject} AS {Tables.Record.BinarySerializedObject.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    {RecordFilterLogic.BuildRecordFilterToBuildRecordsToConsiderTable(streamName, recordIdsToConsiderTable)}

    DECLARE @{mostRecentMatchingRecordId} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
    SELECT @{mostRecentMatchingRecordId} = (SELECT TOP 1 [{Tables.Record.Id.Name}]
         FROM @{recordIdsToConsiderTable}
         ORDER BY [{Tables.Record.Id.Name}] DESC)

    SELECT TOP 1
	   @{OutputParamName.SerializerRepresentationId} = [{Tables.Record.SerializerRepresentationId.Name}]
	 , @{OutputParamName.IdentifierTypeWithVersionId} = [{Tables.Record.IdentifierTypeWithVersionId.Name}]
	 , @{OutputParamName.ObjectTypeWithVersionId} = [{Tables.Record.ObjectTypeWithVersionId.Name}]
     , @{OutputParamName.StringSerializedId} = [{Tables.Record.StringSerializedId.Name}]
     , @{OutputParamName.StringSerializedObject} = (
            CASE @{InputParamName.IncludePayload}
                WHEN 1 THEN [{Tables.Record.StringSerializedObject.Name}]
                WHEN 0 THEN NULL
                ELSE CONVERT({new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}, '@{InputParamName.IncludePayload} is used as a bit flag and should only be 1 or 0.')
            END)
	 , @{OutputParamName.BinarySerializedObject} = (
            CASE @{InputParamName.IncludePayload}
                WHEN 1 THEN [{Tables.Record.BinarySerializedObject.Name}]
                WHEN 0 THEN NULL
                ELSE CONVERT({new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}, '@{InputParamName.IncludePayload} is used as a bit flag and should only be 1 or 0.')
            END)
	 , @{OutputParamName.InternalRecordId} = [{Tables.Record.Id.Name}]
	 , @{OutputParamName.TagIdsCsv} = [{Tables.Record.TagIdsCsv.Name}]
	 , @{OutputParamName.RecordDateTime} = [{Tables.Record.RecordCreatedUtc.Name}]
	 , @{OutputParamName.ObjectDateTime} = [{Tables.Record.ObjectDateTimeUtc.Name}]
	FROM [{streamName}].[{Tables.Record.Table.Name}]
	WHERE [{Tables.Record.Id.Name}] = @{mostRecentMatchingRecordId}

    IF (@{OutputParamName.InternalRecordId} IS NULL)
    BEGIN
        SET @{OutputParamName.SerializerRepresentationId} = {Tables.SerializerRepresentation.NullId}
	    SET @{OutputParamName.IdentifierTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
	    SET @{OutputParamName.ObjectTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
	    SET @{OutputParamName.InternalRecordId} = {Tables.Record.NullId}
	    SET @{OutputParamName.ObjectDateTime} = NULL
	    SET @{OutputParamName.TagIdsCsv} = NULL
	    SET @{OutputParamName.RecordDateTime} = GETUTCDATE()
    END
END

			");

                    return result;
                }
            }
        }
    }
}
