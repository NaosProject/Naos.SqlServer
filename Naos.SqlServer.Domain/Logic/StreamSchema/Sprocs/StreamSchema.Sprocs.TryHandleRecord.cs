// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.TryHandleRecord.cs" company="Naos Project">
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
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: TryHandleRecord.
            /// </summary>
            public static class TryHandleRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(TryHandleRecord);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum InputParamName
                {
                    /// <summary>
                    /// The concern.
                    /// </summary>
                    Concern,

                    /// <summary>
                    /// The details.
                    /// </summary>
                    Details,

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
                    /// The order record strategy.
                    /// </summary>
                    OrderRecordsBy,

                    /// <summary>
                    /// The tag identifiers as CSV.
                    /// </summary>
                    TagIdsForEntryCsv,

                    /// <summary>
                    /// Inherit the record's tags in handling.
                    /// </summary>
                    InheritRecordTags,

                    /// <summary>
                    /// The minimum internal record identifier.
                    /// </summary>
                    MinimumInternalRecordId,

                    /// <summary>
                    /// Should include the payload.
                    /// </summary>
                    IncludePayload,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
                public enum OutputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,

                    /// <summary>
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,

                    /// <summary>
                    /// The serialization configuration assembly qualified name without version.
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
                    /// The serialized object identifier.
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    StringSerializedObject,

                    /// <summary>
                    /// The serialized object as bytes.
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
                    /// Any tags returned as CSV.
                    /// </summary>
                    TagIdsCsv,

                    /// <summary>
                    /// An indicator of whether or not to handle.
                    /// </summary>
                    ShouldHandle,

                    /// <summary>
                    /// Whether or not handling is blocked.
                    /// </summary>
                    IsBlocked,
                }

                /// <summary>
                /// Builds the execute stored procedure op.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="concern">The concern.</param>
                /// <param name="details">The details.</param>
                /// <param name="convertedRecordFilter">Converted form of <see cref="RecordFilter"/>.</param>
                /// <param name="tagIdsForEntryCsv">The tag identifiers of tags to add to new entries.</param>
                /// <param name="orderRecordsBy">The order records strategy.</param>
                /// <param name="minimumInternalRecordId">The optional minimum internal record identifier, null for default.</param>
                /// <param name="inheritRecordTags">The tags on the record should also be on the handling entry.</param>
                /// <param name="streamRecordItemsToInclude">The items to include.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string details,
                    RecordFilterConvertedForStoredProcedure convertedRecordFilter,
                    string tagIdsForEntryCsv,
                    OrderRecordsBy orderRecordsBy,
                    long? minimumInternalRecordId,
                    bool inheritRecordTags,
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

                    var sprocName = Invariant($"[{streamName}].{nameof(TryHandleRecord)}");

                    var parameters = new List<ParameterDefinitionBase>()
                                     {
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.Concern),
                                             Tables.Handling.Concern.SqlDataType,
                                             concern),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.Details),
                                             Tables.Handling.Details.SqlDataType,
                                             details),
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
                                             nameof(InputParamName.TagIdsForEntryCsv),
                                             Tables.Record.TagIdsCsv.SqlDataType,
                                             tagIdsForEntryCsv),
                                         new InputParameterDefinition<string>(
                                             nameof(InputParamName.OrderRecordsBy),
                                             new StringSqlDataTypeRepresentation(false, 50),
                                             orderRecordsBy.ToString()),
                                         new InputParameterDefinition<int>(
                                             nameof(InputParamName.InheritRecordTags),
                                             new IntSqlDataTypeRepresentation(),
                                             inheritRecordTags ? 1 : 0),
                                         new InputParameterDefinition<long?>(
                                             nameof(InputParamName.MinimumInternalRecordId),
                                             Tables.Record.Id.SqlDataType,
                                             minimumInternalRecordId),
                                         new InputParameterDefinition<int>(
                                             nameof(InputParamName.IncludePayload),
                                             new IntSqlDataTypeRepresentation(),
                                             streamRecordItemsToInclude == StreamRecordItemsToInclude.MetadataAndPayload ? 1 : 0),
                                         new OutputParameterDefinition<int>(nameof(OutputParamName.ShouldHandle), new IntSqlDataTypeRepresentation()),
                                         new OutputParameterDefinition<int>(nameof(OutputParamName.IsBlocked), new IntSqlDataTypeRepresentation()),
                                         new OutputParameterDefinition<long>(nameof(OutputParamName.Id), Tables.Handling.Id.SqlDataType),
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
                /// <param name="maxConcurrentHandlingCount">The optional maximum concurrent handling count; DEFAULT is no limit.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    int? maxConcurrentHandlingCount,
                    bool asAlter = false)
                {
                    const string recordIdsToConsiderTable = "RecordIdsToConsiderTable";
                    const string recordIdToAttemptToClaim = "RecordIdToAttemptToClaim";
                    const string candidateRecordIds = "CandidateRecordIds";
                    const string streamBlockedStatus = "StreamBlockedStatus";
                    const string currentRunningCount = "CurrentRunningCount";
                    const string isUnhandledRecord = "IsUnhandledRecord";

                    var acceptableStatusesCsv =
                        new[]
                        {
                            HandlingStatus.AvailableByDefault.ToString(),
                            HandlingStatus.AvailableAfterSelfCancellation.ToString(),
                            HandlingStatus.AvailableAfterExternalCancellation.ToString(),
                            HandlingStatus.AvailableAfterFailure.ToString(),
                            HandlingStatus.AvailableAfterCompletion.ToString(),
                        }.ToCsv();

                    var shouldAttemptHandling = "ShouldAttemptHandling";
                    var concurrentCheckBlock = maxConcurrentHandlingCount == null
                        ? string.Empty
                        : Invariant($@"
    IF (@{shouldAttemptHandling} = 1)
	BEGIN
        -- Can handle but might be at max handling capacity
	    DECLARE @{currentRunningCount} INT
	    SELECT @{currentRunningCount} = COUNT(*)
		FROM [{streamName}].[{Tables.Handling.Table.Name}] h
		LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
		ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
	    AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
		WHERE
	        h1.[{Tables.Handling.Status.Name}] IS NULL
		AND h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.Running}'

		IF (@{currentRunningCount} >= {maxConcurrentHandlingCount})
		BEGIN
			SET @{shouldAttemptHandling} = 0
	    END
	END
");

                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    var result = Invariant($@"
{createOrModify} PROCEDURE [{streamName}].[{TryHandleRecord.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Concern.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.InternalRecordIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.StringIdentifiersXml} {new XmlSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.TagIdsToMatchCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.TagMatchStrategy} {new StringSqlDataTypeRepresentation(false, 40).DeclarationInSqlSyntax}
, @{InputParamName.VersionMatchStrategy} {new StringSqlDataTypeRepresentation(false, 20).DeclarationInSqlSyntax}
, @{InputParamName.DeprecatedIdEventTypeIdsCsv} {new StringSqlDataTypeRepresentation(false, StringSqlDataTypeRepresentation.MaxNonUnicodeLengthConstant).DeclarationInSqlSyntax}
, @{InputParamName.OrderRecordsBy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.TagIdsForEntryCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.InheritRecordTags} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.MinimumInternalRecordId} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IncludePayload} {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Handling.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.BinarySerializedObject} AS {Tables.Record.BinarySerializedObject.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ShouldHandle} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IsBlocked} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{streamBlockedStatus} {Tables.Handling.Status.SqlDataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{streamBlockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.StreamHandlingDisabledConcern}'
    ORDER BY [{Tables.Handling.Id.Name}] DESC

	-- Check if global handling block has been applied
	DECLARE @{shouldAttemptHandling} BIT
	IF(@{streamBlockedStatus} = '{HandlingStatus.DisabledForStream}')
	BEGIN
		SET @{OutputParamName.IsBlocked} = 1
    END
	ELSE
	BEGIN
		SET @{shouldAttemptHandling} = 1
		SET @{OutputParamName.IsBlocked} = 0
	END

	{concurrentCheckBlock}

	IF (@{shouldAttemptHandling} = 1)
	BEGIN
		{RecordFilterLogic.BuildRecordFilterToBuildRecordsToConsiderTable(streamName, recordIdsToConsiderTable)}

        -- Remove records under the minimum record id from consideration
        DELETE FROM @{recordIdsToConsiderTable}
	        WHERE [{Tables.Record.Id.Name}] < @{InputParamName.MinimumInternalRecordId}

        -- Remove records with disabled handling status from consideration
        DELETE FROM @{recordIdsToConsiderTable}
	        WHERE [{Tables.Record.Id.Name}] IN
	        (
	            SELECT DISTINCT h.[{Tables.Handling.RecordId.Name}]
	            FROM [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
			    LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1 WITH (NOLOCK) -- the most recent handling status is disabled
			        ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
                    AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
	            WHERE
	                    h1.[{Tables.Handling.Id.Name}] IS NULL
	                AND h.[{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingDisabledConcern}'
	                AND h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.DisabledForRecord}'
	        )

        DECLARE @{candidateRecordIds} TABLE([{Tables.Record.Id.Name}] {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
		DECLARE @{isUnhandledRecord} {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
		IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.InternalRecordIdAscending}')
		BEGIN
			-- See if any reprocessing is needed
			INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
			FROM @{recordIdsToConsiderTable} rtc
			INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		    LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1 WITH (NOLOCK)
		        ON     h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
                   AND h.[{Tables.Handling.Concern.Name}] = h1.[{Tables.Handling.Concern.Name}]
                   AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
			WHERE
                 h1.[{Tables.Handling.Id.Name}] IS NULL
              AND
              (   h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterFailure}'
               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterExternalCancellation}'
               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterSelfCancellation}'
               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterCompletion}')

			-- See if any new records
			IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
			BEGIN
				-- Found records to reprocess
				SET @{isUnhandledRecord} = 0
			END
			ELSE
			BEGIN
				INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
				FROM @{recordIdsToConsiderTable} rtc
				LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
				WHERE
                   h.[{Tables.Handling.Id.Name}] IS NULL

				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					-- Found new records to process
					SET @{isUnhandledRecord} = 1
				END
			END -- Check for new records
		END -- If ascending
		ELSE IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.InternalRecordIdDescending}')
		BEGIN
			-- See if any new records
			INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
			FROM @{recordIdsToConsiderTable} rtc
			LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
            ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
               AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			WHERE
               h.[{Tables.Handling.Id.Name}] IS NULL

			-- See if any reprocessing is needed
			IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
			BEGIN
				SET @{isUnhandledRecord} = 1
			END
			ELSE
			BEGIN
				INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
				FROM @{recordIdsToConsiderTable} rtc
				INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
	                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
	                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			    LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1 WITH (NOLOCK)
			        ON     h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
	                   AND h.[{Tables.Handling.Concern.Name}] = h1.[{Tables.Handling.Concern.Name}]
	                   AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
				WHERE
	                 h1.[{Tables.Handling.Id.Name}] IS NULL
	              AND
	              (   h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterFailure}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterExternalCancellation}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterSelfCancellation}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterCompletion}')

				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					SET @{isUnhandledRecord} = 0
				END
			END -- Check for re-run
		END -- Descending
		ELSE IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.Random}')
		BEGIN
			-- Choose to handle old or new first
			IF (RAND() > .5)
			BEGIN
				-- See if any new records
				INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
				FROM @{recordIdsToConsiderTable} rtc
				LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
	            ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
	               AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
				WHERE
	               h.[{Tables.Handling.Id.Name}] IS NULL

				-- See if any reprocessing is needed
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					SET @{isUnhandledRecord} = 1
				END
				ELSE
				BEGIN
					INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
					FROM @{recordIdsToConsiderTable} rtc
					INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
		                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
		                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
				    LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1 WITH (NOLOCK)
				        ON     h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
		                   AND h.[{Tables.Handling.Concern.Name}] = h1.[{Tables.Handling.Concern.Name}]
		                   AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
					WHERE
		                 h1.[{Tables.Handling.Id.Name}] IS NULL
		              AND
		              (   h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterFailure}'
		               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterExternalCancellation}'
		               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterSelfCancellation}'
		               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterCompletion}')

					IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
					BEGIN
						SET @{isUnhandledRecord} = 0
					END
				END -- Check for re-run
			END -- New First
			ELSE
			BEGIN
				-- See if any reprocessing is needed
				INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
				FROM @{recordIdsToConsiderTable} rtc
				INNER JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
	                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
	                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			    LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1 WITH (NOLOCK)
			        ON     h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}]
	                   AND h.[{Tables.Handling.Concern.Name}] = h1.[{Tables.Handling.Concern.Name}]
	                   AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
				WHERE
	                 h1.[{Tables.Handling.Id.Name}] IS NULL
	              AND
	              (   h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterFailure}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterExternalCancellation}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterSelfCancellation}'
	               OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.AvailableAfterCompletion}')

				-- See if any new records
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					-- Found records to reprocess
					SET @{isUnhandledRecord} = 0
				END
				ELSE
				BEGIN
					INSERT INTO @{candidateRecordIds} SELECT rtc.[{Tables.Record.Id.Name}]
					FROM @{recordIdsToConsiderTable} rtc
					LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h WITH (NOLOCK)
	                ON     h.[{Tables.Handling.RecordId.Name}] = rtc.[{Tables.Record.Id.Name}]
	                   AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
					WHERE
	                   h.[{Tables.Handling.Id.Name}] IS NULL

					IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
					BEGIN
						-- Found new records to process
						SET @{isUnhandledRecord} = 1
					END
				END -- Check for new records
			END -- Retry First
		END -- Random
		ELSE
		BEGIN
				DECLARE @NotValidStrategyErrorMessage varchar(100)
				SET @NotValidStrategyErrorMessage =  CONCAT('Invalid {InputParamName.OrderRecordsBy}: ', @{InputParamName.OrderRecordsBy}, '.');
				THROW 60000, @NotValidStrategyErrorMessage, 1
		END
		IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
		BEGIN
			DECLARE @{recordIdToAttemptToClaim} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
			-- TODO: add logic here, loop through candidates until we have one or out of options...
			IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.InternalRecordIdAscending}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY [{Tables.Record.Id.Name}] ASC
			END
			ELSE IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.InternalRecordIdDescending}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY [{Tables.Record.Id.Name}] DESC
			END
			ELSE IF (@{InputParamName.OrderRecordsBy} = '{OrderRecordsBy.Random}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY NEWID()
			END
			ELSE
			BEGIN
				DECLARE @NotValidStrategyClaimErrorMessage varchar(100)
				SET @NotValidStrategyClaimErrorMessage =  CONCAT('Invalid {InputParamName.OrderRecordsBy}: ', @{InputParamName.OrderRecordsBy}, '.');
				THROW 60000, @NotValidStrategyClaimErrorMessage, 1
			END

			EXEC [{streamName}].[{PutHandling.Name}]
			@{PutHandling.InputParamName.Concern} = @{InputParamName.Concern},
			@{PutHandling.InputParamName.Details} = @{InputParamName.Details},
			@{PutHandling.InputParamName.RecordId} = @{recordIdToAttemptToClaim},
			@{PutHandling.InputParamName.NewStatus} = '{HandlingStatus.Running}',
			@{PutHandling.InputParamName.AcceptableCurrentStatusesCsv} = '{acceptableStatusesCsv}',
			@{PutHandling.InputParamName.TagIdsForEntryCsv} = @{InputParamName.TagIdsForEntryCsv},
			@{PutHandling.InputParamName.InheritRecordTags} = @{InputParamName.InheritRecordTags},
			@{PutHandling.InputParamName.IsUnHandledRecord} = @{isUnhandledRecord},
			@{PutHandling.InputParamName.IsClaimingRecordId} = 1,
			@{PutHandling.OutputParamName.Id} = @{OutputParamName.Id} OUTPUT

			IF (@{OutputParamName.Id} IS NULL)
			BEGIN
			    SET @{OutputParamName.ShouldHandle} = 0
			END
			ELSE
			BEGIN
			    SET @{OutputParamName.ShouldHandle} = 1
                SET @{OutputParamName.InternalRecordId} = @{recordIdToAttemptToClaim}
			END
		END -- Found record
	END -- Should attempt handling
    IF (@{OutputParamName.ShouldHandle} = 1)
	BEGIN
		-- Fetch record to handle to output params
	    SELECT TOP 1
		   @{OutputParamName.SerializerRepresentationId} = [{Tables.Record.SerializerRepresentationId.Name}]
		 , @{OutputParamName.IdentifierTypeWithVersionId} = [{Tables.Record.IdentifierTypeWithVersionId.Name}]
		 , @{OutputParamName.ObjectTypeWithVersionId} = [{Tables.Record.ObjectTypeWithVersionId.Name}]
		 , @{OutputParamName.StringSerializedId} = [{Tables.Record.StringSerializedId.Name}]
		 , @{OutputParamName.StringSerializedObject} = (
            CASE @{InputParamName.IncludePayload}
                WHEN 1 THEN [{Tables.Record.StringSerializedObject.Name}]
                WHEN 0 THEN NULL
                ELSE CONVERT({new StringSqlDataTypeRepresentation(false, 1).DeclarationInSqlSyntax}, CONVERT({new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}, '@{InputParamName.IncludePayload} is used as a bit flag and should only be 1 or 0.'))
            END)
		 , @{OutputParamName.BinarySerializedObject} = (
            CASE @{InputParamName.IncludePayload}
                WHEN 1 THEN [{Tables.Record.BinarySerializedObject.Name}]
                WHEN 0 THEN NULL
                ELSE CONVERT({new BinarySqlDataTypeRepresentation(1).DeclarationInSqlSyntax}, CONVERT({new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}, '@{InputParamName.IncludePayload} is used as a bit flag and should only be 1 or 0.'))
            END)
		 , @{OutputParamName.TagIdsCsv} = [{Tables.Record.TagIdsCsv.Name}]
		 , @{OutputParamName.RecordDateTime} = [{Tables.Record.RecordCreatedUtc.Name}]
		 , @{OutputParamName.ObjectDateTime} = [{Tables.Record.ObjectDateTimeUtc.Name}]
		FROM [{streamName}].[{Tables.Record.Table.Name}]
		WHERE [{Tables.Record.Id.Name}] = @{OutputParamName.InternalRecordId}
	END
    ELSE
	BEGIN
		SET @{OutputParamName.ShouldHandle} = 0
		SET @{OutputParamName.Id} = {Tables.Handling.NullId}
		SET @{OutputParamName.InternalRecordId} = {Tables.Record.NullId}
		SET @{OutputParamName.SerializerRepresentationId} = {Tables.SerializerRepresentation.NullId}
		SET @{OutputParamName.IdentifierTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
		SET @{OutputParamName.ObjectTypeWithVersionId} = {Tables.TypeWithVersion.NullId}
		SET @{OutputParamName.StringSerializedId} = 'Fake'
		SET @{OutputParamName.ObjectDateTime} = GETUTCDATE()
		SET @{OutputParamName.RecordDateTime} = GETUTCDATE()
		SET @{OutputParamName.TagIdsCsv} = null
	END
END
");
                    return result;
                }
            }
        }
    }
}
