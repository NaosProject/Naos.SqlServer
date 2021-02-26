// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.TryHandleRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Container for schema.
    /// </summary>
    public static partial class StreamSchema
    {
        /// <summary>
        /// Stored procedures.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sprocs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        public partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: GetLatestByIdAndType.
            /// </summary>
            public static class TryHandleRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name.</value>
                public static string Name => nameof(TryHandleRecord);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Naming",
                    "CA1704:IdentifiersShouldBeSpelledCorrectly",
                    MessageId = "Param",
                    Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
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
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectTypeWithoutVersionIdQuery,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionIdQuery,

                    /// <summary>
                    /// The order record strategy
                    /// </summary>
                    OrderRecordsStrategy,

                    /// <summary>
                    /// The type version match strategy
                    /// </summary>
                    TypeVersionMatchStrategy,

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
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
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
                /// <param name="identifierType">Type of the identifier.</param>
                /// <param name="objectType">Type of the object.</param>
                /// <param name="orderRecordsStrategy">The order records strategy.</param>
                /// <param name="typeVersionMatchStrategy">The type version match strategy.</param>
                /// <param name="tagIdsCsv">The tag identifiers as CSV.</param>
                /// <param name="minimumInternalRecordId">The optional minimum internal record identifier, null for default.</param>
                /// <param name="inheritRecordTags">The tags on the record should also be on the handling entry.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    string concern,
                    string details,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    OrderRecordsStrategy orderRecordsStrategy,
                    TypeVersionMatchStrategy typeVersionMatchStrategy,
                    string tagIdsCsv,
                    long? minimumInternalRecordId,
                    bool inheritRecordTags)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(TryHandleRecord)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Concern), Tables.Handling.Concern.DataType, concern),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.Details), Tables.Handling.Details.DataType, details),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.OrderRecordsStrategy), new StringSqlDataTypeRepresentation(false, 50), orderRecordsStrategy.ToString()),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.DataType, identifierType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.DataType, identifierType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithoutVersionIdQuery), Tables.TypeWithoutVersion.Id.DataType, objectType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithVersionIdQuery), Tables.TypeWithVersion.Id.DataType, objectType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy.ToString()),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsForEntryCsv), Tables.Record.TagIdsCsv.DataType, tagIdsCsv),
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.InheritRecordTags), new IntSqlDataTypeRepresentation(), inheritRecordTags ? 1 : 0),
                                         new SqlInputParameterRepresentation<long?>(nameof(InputParamName.MinimumInternalRecordId), Tables.Record.Id.DataType, minimumInternalRecordId),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ShouldHandle), new IntSqlDataTypeRepresentation()),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.IsBlocked), new IntSqlDataTypeRepresentation()),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.Id), Tables.Handling.Id.DataType),
                                         new SqlOutputParameterRepresentation<long>(nameof(OutputParamName.InternalRecordId), Tables.Record.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.DataType),
                                         new SqlOutputParameterRepresentation<int>(nameof(OutputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedId), Tables.Record.StringSerializedId.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.DataType),
                                         new SqlOutputParameterRepresentation<byte[]>(nameof(OutputParamName.BinarySerializedObject), Tables.Record.BinarySerializedObject.DataType),
                                         new SqlOutputParameterRepresentation<DateTime>(nameof(OutputParamName.RecordDateTime), Tables.Record.RecordCreatedUtc.DataType),
                                         new SqlOutputParameterRepresentation<DateTime?>(nameof(OutputParamName.ObjectDateTime), Tables.Record.ObjectDateTimeUtc.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.TagIdsCsv), Tables.Record.TagIdsCsv.DataType),
                                     };

                    var parameterNameToRepresentationMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToRepresentationMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put sproc.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="maxConcurrentHandlingCount">The optional maximum concurrent handling count; DEFAULT is no limit.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>System.String.</returns>
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
                    int? maxConcurrentHandlingCount,
                    bool asAlter = false)
                {
                    const string recordIdToAttemptToClaim = "RecordIdToAttemptToClaim";
                    const string candidateRecordIds = "CandidateRecordIds";
                    const string blockedStatus = "BlockedStatus";
                    const string currentRunningCount = "CurrentRunningCount";
                    const string isUnhandledRecord = "IsUnhandledRecord";
                    const string unionedIfNecessaryTagIdsCsv = "UnionedIfNecessaryTagIdsCsv";
                    var acceptableStatusesCsv =
                        new[]
                        {
                            HandlingStatus.None.ToString(),
                            HandlingStatus.Requested.ToString(),
                            HandlingStatus.SelfCanceledRunning.ToString(),
                            HandlingStatus.CanceledRunning.ToString(),
                            HandlingStatus.RetryFailed.ToString(),
                        }.ToCsv();

                    var shouldAttemptHandling = "ShouldAttemptHandling";
                    var concurrentCheckBlock = maxConcurrentHandlingCount == null
                        ? string.Empty
                        : Invariant($@"
    DECLARE @{currentRunningCount} INT
    SELECT @{currentRunningCount} = COUNT(*)
	FROM [{streamName}].[{Tables.Handling.Table.Name}] h 
	LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h1
	ON h.[{Tables.Handling.RecordId.Name}] = h1.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Id.Name}] < h1.[{Tables.Handling.Id.Name}]
	WHERE
        h1.[{Tables.Handling.Status.Name}] IS NULL
	AND h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.Running}'
    
	IF (@{currentRunningCount} >= {maxConcurrentHandlingCount})
	BEGIN
		SET @{shouldAttemptHandling} = 0
    END
