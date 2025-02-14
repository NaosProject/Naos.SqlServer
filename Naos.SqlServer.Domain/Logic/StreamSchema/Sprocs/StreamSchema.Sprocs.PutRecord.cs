// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    public static partial class StreamSchema
    {
        public static partial class Sprocs
        {
            /// <summary>
            /// Stored procedure: PutRecord.
            /// </summary>
            public static class PutRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                public static string Name => nameof(PutRecord);

                /// <summary>
                /// Input parameter names.
                /// </summary>
                public enum InputParamName
                {
                    /// <summary>
                    /// The identifier assembly qualified name without version
                    /// </summary>
                    IdentifierTypeWithoutVersionId,

                    /// <summary>
                    /// The identifier assembly qualified name with version
                    /// </summary>
                    IdentifierTypeWithVersionId,

                    /// <summary>
                    /// The object assembly qualified name without version
                    /// </summary>
                    ObjectTypeWithoutVersionId,

                    /// <summary>
                    /// The object assembly qualified name with version
                    /// </summary>
                    ObjectTypeWithVersionId,

                    /// <summary>
                    /// The serializer description identifier.
                    /// </summary>
                    SerializerRepresentationId,

                    /// <summary>
                    /// The internal record identifier.
                    /// </summary>
                    InternalRecordId,

                    /// <summary>
                    /// The serialized object identifier.
                    /// </summary>
                    StringSerializedId,

                    /// <summary>
                    /// The serialized object string.
                    /// </summary>
                    StringSerializedObject,

                    /// <summary>
                    /// The serialized object bytes.
                    /// </summary>
                    BinarySerializedObject,

                    /// <summary>
                    /// The object's date time UTC if available.
                    /// </summary>
                    ObjectDateTimeUtc,

                    /// <summary>
                    /// The tags as CSV.
                    /// </summary>
                    TagIdsCsv,

                    /// <summary>
                    /// The existing record encountered strategy.
                    /// </summary>
                    ExistingRecordStrategy,

                    /// <summary>
                    /// The number of records to keep if using a pruning <see cref="ExistingRecordStrategy"/>.
                    /// </summary>
                    RecordRetentionCount,

                    /// <summary>
                    /// The type version match strategy.
                    /// </summary>
                    VersionMatchStrategy,
                }

                /// <summary>
                /// Output parameter names.
                /// </summary>
                public enum OutputParamName
                {
                    /// <summary>
                    /// The identifier.
                    /// </summary>
                    Id,

                    /// <summary>
                    /// The existing record identifiers as CSV (if any dependent on strategy).
                    /// </summary>
                    ExistingRecordIdsCsv,

                    /// <summary>
                    /// The pruned record identifiers as CSV (if any dependent on strategy).
                    /// </summary>
                    PrunedRecordIdsCsv,
                }

                /// <summary>
                /// Builds the execute stored procedure operation.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="serializerRepresentation">The serializer representation.</param>
                /// <param name="identifierType">The identifier type.</param>
                /// <param name="objectType">The object type.</param>
                /// <param name="internalRecordId">The optional internal record identifier (for stream re-play).</param>
                /// <param name="serializedObjectId">The serialized object identifier.</param>
                /// <param name="serializedObjectString">The serialized object as a string (should have data IFF the serializer is set to SerializationFormat.String, otherwise null).</param>
                /// <param name="serializedObjectBytes">The serialized object as a byte array (should have data IFF the serializer is set to SerializationFormat.Binary, otherwise null).</param>
                /// <param name="objectDateTimeUtc">The date time of the object if exists.</param>
                /// <param name="tagIdsCsv">The tag identifiers as CSV.</param>
                /// <param name="existingRecordStrategy">Existing record encountered strategy.</param>
                /// <param name="recordRetentionCount">Number of records to keep if using a pruning <paramref name="existingRecordStrategy"/>.</param>
                /// <param name="versionMatchStrategy">Type version match strategy.</param>
                /// <returns>Operation to execute stored procedure.</returns>
                public static ExecuteStoredProcedureOp BuildExecuteStoredProcedureOp(
                    string streamName,
                    IdentifiedSerializerRepresentation serializerRepresentation,
                    IdentifiedType identifierType,
                    IdentifiedType objectType,
                    long? internalRecordId,
                    string serializedObjectId,
                    string serializedObjectString,
                    byte[] serializedObjectBytes,
                    DateTime? objectDateTimeUtc,
                    string tagIdsCsv,
                    ExistingRecordStrategy existingRecordStrategy,
                    int? recordRetentionCount,
                    VersionMatchStrategy versionMatchStrategy)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(PutRecord)}");

                    var parameters = new List<ParameterDefinitionBase>()
                    {
                        new InputParameterDefinition<int>(nameof(InputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.SqlDataType, serializerRepresentation.Id),
                        new InputParameterDefinition<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.SqlDataType, identifierType?.IdWithoutVersion),
                        new InputParameterDefinition<int?>(nameof(InputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.SqlDataType, identifierType?.IdWithVersion),
                        new InputParameterDefinition<int?>(nameof(InputParamName.ObjectTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.SqlDataType, objectType?.IdWithoutVersion),
                        new InputParameterDefinition<int?>(nameof(InputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.SqlDataType, objectType?.IdWithVersion),
                        new InputParameterDefinition<long?>(nameof(InputParamName.InternalRecordId), Tables.Record.Id.SqlDataType, internalRecordId),
                        new InputParameterDefinition<string>(nameof(InputParamName.StringSerializedId), Tables.Record.StringSerializedId.SqlDataType, serializedObjectId),
                        new InputParameterDefinition<string>(nameof(InputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.SqlDataType, serializedObjectString),
                        new InputParameterDefinition<byte[]>(nameof(InputParamName.BinarySerializedObject), Tables.Record.BinarySerializedObject.SqlDataType, serializedObjectBytes),
                        new InputParameterDefinition<DateTime?>(nameof(InputParamName.ObjectDateTimeUtc), Tables.Record.ObjectDateTimeUtc.SqlDataType, objectDateTimeUtc),
                        new InputParameterDefinition<string>(nameof(InputParamName.TagIdsCsv), Tables.Record.TagIdsCsv.SqlDataType, tagIdsCsv),
                        new InputParameterDefinition<ExistingRecordStrategy>(nameof(InputParamName.ExistingRecordStrategy), new StringSqlDataTypeRepresentation(false, 50), existingRecordStrategy),
                        new InputParameterDefinition<int?>(nameof(InputParamName.RecordRetentionCount), new IntSqlDataTypeRepresentation(), recordRetentionCount),
                        new InputParameterDefinition<VersionMatchStrategy>(nameof(InputParamName.VersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), versionMatchStrategy),
                        new OutputParameterDefinition<long?>(nameof(OutputParamName.Id), Tables.Record.Id.SqlDataType),
                        new OutputParameterDefinition<string>(nameof(OutputParamName.ExistingRecordIdsCsv), Tables.Record.TagIdsCsv.SqlDataType),
                        new OutputParameterDefinition<string>(nameof(OutputParamName.PrunedRecordIdsCsv), Tables.Record.TagIdsCsv.SqlDataType),
                    };

                    var result = new ExecuteStoredProcedureOp(sprocName, parameters);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="recordTagAssociationManagementStrategy">The record tag association management strategy.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>Creation script for creating the stored procedure.</returns>
                public static string BuildCreationScript(
                    string streamName,
                    RecordTagAssociationManagementStrategy recordTagAssociationManagementStrategy,
                    bool asAlter = false)
                {
                    const string recordCreatedUtc = "RecordCreatedUtc";
                    const string existingIdsTable = "ExistingIdsTable";
                    const string existingIdsCount = "ExistingIdsCount";
                    const string prunedIdsTable = "PrunedIdsTable";
                    var transaction = Invariant($"{nameof(PutRecord)}Transaction");
                    var pruneTransaction = Invariant($"PruneTransaction");
                    var createOrModify = asAlter ? "CREATE OR ALTER" : "CREATE";
                    string insertRowsBlock;
                    switch (recordTagAssociationManagementStrategy)
                    {
                        case RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocInTransaction:
                            insertRowsBlock = Invariant($@"
		BEGIN TRANSACTION [{transaction}]
		  BEGIN TRY
		  INSERT INTO [{streamName}].[{Tables.Record.Table.Name}] (
			  [{nameof(Tables.Record.IdentifierTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.IdentifierTypeWithVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithVersionId)}]
			, [{nameof(Tables.Record.SerializerRepresentationId)}]
			, [{nameof(Tables.Record.StringSerializedId)}]
			, [{nameof(Tables.Record.StringSerializedObject)}]
			, [{nameof(Tables.Record.BinarySerializedObject)}]
			, [{nameof(Tables.Record.TagIdsCsv)}]
			, [{nameof(Tables.Record.ObjectDateTimeUtc)}]
			, [{nameof(Tables.Record.RecordCreatedUtc)}]
			) VALUES (
			  @{InputParamName.IdentifierTypeWithoutVersionId}
			, @{InputParamName.IdentifierTypeWithVersionId}
			, @{InputParamName.ObjectTypeWithoutVersionId}
			, @{InputParamName.ObjectTypeWithVersionId}
			, @{InputParamName.SerializerRepresentationId}
			, @{InputParamName.StringSerializedId}
			, @{InputParamName.StringSerializedObject}
			, @{InputParamName.BinarySerializedObject}
			, @{InputParamName.TagIdsCsv}
			, @{InputParamName.ObjectDateTimeUtc}
			, @{recordCreatedUtc}
			)

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()

	      INSERT INTO [{streamName}].[{Tables.RecordTag.Table.Name}](
		    [{Tables.RecordTag.RecordId.Name}]
		  , [{Tables.RecordTag.TagId.Name}]
		  , [{Tables.RecordTag.RecordCreatedUtc.Name}])
          SELECT
  		    @{OutputParamName.Id}
		  , value AS [{Tables.Tag.Id.Name}]
		  , @{recordCreatedUtc}
          FROM STRING_SPLIT(@{InputParamName.TagIdsCsv}, ',')
	    COMMIT TRANSACTION [{transaction}]
	  END TRY
	  BEGIN CATCH
	      DECLARE @PruneThrowMessage nvarchar(max),
                  @PruneErrorMessage nvarchar(max),
	              @PruneErrorSeverity int,
	              @PruneErrorState int

	      SELECT @PruneErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @PruneErrorSeverity = ERROR_SEVERITY(), @PruneErrorState = ERROR_STATE()
          SELECT @PruneThrowMessage = @PruneErrorMessage + '; ErrorSeverity=' + cast(@PruneErrorSeverity as nvarchar(20)) + '; ErrorState=' + cast(@PruneErrorState as nvarchar(20))

	      IF (@@trancount > 0)
	      BEGIN
	         ROLLBACK TRANSACTION [{transaction}]
	      END;

	      THROW {GeneralPurposeErrorNumberForThrowStatement}, @PruneThrowMessage, {GeneralPurposeErrorStateForThrowStatement}
	  END CATCH");
                            break;
                        case RecordTagAssociationManagementStrategy.AssociatedDuringPutInSprocOutOfTransaction:
                            insertRowsBlock = Invariant($@"

		  INSERT INTO [{streamName}].[{Tables.Record.Table.Name}] (
			  [{nameof(Tables.Record.IdentifierTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.IdentifierTypeWithVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithVersionId)}]
			, [{nameof(Tables.Record.SerializerRepresentationId)}]
			, [{nameof(Tables.Record.StringSerializedId)}]
			, [{nameof(Tables.Record.StringSerializedObject)}]
			, [{nameof(Tables.Record.BinarySerializedObject)}]
			, [{nameof(Tables.Record.TagIdsCsv)}]
			, [{nameof(Tables.Record.ObjectDateTimeUtc)}]
			, [{nameof(Tables.Record.RecordCreatedUtc)}]
			) VALUES (
			  @{InputParamName.IdentifierTypeWithoutVersionId}
			, @{InputParamName.IdentifierTypeWithVersionId}
			, @{InputParamName.ObjectTypeWithoutVersionId}
			, @{InputParamName.ObjectTypeWithVersionId}
			, @{InputParamName.SerializerRepresentationId}
			, @{InputParamName.StringSerializedId}
			, @{InputParamName.StringSerializedObject}
			, @{InputParamName.BinarySerializedObject}
			, @{InputParamName.TagIdsCsv}
			, @{InputParamName.ObjectDateTimeUtc}
			, @{recordCreatedUtc}
			)

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()

	      INSERT INTO [{streamName}].[{Tables.RecordTag.Table.Name}](
		    [{Tables.RecordTag.RecordId.Name}]
		  , [{Tables.RecordTag.TagId.Name}]
		  , [{Tables.RecordTag.RecordCreatedUtc.Name}])
	      SELECT
  		    @{OutputParamName.Id}
		  , value AS [{Tables.Tag.Id.Name}]
		  , @{recordCreatedUtc}
         FROM STRING_SPLIT(@{InputParamName.TagIdsCsv}, ',')
");
                            break;
                        case RecordTagAssociationManagementStrategy.ExternallyManaged:
                            insertRowsBlock = Invariant($@"
		  INSERT INTO [{streamName}].[{Tables.Record.Table.Name}] (
			  [{nameof(Tables.Record.IdentifierTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.IdentifierTypeWithVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithoutVersionId)}]
			, [{nameof(Tables.Record.ObjectTypeWithVersionId)}]
			, [{nameof(Tables.Record.SerializerRepresentationId)}]
			, [{nameof(Tables.Record.StringSerializedId)}]
			, [{nameof(Tables.Record.StringSerializedObject)}]
			, [{nameof(Tables.Record.BinarySerializedObject)}]
			, [{nameof(Tables.Record.TagIdsCsv)}]
			, [{nameof(Tables.Record.ObjectDateTimeUtc)}]
			, [{nameof(Tables.Record.RecordCreatedUtc)}]
			) VALUES (
			  @{InputParamName.IdentifierTypeWithoutVersionId}
			, @{InputParamName.IdentifierTypeWithVersionId}
			, @{InputParamName.ObjectTypeWithoutVersionId}
			, @{InputParamName.ObjectTypeWithVersionId}
			, @{InputParamName.SerializerRepresentationId}
			, @{InputParamName.StringSerializedId}
			, @{InputParamName.StringSerializedObject}
			, @{InputParamName.BinarySerializedObject}
			, @{InputParamName.TagIdsCsv}
			, @{InputParamName.ObjectDateTimeUtc}
			, @{recordCreatedUtc}
			)

	      SET @{OutputParamName.Id} = SCOPE_IDENTITY()
		  ");
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"{nameof(RecordTagAssociationManagementStrategy)} '{recordTagAssociationManagementStrategy}' is not supported."));
                    }

                    var result = Invariant($@"
{createOrModify} PROCEDURE [{streamName}].[{PutRecord.Name}](
  @{InputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithVersion.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.InternalRecordId} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.BinarySerializedObject} AS {Tables.Record.BinarySerializedObject.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectDateTimeUtc} AS {Tables.Record.ObjectDateTimeUtc.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax}
, @{InputParamName.ExistingRecordStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.RecordRetentionCount} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.VersionMatchStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ExistingRecordIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.PrunedRecordIdsCsv} AS {Tables.Record.TagIdsCsv.SqlDataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    -- Adjust {InputParamName.StringSerializedId} if null since the table cannot hold nulls because of indexing.
    SET @{InputParamName.StringSerializedId} = [{streamName}].[{Funcs.AdjustForPutStringSerializedId.Name}](@{InputParamName.StringSerializedId})

    -- If two actors try to both insert for the same ID with the '{nameof(ExistingRecordStrategy)}'
	--	   set to e.g. {ExistingRecordStrategy.DoNotWriteIfFoundByIdAndTypeAndContent}; they could both
	--     write the same payload; this does work in a single actor re-entrant scenario and is the expected usage.
    DECLARE @{existingIdsTable} TABLE({Tables.Record.Id.Name} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
    DECLARE @{prunedIdsTable} TABLE({Tables.Record.Id.Name} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax} NOT NULL)
	IF (@{InputParamName.ExistingRecordStrategy} <> '{ExistingRecordStrategy.None}')
	BEGIN
		INSERT INTO @{existingIdsTable}
	    SELECT [{Tables.Record.Id.Name}] FROM [{streamName}].[{Tables.Record.Table.Name}]
		WHERE
			([{Tables.Record.StringSerializedId.Name}] = @{InputParamName.StringSerializedId})
			AND
			(
				(
					(@{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.DoNotWriteIfFoundById}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.ThrowIfFoundById}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.PruneIfFoundById}')
				)
				OR
				(
					(@{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.DoNotWriteIfFoundByIdAndType}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.ThrowIfFoundByIdAndType}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.PruneIfFoundByIdAndType}')
					AND
					(
						(
								@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
							AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionId}
							AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionId}
						)
						OR
						(
								@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
							AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionId}
							AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionId}
						)
					)
				)
				OR
				(
					(@{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.DoNotWriteIfFoundByIdAndTypeAndContent}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.ThrowIfFoundByIdAndTypeAndContent}')
					AND
					(
						(
							    @{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.Any}'
							AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionId}
							AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionId}
							AND ([{Tables.Record.StringSerializedObject.Name}] = @{InputParamName.StringSerializedObject} OR [{Tables.Record.BinarySerializedObject.Name}] = @{InputParamName.BinarySerializedObject})
						)
						OR
						(
								@{InputParamName.VersionMatchStrategy} = '{VersionMatchStrategy.SpecifiedVersion}'
							AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionId}
							AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionId}
							AND ([{Tables.Record.StringSerializedObject.Name}] = @{InputParamName.StringSerializedObject} OR [{Tables.Record.BinarySerializedObject.Name}] = @{InputParamName.BinarySerializedObject})
						)
					)
				)
			)
	END

    DECLARE @{existingIdsCount} {Tables.Record.Id.SqlDataType.DeclarationInSqlSyntax}
	SELECT @{existingIdsCount} = COUNT(*) FROM @{existingIdsTable}
    IF (@{existingIdsCount} > 0)
	BEGIN
        -- STRING_AGG errors (but does not interrupt execution) if the bytes exceed 8000 which can easily happen with less than 1000 ids.
        --    Instead of switching to an [XML] return type to accomodate all ids, we are capping to the most recent matching record id.
		SELECT @{OutputParamName.ExistingRecordIdsCsv} = MAX([{Tables.Record.Id.Name}]) FROM @{existingIdsTable}
	END

    IF (@{existingIdsCount} > 1)
	BEGIN
        -- Since we cannot return all records, if there is more than one match then the list is suffixed with '-1' as a continuation token
        --    indicating there are multiple matches for purposes of debugging unexpected results.
	    SELECT @{OutputParamName.ExistingRecordIdsCsv} = @{OutputParamName.ExistingRecordIdsCsv} + ',-1'
	END

	IF (@{OutputParamName.ExistingRecordIdsCsv} IS NULL OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.PruneIfFoundById}' OR @{InputParamName.ExistingRecordStrategy} = '{ExistingRecordStrategy.PruneIfFoundByIdAndType}')
	BEGIN
		DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.SqlDataType.DeclarationInSqlSyntax}
		SET @RecordCreatedUtc = GETUTCDATE()

		{insertRowsBlock}

	  IF (@{OutputParamName.ExistingRecordIdsCsv} IS NOT NULL)
	  BEGIN
		-- must be a prune scenario to get here as this is checked above...
		IF (@{existingIdsCount} >= (@{InputParamName.RecordRetentionCount} - 1))
		BEGIN
			-- have records to prune
			INSERT INTO @{prunedIdsTable}
			SELECT TOP (@{existingIdsCount} - @{InputParamName.RecordRetentionCount} + 1)
				[{Tables.Record.Id.Name}] FROM @{existingIdsTable}
				ORDER BY [{Tables.Record.Id.Name}] ASC

			BEGIN TRANSACTION [{pruneTransaction}]
			BEGIN TRY
				DELETE FROM [{streamName}].[{Tables.HandlingTag.Table.Name}] WHERE [{Tables.HandlingTag.HandlingId.Name}]
					IN (
						SELECT [{Tables.Handling.Id.Name}] FROM [{streamName}].[{Tables.Handling.Table.Name}]
						WHERE [{Tables.Handling.RecordId.Name}] IN (SELECT [{Tables.Tag.Id.Name}] FROM @{prunedIdsTable})
					)
				DELETE FROM [{streamName}].[{Tables.Handling.Table.Name}]
						WHERE [{Tables.Handling.RecordId.Name}] IN (SELECT [{Tables.Tag.Id.Name}] FROM @{prunedIdsTable})
				DELETE FROM [{streamName}].[{Tables.RecordTag.Table.Name}]
						WHERE [{Tables.RecordTag.RecordId.Name}] IN (SELECT [{Tables.Tag.Id.Name}] FROM @{prunedIdsTable})
				DELETE FROM [{streamName}].[{Tables.Record.Table.Name}]
						WHERE [{Tables.Record.Id.Name}] IN (SELECT [{Tables.Tag.Id.Name}] FROM @{prunedIdsTable})

				COMMIT TRANSACTION [{pruneTransaction}]
		    END TRY
		    BEGIN CATCH
                DECLARE @ThrowMessage nvarchar(max),
                        @ErrorMessage nvarchar(max),
                        @ErrorSeverity int,
                        @ErrorState int
              
                SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
                SELECT @ThrowMessage = @ErrorMessage + '; ErrorSeverity=' + cast(@ErrorSeverity as nvarchar(20)) + '; ErrorState=' + cast(@ErrorState as nvarchar(20))

		        IF (@@trancount > 0)
		        BEGIN
		           ROLLBACK TRANSACTION [{pruneTransaction}]
		        END;

		        THROW {GeneralPurposeErrorNumberForThrowStatement}, @ThrowMessage, {GeneralPurposeErrorStateForThrowStatement}
		    END CATCH

        SELECT @{OutputParamName.PrunedRecordIdsCsv} = STRING_AGG([{Tables.Record.Id.Name}], ',') FROM @{prunedIdsTable}

		END -- have enough records to delete - the actual prune
	  END -- have existing records - check for pruning
	END -- need to insert a record
END
			");

                    return result;
                }
            }
        }
    }
}
