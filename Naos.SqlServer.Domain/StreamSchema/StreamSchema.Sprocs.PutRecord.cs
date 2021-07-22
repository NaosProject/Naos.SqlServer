// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamSchema.Sprocs.PutRecord.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using Naos.Protocol.Domain;
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
            /// Stored procedure: PutRecord.
            /// </summary>
            public static class PutRecord
            {
                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>The name.</value>
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
                    ExistingRecordEncounteredStrategy,

                    /// <summary>
                    /// The number of records to keep if using a pruning <see cref="ExistingRecordEncounteredStrategy"/>.
                    /// </summary>
                    RecordRetentionCount,

                    /// <summary>
                    /// The type version match strategy.
                    /// </summary>
                    TypeVersionMatchStrategy,
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
                /// <param name="existingRecordEncounteredStrategy">Existing record encountered strategy.</param>
                /// <param name="recordRetentionCount">Number of records to keep if using a pruning <paramref name="existingRecordEncounteredStrategy"/>.</param>
                /// <param name="typeVersionMatchStrategy">Type version match strategy.</param>
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
                    ExistingRecordEncounteredStrategy existingRecordEncounteredStrategy,
                    int? recordRetentionCount,
                    TypeVersionMatchStrategy typeVersionMatchStrategy)
                {
                    var sprocName = Invariant($"[{streamName}].{nameof(PutRecord)}");

                    var parameters = new List<SqlParameterRepresentationBase>()
                                     {
                                         new SqlInputParameterRepresentation<int>(nameof(InputParamName.SerializerRepresentationId), Tables.SerializerRepresentation.Id.DataType, serializerRepresentation.Id),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.DataType, identifierType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.IdentifierTypeWithVersionId), Tables.TypeWithVersion.Id.DataType, identifierType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithoutVersionId), Tables.TypeWithoutVersion.Id.DataType, objectType?.IdWithoutVersion),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.ObjectTypeWithVersionId), Tables.TypeWithVersion.Id.DataType, objectType?.IdWithVersion),
                                         new SqlInputParameterRepresentation<long?>(nameof(InputParamName.InternalRecordId), Tables.Record.Id.DataType, internalRecordId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedId), Tables.Record.StringSerializedId.DataType, serializedObjectId),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.StringSerializedObject), Tables.Record.StringSerializedObject.DataType, serializedObjectString),
                                         new SqlInputParameterRepresentation<byte[]>(nameof(InputParamName.BinarySerializedObject), Tables.Record.BinarySerializedObject.DataType, serializedObjectBytes),
                                         new SqlInputParameterRepresentation<DateTime?>(nameof(InputParamName.ObjectDateTimeUtc), Tables.Record.ObjectDateTimeUtc.DataType, objectDateTimeUtc),
                                         new SqlInputParameterRepresentation<string>(nameof(InputParamName.TagIdsCsv), Tables.Record.TagIdsCsv.DataType, tagIdsCsv),
                                         new SqlInputParameterRepresentation<ExistingRecordEncounteredStrategy>(nameof(InputParamName.ExistingRecordEncounteredStrategy), new StringSqlDataTypeRepresentation(false, 50), existingRecordEncounteredStrategy),
                                         new SqlInputParameterRepresentation<int?>(nameof(InputParamName.RecordRetentionCount), new IntSqlDataTypeRepresentation(), recordRetentionCount),
                                         new SqlInputParameterRepresentation<TypeVersionMatchStrategy>(nameof(InputParamName.TypeVersionMatchStrategy), new StringSqlDataTypeRepresentation(false, 50), typeVersionMatchStrategy),
                                         new SqlOutputParameterRepresentation<long?>(nameof(OutputParamName.Id), Tables.Record.Id.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.ExistingRecordIdsCsv), Tables.Record.TagIdsCsv.DataType),
                                         new SqlOutputParameterRepresentation<string>(nameof(OutputParamName.PrunedRecordIdsCsv), Tables.Record.TagIdsCsv.DataType),
                                     };

                    var parameterNameToDetailsMap = parameters.ToDictionary(k => k.Name, v => v);

                    var result = new ExecuteStoredProcedureOp(sprocName, parameterNameToDetailsMap);

                    return result;
                }

                /// <summary>
                /// Builds the creation script for put stored procedure.
                /// </summary>
                /// <param name="streamName">Name of the stream.</param>
                /// <param name="recordTagAssociationManagementStrategy">The record tag association management strategy.</param>
                /// <param name="asAlter">An optional value indicating whether or not to generate a ALTER versus CREATE; DEFAULT is false and will generate a CREATE script.</param>
                /// <returns>System.String.</returns>
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
                    var createOrModify = asAlter ? "ALTER" : "CREATE";
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
	      DECLARE @PruneErrorMessage nvarchar(max), 
	              @PruneErrorSeverity int, 
	              @PruneErrorState int

	      SELECT @PruneErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @PruneErrorSeverity = ERROR_SEVERITY(), @PruneErrorState = ERROR_STATE()

	      IF (@@trancount > 0)
	      BEGIN
	         ROLLBACK TRANSACTION [{transaction}]
	      END
	    RAISERROR (@PruneErrorMessage, @PruneErrorSeverity, @PruneErrorState)
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
  @{InputParamName.SerializerRepresentationId} AS {Tables.SerializerRepresentation.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.IdentifierTypeWithVersionId} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithoutVersionId} AS {Tables.TypeWithoutVersion.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectTypeWithVersionId} AS {Tables.TypeWithVersion.Id.DataType.DeclarationInSqlSyntax}
  @{InputParamName.InternalRecordId} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedId} AS {Tables.Record.StringSerializedId.DataType.DeclarationInSqlSyntax}