");

                    var createOrModify = asAlter ? "ALTER" : "CREATE";
                    var result = Invariant($@"
{createOrModify} PROCEDURE [{streamName}].[{TryHandleRecord.Name}](
  @{InputParamName.Concern} AS {Tables.Handling.Concern.DataType.DeclarationInSqlSyntax}
, @{InputParamName.Details} AS {Tables.Handling.Details.DataType.DeclarationInSqlSyntax}
, @{InputParamName.OrderRecordsStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionIdQuery} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionIdQuery} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TypeVersionMatchStrategy} AS varchar(10)
, @{InputParamName.TagIdsForEntryCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax}
, @{InputParamName.InheritRecordTags} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.MinimumInternalRecordId} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Handling.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.InternalRecordId} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.BinarySerializedObject} AS {Tables.Record.BinarySerializedObject.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ObjectDateTime} AS {Tables.Record.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.RecordDateTime} AS {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ShouldHandle} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.IsBlocked} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    DECLARE @{blockedStatus} {Tables.Handling.Status.DataType.DeclarationInSqlSyntax}
	SELECT TOP 1 @{blockedStatus} = [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
	WHERE [{Tables.Handling.Concern.Name}] = '{Concerns.RecordHandlingConcern}'

	-- Check if global handling block has been applied
	DECLARE @{shouldAttemptHandling} BIT
	IF(@{blockedStatus} = '{HandlingStatus.Blocked}')
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
        DECLARE @{candidateRecordIds} TABLE([{Tables.Record.Id.Name}] {Tables.Record.Id.DataType.DeclarationInSqlSyntax} NOT NULL)
		DECLARE @{isUnhandledRecord} {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
		IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdAscending}')
		BEGIN
			-- See if any reprocessing is needed
			INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
			FROM [{streamName}].[{Tables.Handling.Table.Name}] h
			INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
			WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
	        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
			AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
		    AND (
		            -- No Type filter at all
		            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
		            OR
		            -- Specific Only Id
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
		            OR
		            -- Specific Only Object
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
		            OR
		            -- Specific Both
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
		            OR
		            -- Any Only Id
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
		            OR
		            -- Any Only Object
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
		            OR
		            -- Any Both
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
		        )
			AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
			-- Check for re-run scenario

			-- See if any new records
			IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
			BEGIN
				-- Found records to reprocess
				SET @{isUnhandledRecord} = 0
			END
			ELSE
			BEGIN
				INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
				FROM [{streamName}].[{Tables.Record.Table.Name}] r
				LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
				ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
				WHERE h.[{Tables.Handling.Id.Name}] IS NULL
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
				AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					-- Found new records to process
					SET @{isUnhandledRecord} = 1
				END
			END -- Check for new records
		END -- If ascending
		ELSE IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdDescending}')
		BEGIN
			-- See if any new records
			INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
			FROM [{streamName}].[{Tables.Record.Table.Name}] r
			LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
			ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			WHERE h.[{Tables.Handling.Id.Name}] IS NULL
		    AND (
		            -- No Type filter at all
		            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
		            OR
		            -- Specific Only Id
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
		            OR
		            -- Specific Only Object
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
		            OR
		            -- Specific Both
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
		            OR
		            -- Any Only Id
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
		            OR
		            -- Any Only Object
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
		            OR
		            -- Any Both
		            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
		        )
			AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})

			-- Check for new records
			IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
			BEGIN
				SET @{isUnhandledRecord} = 1
			END
			ELSE
			BEGIN
				INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
				FROM [{streamName}].[{Tables.Handling.Table.Name}] h
				INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
				WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
				AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
				AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					SET @{isUnhandledRecord} = 0
				END
			END -- Check for re-run
		END -- Descending
		ELSE IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.Random}')
		BEGIN
			-- Choose to handle old or new first
			IF (RAND() > .5)
			BEGIN
				-- See if any new records
				INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
				FROM [{streamName}].[{Tables.Record.Table.Name}] r
				LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
				ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
				WHERE h.[{Tables.Handling.Id.Name}] IS NULL
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
				AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})

				-- Check for new records
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					SET @{isUnhandledRecord} = 1
				END
				ELSE
				BEGIN
					INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
					FROM [{streamName}].[{Tables.Handling.Table.Name}] h
					INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
					WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
			        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
					AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
				    AND (
				            -- No Type filter at all
				            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
				            OR
				            -- Specific Only Id
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
				            OR
				            -- Specific Only Object
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
				            OR
				            -- Specific Both
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
				            OR
				            -- Any Only Id
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
				            OR
				            -- Any Only Object
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
				            OR
				            -- Any Both
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
				        )
					AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
					IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
					BEGIN
						SET @{isUnhandledRecord} = 0
					END
				END -- Check for re-run
			END -- New First
			ELSE
			BEGIN
				-- See if any reprocessing is needed
				INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
				FROM [{streamName}].[{Tables.Handling.Table.Name}] h
				INNER JOIN [{streamName}].[{Tables.Record.Table.Name}] r ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}]
				WHERE h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
		        AND (h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.RetryFailed}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.CanceledRunning}' OR h.[{Tables.Handling.Status.Name}] = '{HandlingStatus.SelfCanceledRunning}')
				AND (SELECT TOP 1 [{Tables.Handling.Status.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}] i WHERE i.{Tables.Handling.RecordId.Name} = h.{Tables.Handling.RecordId.Name} ORDER BY i.{Tables.Handling.Id.Name} DESC) = h.{Tables.Handling.Status.Name}
			    AND (
			            -- No Type filter at all
			            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
			            OR
			            -- Specific Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
			            OR
			            -- Specific Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Specific Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
			            OR
			            -- Any Only Id
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
			            OR
			            -- Any Only Object
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			            OR
			            -- Any Both
			            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
			        )
				AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
				-- Check for re-run scenario

				-- See if any new records
				IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
				BEGIN
					-- Found records to reprocess
					SET @{isUnhandledRecord} = 0
				END
				ELSE
				BEGIN
					INSERT INTO @{candidateRecordIds} SELECT r.[{Tables.Record.Id.Name}]
					FROM [{streamName}].[{Tables.Record.Table.Name}] r
					LEFT JOIN [{streamName}].[{Tables.Handling.Table.Name}] h
					ON r.[{Tables.Record.Id.Name}] = h.[{Tables.Handling.RecordId.Name}] AND h.[{Tables.Handling.Concern.Name}] = @{InputParamName.Concern}
					WHERE h.[{Tables.Handling.Id.Name}] IS NULL
				    AND (
				            -- No Type filter at all
				            (@{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL)
				            OR
				            -- Specific Only Id
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery})
				            OR
				            -- Specific Only Object
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
				            OR
				            -- Specific Both
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}' AND @{InputParamName.IdentifierTypeWithVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionIdQuery} AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionIdQuery})
				            OR
				            -- Any Only Id
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery})
				            OR
				            -- Any Only Object
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NULL AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
				            OR
				            -- Any Both
				            (@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}' AND @{InputParamName.IdentifierTypeWithoutVersionIdQuery} IS NOT NULL AND @{InputParamName.ObjectTypeWithoutVersionIdQuery} IS NOT NULL AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionIdQuery} AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionIdQuery})
				        )
					AND (@{InputParamName.MinimumInternalRecordId} IS NULL OR r.[{Tables.Record.Id.Name}] >= @{InputParamName.MinimumInternalRecordId})
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
				SET @NotValidStrategyErrorMessage =  CONCAT('Invalid {InputParamName.OrderRecordsStrategy}: ', @{InputParamName.OrderRecordsStrategy}, '.');
				THROW 60000, @NotValidStrategyErrorMessage, 1
		END
		IF EXISTS (SELECT TOP 1 [{Tables.Record.Id.Name}] FROM @{candidateRecordIds})
		BEGIN
			DECLARE @{recordIdToAttemptToClaim} {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
			-- TODO: loop through candidates until we have one or out of options...
			IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdAscending}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY [{Tables.Record.Id.Name}] ASC
			END
			ELSE IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.ByInternalRecordIdDescending}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY [{Tables.Record.Id.Name}] DESC
			END
			ELSE IF (@{InputParamName.OrderRecordsStrategy} = '{OrderRecordsStrategy.Random}')
			BEGIN
				SELECT TOP 1 @{recordIdToAttemptToClaim} = [{Tables.Record.Id.Name}] FROM @{candidateRecordIds} ORDER BY NEWID()
			END
			ELSE
			BEGIN
				DECLARE @NotValidStrategyClaimErrorMessage varchar(100)
				SET @NotValidStrategyClaimErrorMessage =  CONCAT('Invalid {InputParamName.OrderRecordsStrategy}: ', @{InputParamName.OrderRecordsStrategy}, '.');
				THROW 60000, @NotValidStrategyClaimErrorMessage, 1
			END

			DECLARE @{unionedIfNecessaryTagIdsCsv} {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax}
			
	        SELECT @{unionedIfNecessaryTagIdsCsv} = STRING_AGG([{Tables.Tag.Id.Name}], ',')
	        FROM
				(
	                SELECT DISTINCT [{Tables.Tag.Id.Name}] FROM
					(
						SELECT value AS [{Tables.Tag.Id.Name}]
					    FROM STRING_SPLIT(@{InputParamName.TagIdsForEntryCsv}, ',')
				        UNION ALL
						SELECT [{Tables.RecordTag.TagId.Name}] AS [{Tables.Tag.Id.Name}]
	                    FROM [{streamName}].[{Tables.RecordTag.Table.Name}]
						WHERE @{InputParamName.InheritRecordTags} = 1 AND [{Tables.RecordTag.RecordId.Name}] = @{recordIdToAttemptToClaim}
					) AS u
				) AS d

			EXEC [{streamName}].[{PutHandling.Name}] 
			@{PutHandling.InputParamName.Concern} = @{InputParamName.Concern}, 
			@{PutHandling.InputParamName.Details} = @{InputParamName.Details}, 
			@{PutHandling.InputParamName.RecordId} = @{recordIdToAttemptToClaim}, 
			@{PutHandling.InputParamName.NewStatus} = '{HandlingStatus.Running}', 
			@{PutHandling.InputParamName.AcceptableCurrentStatusesCsv} = '{acceptableStatusesCsv}', 
			@{PutHandling.InputParamName.TagIdsCsv} = @{unionedIfNecessaryTagIdsCsv}, 
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
		 , @{OutputParamName.StringSerializedObject} = [{Tables.Record.StringSerializedObject.Name}]
		 , @{OutputParamName.BinarySerializedObject} = [{Tables.Record.BinarySerializedObject.Name}]
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
