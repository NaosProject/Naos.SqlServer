DROP PROCEDURE IF EXISTS [Example].[PutRecord]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[PutRecord](
  @SerializerRepresentationId AS [INT]
, @IdentifierTypeWithoutVersionId AS [INT]
, @IdentifierTypeWithVersionId AS [INT]
, @ObjectTypeWithoutVersionId AS [INT]
, @ObjectTypeWithVersionId AS [INT]
, @StringSerializedId AS [NVARCHAR](450)
, @StringSerializedObject AS [NVARCHAR](MAX)
, @ObjectDateTimeUtc AS [DATETIME2]
, @TagIdsXml AS xml
, @ExistingRecordEncounteredStrategy AS [VARCHAR](50)
, @TypeVersionMatchStrategy AS [VARCHAR](50)
, @Id AS [BIGINT] OUTPUT
, @ExistingRecordId AS [BIGINT] OUTPUT
)
AS
BEGIN
BEGIN TRANSACTION [PutRecordTransaction]
  BEGIN TRY
		IF (@ExistingRecordEncounteredStrategy <> 'None')
		BEGIN
		    SELECT @ExistingRecordId = [Id] FROM [Example].[Record]
			WHERE
				(
					(@ExistingRecordEncounteredStrategy = 'DoNotWriteIfFoundById' OR @ExistingRecordEncounteredStrategy = 'ThrowIfFoundById')
					AND
					([StringSerializedId] = @StringSerializedId)
				)
				OR
				(
					(@ExistingRecordEncounteredStrategy = 'DoNotWriteIfFoundByIdAndType' OR @ExistingRecordEncounteredStrategy = 'ThrowIfFoundByIdAndType')
					AND
					([StringSerializedId] = @StringSerializedId)
					AND
					(
						(
								@TypeVersionMatchStrategy = 'Any'
							AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionId
							AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionId
						)
						OR
						(
								@TypeVersionMatchStrategy = 'Specific'
							AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionId
							AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionId
						)
					)
				)
				OR
				(
					(@ExistingRecordEncounteredStrategy = 'DoNotWriteIfFoundByIdAndTypeAndContent' OR @ExistingRecordEncounteredStrategy = 'ThrowIfFoundByIdAndTypeAndContent')
					AND
					([StringSerializedId] = @StringSerializedId)
					AND
					(
						(
							    @TypeVersionMatchStrategy = 'Any'
							AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionId
							AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionId
						)
						OR
						(
								@TypeVersionMatchStrategy = 'Specific'
							AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionId
							AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionId
						)
					)
					AND
					([StringSerializedObject] = @StringSerializedObject)
				)
		END
		IF (@ExistingRecordId IS NOT NULL)
		BEGIN
			SET @Id = -1
		END
		ELSE
		BEGIN
		  DECLARE @RecordCreatedUtc [DATETIME2]
		  SET @RecordCreatedUtc = GETUTCDATE()
		  INSERT INTO [Example].[Record] (
			  [IdentifierTypeWithoutVersionId]
			, [IdentifierTypeWithVersionId]
			, [ObjectTypeWithoutVersionId]
			, [ObjectTypeWithVersionId]
			, [SerializerRepresentationId]
			, [StringSerializedId]
			, [StringSerializedObject]
			, [ObjectDateTimeUtc]
			, [RecordCreatedUtc]
			) VALUES (
			  @IdentifierTypeWithoutVersionId
			, @IdentifierTypeWithVersionId
			, @ObjectTypeWithoutVersionId
			, @ObjectTypeWithVersionId
			, @SerializerRepresentationId
			, @StringSerializedId
			, @StringSerializedObject
			, @ObjectDateTimeUtc
			, @RecordCreatedUtc
			)

	      SET @Id = SCOPE_IDENTITY()
		  
	      INSERT INTO [Example].[RecordTag](
		    [RecordId]
		  , [TagId]
		  , [RecordCreatedUtc])
	     SELECT 
  		    @Id
		  , t.[TagValue]
		  , @RecordCreatedUtc
	     FROM [Example].[GetTagsTableVariableFromTagsXml](@TagIdsXml) t
		END
	    COMMIT TRANSACTION [PutRecordTransaction]

	  END TRY
	  BEGIN CATCH
	      DECLARE @ErrorMessage nvarchar(max), 
	              @ErrorSeverity int, 
	              @ErrorState int

	      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

	      IF (@@trancount > 0)
	      BEGIN
	         ROLLBACK TRANSACTION [PutRecordTransaction]
	      END
	    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	  END CATCH
END
			
GO