, @{InputParamName.StringSerializedObject} AS {Tables.Record.StringSerializedObject.DataType.DeclarationInSqlSyntax}
, @{InputParamName.BinarySerializedObject} AS {Tables.Record.BinarySerializedObject.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ObjectDateTimeUtc} AS {Tables.Record.ObjectDateTimeUtc.DataType.DeclarationInSqlSyntax}
, @{InputParamName.TagIdsCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax}
, @{InputParamName.ExistingRecordEncounteredStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{InputParamName.RecordRetentionCount} AS {new IntSqlDataTypeRepresentation().DeclarationInSqlSyntax}
, @{InputParamName.TypeVersionMatchStrategy} AS {new StringSqlDataTypeRepresentation(false, 50).DeclarationInSqlSyntax}
, @{OutputParamName.Id} AS {Tables.Record.Id.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.ExistingRecordIdsCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax} OUTPUT
, @{OutputParamName.PrunedRecordIdsCsv} AS {Tables.Record.TagIdsCsv.DataType.DeclarationInSqlSyntax} OUTPUT
)
AS
BEGIN
    -- If two actors try to both insert for the same ID with the '{nameof(ExistingRecordEncounteredStrategy)}'
	--	   set to e.g. {ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndTypeAndContent}; they could both
	--     write the same payload; this does work in a single actor re-entrant scenario and is the expected usage.
    DECLARE @{existingIdsTable} TABLE({Tables.Record.Id.Name} {Tables.Record.Id.DataType.DeclarationInSqlSyntax} NOT NULL)
    DECLARE @{prunedIdsTable} TABLE({Tables.Record.Id.Name} {Tables.Record.Id.DataType.DeclarationInSqlSyntax} NOT NULL)
	IF (@{InputParamName.ExistingRecordEncounteredStrategy} <> '{ExistingRecordEncounteredStrategy.None}')
	BEGIN
		INSERT INTO @{existingIdsTable}
	    SELECT [{Tables.Record.Id.Name}] FROM [{streamName}].[{Tables.Record.Table.Name}]
		WHERE				
			([{Tables.Record.StringSerializedId.Name}] = @{InputParamName.StringSerializedId})
			AND
			(
				(
					(@{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.DoNotWriteIfFoundById}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.ThrowIfFoundById}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.PruneIfFoundById}')
				)
				OR
				(
					(@{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndType}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndType}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.PruneIfFoundByIdAndType}')
					AND
					(
						(
								@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}'
							AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionId}
							AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionId}
						)
						OR
						(
								@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}'
							AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionId}
							AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionId}
						)
					)
				)
				OR
				(
					(@{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.DoNotWriteIfFoundByIdAndTypeAndContent}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.ThrowIfFoundByIdAndTypeAndContent}')
					AND
					(
						(
							    @{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Any}'
							AND [{Tables.Record.IdentifierTypeWithoutVersionId.Name}] = @{InputParamName.IdentifierTypeWithoutVersionId}
							AND [{Tables.Record.ObjectTypeWithoutVersionId.Name}] = @{InputParamName.ObjectTypeWithoutVersionId}
							AND ([{Tables.Record.StringSerializedObject.Name}] = @{InputParamName.StringSerializedObject} OR [{Tables.Record.BinarySerializedObject.Name}] = @{InputParamName.BinarySerializedObject})
						)
						OR
						(
								@{InputParamName.TypeVersionMatchStrategy} = '{TypeVersionMatchStrategy.Specific}'
							AND [{Tables.Record.IdentifierTypeWithVersionId.Name}] = @{InputParamName.IdentifierTypeWithVersionId}
							AND [{Tables.Record.ObjectTypeWithVersionId.Name}] = @{InputParamName.ObjectTypeWithVersionId}
							AND ([{Tables.Record.StringSerializedObject.Name}] = @{InputParamName.StringSerializedObject} OR [{Tables.Record.BinarySerializedObject.Name}] = @{InputParamName.BinarySerializedObject})
						)
					)
				)
			)
	END

	IF EXISTS (SELECT TOP 1 * FROM @{existingIdsTable})
	BEGIN
        SELECT @{OutputParamName.ExistingRecordIdsCsv} = STRING_AGG([{Tables.Tag.Id.Name}], ',') FROM @{existingIdsTable}
	END

	IF (@{OutputParamName.ExistingRecordIdsCsv} IS NULL OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.PruneIfFoundById}' OR @{InputParamName.ExistingRecordEncounteredStrategy} = '{ExistingRecordEncounteredStrategy.PruneIfFoundByIdAndType}')
	BEGIN
		DECLARE @{recordCreatedUtc} {Tables.Record.RecordCreatedUtc.DataType.DeclarationInSqlSyntax}
		SET @RecordCreatedUtc = GETUTCDATE()

		{insertRowsBlock}

	  IF (@{OutputParamName.ExistingRecordIdsCsv} IS NOT NULL)
	  BEGIN
		-- must be a prune scenario to get here as this is checked above...
        DECLARE @{existingIdsCount} {Tables.Record.Id.DataType.DeclarationInSqlSyntax}
		SELECT @{existingIdsCount} = COUNT(*) FROM @{existingIdsTable}
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
		        DECLARE @ErrorMessage nvarchar(max), 
		                @ErrorSeverity int, 
		                @ErrorState int

		        SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

		        IF (@@trancount > 0)
		        BEGIN
		           ROLLBACK TRANSACTION [{pruneTransaction}]
		        END
		      RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
		    END CATCH

        SELECT @{OutputParamName.PrunedRecordIdsCsv} = STRING_AGG([{Tables.Tag.Id.Name}], ',') FROM @{prunedIdsTable}

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
