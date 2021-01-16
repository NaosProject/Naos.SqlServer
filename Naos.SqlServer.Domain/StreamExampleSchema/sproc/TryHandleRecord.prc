DROP PROCEDURE IF EXISTS [Example].[TryHandleRecord]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[TryHandleRecord](
  @Concern AS [NVARCHAR](450)
, @Details AS [NVARCHAR](MAX)
, @OrderRecordsStrategy AS [VARCHAR](50)
, @IdentifierTypeWithoutVersionIdQuery AS [INT]
, @IdentifierTypeWithVersionIdQuery AS [INT]
, @ObjectTypeWithoutVersionIdQuery AS [INT]
, @ObjectTypeWithVersionIdQuery AS [INT]
, @TypeVersionMatchStrategy AS varchar(10)
, @TagIdsForEntryXml AS [NVARCHAR](MAX)
, @InheritRecordTags AS [INT]
, @Id AS [BIGINT] OUTPUT
, @InternalRecordId AS [BIGINT] OUTPUT
, @SerializerRepresentationId AS [INT] OUTPUT
, @IdentifierTypeWithVersionId AS [INT] OUTPUT
, @ObjectTypeWithVersionId AS [INT] OUTPUT
, @StringSerializedId AS [NVARCHAR](450) OUTPUT
, @StringSerializedObject AS [NVARCHAR](MAX) OUTPUT
, @ObjectDateTime AS [DATETIME2] OUTPUT
, @RecordDateTime AS [DATETIME2] OUTPUT
, @TagIdsXml AS [NVARCHAR](MAX) OUTPUT
, @ShouldHandle AS [INT] OUTPUT
)
AS
BEGIN
    DECLARE @BlockedStatus [VARCHAR](50)
	SELECT TOP 1 @BlockedStatus = [Status] FROM [Example].[Handling]
	WHERE [Concern] = 'RecordHandling'

	IF ((@BlockedStatus IS NULL) OR (@BlockedStatus <> 'Blocked'))
	BEGIN
		DECLARE @RecordToHandleId [BIGINT]
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
		BEGIN TRANSACTION [Transaction]
	  BEGIN TRY
	  IF (@OrderRecordsStrategy = 'ByInternalRecordIdAscending')
      BEGIN
		  -- See if any reprocessing is needed
		  IF (@RecordToHandleId IS NULL)
		  BEGIN
		      SELECT TOP 1 @RecordToHandleId = h.[RecordId]
			  FROM [Example].[Handling] h
              INNER JOIN [Example].[Record] r ON r.[Id] = h.[RecordId]
		      WHERE h.[Concern] = @Concern
		        AND (h.[Status] = 'RetryFailed' OR h.[Status] = 'CanceledRunning' OR h.[Status] = 'SelfCanceledRunning')
				AND (SELECT TOP 1 [Status] FROM [Example].[Handling] i WHERE i.RecordId = h.RecordId ORDER BY i.Id DESC) = h.Status
			    AND (
			            -- No Type filter at all
			            (@IdentifierTypeWithoutVersionIdQuery IS NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND @ObjectTypeWithVersionIdQuery IS NULL)
			            OR
			            -- Specific Only Id
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery)
			            OR
			            -- Specific Only Object
			            (@TypeVersionMatchStrategy = 'Specific' AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Specific Both
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Any Only Id
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery)
			            OR
			            -- Any Only Object
			            (@TypeVersionMatchStrategy = 'Any' AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND @IdentifierTypeWithoutVersionIdQuery IS NULL AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			            OR
			            -- Any Both
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			        )
			  ORDER BY h.[Id] ASC
		  END

		  -- See if any new records
		  IF (@RecordToHandleId IS NULL)
		  BEGIN
		      SELECT TOP 1 @RecordToHandleId = r.[Id]
		      FROM [Example].[Record] r
			  LEFT JOIN [Example].[Handling] h
		      ON r.[Id] = h.[RecordId] AND h.[Concern] = @Concern
		      WHERE h.[Id] IS NULL
			    AND (
			            -- No Type filter at all
			            (@IdentifierTypeWithoutVersionIdQuery IS NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND @ObjectTypeWithVersionIdQuery IS NULL)
			            OR
			            -- Specific Only Id
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery)
			            OR
			            -- Specific Only Object
			            (@TypeVersionMatchStrategy = 'Specific' AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Specific Both
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Any Only Id
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery)
			            OR
			            -- Any Only Object
			            (@TypeVersionMatchStrategy = 'Any' AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND @IdentifierTypeWithoutVersionIdQuery IS NULL AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			            OR
			            -- Any Both
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			        )
			  ORDER BY r.[Id] ASC
		  END
      END
      ELSE IF (@OrderRecordsStrategy = 'ByInternalRecordIdDescending')
		  -- See if any new records
		  IF (@RecordToHandleId IS NULL)
		  BEGIN
		      SELECT TOP 1 @RecordToHandleId = r.[Id]
		      FROM [Example].[Record] r
			  LEFT JOIN [Example].[Handling] h
		      ON r.[Id] = h.[RecordId] AND h.[Concern] = @Concern
		      WHERE h.[Id] IS NULL
			    AND (
			            -- No Type filter at all
			            (@IdentifierTypeWithoutVersionIdQuery IS NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND @ObjectTypeWithVersionIdQuery IS NULL)
			            OR
			            -- Specific Only Id
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery)
			            OR
			            -- Specific Only Object
			            (@TypeVersionMatchStrategy = 'Specific' AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Specific Both
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Any Only Id
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery)
			            OR
			            -- Any Only Object
			            (@TypeVersionMatchStrategy = 'Any' AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND @IdentifierTypeWithoutVersionIdQuery IS NULL AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			            OR
			            -- Any Both
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			        )
			  ORDER BY r.[Id] ASC
		  END

		  -- See if any reprocessing is needed
		  IF (@RecordToHandleId IS NULL)
		  BEGIN
		      SELECT TOP 1 @RecordToHandleId = h.[RecordId]
			  FROM [Example].[Handling] h
              INNER JOIN [Example].[Record] r ON r.[Id] = h.[RecordId]
		      WHERE h.[Concern] = @Concern
		        AND (h.[Status] = 'RetryFailed' OR h.[Status] = 'CanceledRunning' OR h.[Status] = 'SelfCanceledRunning')
				AND (SELECT TOP 1 [Status] FROM [Example].[Handling] i WHERE i.RecordId = h.RecordId ORDER BY i.Id DESC) = h.Status
			    AND (
			            -- No Type filter at all
			            (@IdentifierTypeWithoutVersionIdQuery IS NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND @ObjectTypeWithVersionIdQuery IS NULL)
			            OR
			            -- Specific Only Id
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery)
			            OR
			            -- Specific Only Object
			            (@TypeVersionMatchStrategy = 'Specific' AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND @IdentifierTypeWithVersionIdQuery IS NULL AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Specific Both
			            (@TypeVersionMatchStrategy = 'Specific' AND @IdentifierTypeWithVersionIdQuery IS NOT NULL AND @ObjectTypeWithVersionIdQuery IS NOT NULL AND [IdentifierTypeWithVersionId] = @IdentifierTypeWithVersionIdQuery AND [ObjectTypeWithVersionId] = @ObjectTypeWithVersionIdQuery)
			            OR
			            -- Any Only Id
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery)
			            OR
			            -- Any Only Object
			            (@TypeVersionMatchStrategy = 'Any' AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND @IdentifierTypeWithoutVersionIdQuery IS NULL AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			            OR
			            -- Any Both
			            (@TypeVersionMatchStrategy = 'Any' AND @IdentifierTypeWithoutVersionIdQuery IS NOT NULL AND @ObjectTypeWithoutVersionIdQuery IS NOT NULL AND [IdentifierTypeWithoutVersionId] = @IdentifierTypeWithoutVersionIdQuery AND [ObjectTypeWithoutVersionId] = @ObjectTypeWithoutVersionIdQuery)
			        )
			  ORDER BY h.[Id] ASC
		  END
		  ELSE
		  BEGIN
		      DECLARE @NotValidOrderRecordsStrategyErrorMessage nvarchar(max), 
		              @NotValidOrderRecordsStrategyErrorSeverity int, 
		              @NotValidOrderRecordsStrategyErrorState int

		      SELECT @NotValidOrderRecordsStrategyErrorMessage = 'Unsupported OrderRecordsStrategy: ' + @OrderRecordsStrategy + ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @NotValidOrderRecordsStrategyErrorSeverity = ERROR_SEVERITY(), @NotValidOrderRecordsStrategyErrorState = ERROR_STATE()
		      RAISERROR (@NotValidOrderRecordsStrategyErrorMessage, @NotValidOrderRecordsStrategyErrorSeverity, @NotValidOrderRecordsStrategyErrorState)
		  END

	IF (@RecordToHandleId IS NOT NULL)
	BEGIN
		DECLARE @UnionedIfNecessaryTagIdsXml [XML]
		
        SELECT @UnionedIfNecessaryTagIdsXml = (
        SELECT
				  ROW_NUMBER() OVER (ORDER BY [Id]) AS [@Key]
				, [Id] AS [@Value]
        FROM
			(
                SELECT DISTINCT [Id] FROM
				(
					SELECT [TagValue] AS [Id]
				    FROM [Example].[GetTagsTableVariableFromTagIdsXml](@TagIdsForEntryXml)
			        UNION ALL
					SELECT [TagId] AS [Id]
                    FROM [Example].[RecordTag]
					WHERE @InheritRecordTags = 1 AND [RecordId] = @RecordToHandleId
				) AS u
			) AS d
	    FOR XML PATH ('Tag'), ROOT('Tags'))

		EXEC [Example].[PutHandling] 
		@Concern = @Concern, 
		@Details = @Details, 
		@RecordId = @RecordToHandleId, 
		@NewStatus = 'Running', 
		@AcceptableCurrentStatusesXml = '<Tags><Tag Key="1" Value="None"/></Tags>', 
		@TagIdsXml = @UnionedIfNecessaryTagIdsXml, 
		@Id = @Id OUTPUT

	    SET @ShouldHandle = 1
		END

	      COMMIT TRANSACTION [Transaction]
		  END TRY
	      BEGIN CATCH
		      DECLARE @ErrorMessage nvarchar(max), 
		              @ErrorSeverity int, 
		              @ErrorState int

		      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

		      IF (@@trancount > 0)
		      BEGIN
		         ROLLBACK TRANSACTION [Transaction]
		      END
			  RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	      END CATCH
	END

    IF (@ShouldHandle = 1)
	BEGIN
	    SELECT TOP 1
		   @SerializerRepresentationId = [SerializerRepresentationId]
		 , @IdentifierTypeWithVersionId = [IdentifierTypeWithVersionId]
		 , @ObjectTypeWithVersionId = [ObjectTypeWithVersionId]
		 , @StringSerializedId = [StringSerializedId]
		 , @StringSerializedObject = [StringSerializedObject]
		 , @InternalRecordId = [Id]
		 , @RecordDateTime = [RecordCreatedUtc]
		 , @ObjectDateTime = [ObjectDateTimeUtc]
		FROM [Example].[Record]
		WHERE [Id] = @RecordToHandleId

	    SELECT @TagIdsXml = (SELECT
			ROW_NUMBER() OVER (ORDER BY [Id]) AS [@Key],
			Id AS [@Value]
		FROM [Example].[RecordTag]
		WHERE [RecordId] = @RecordToHandleId
		FOR XML PATH ('Tag'), ROOT('Tags'))
	END
    ELSE
	BEGIN
		SET @ShouldHandle = 0
		SET @Id = -1
		SET @InternalRecordId = -1
		SET @SerializerRepresentationId = -1
		SET @IdentifierTypeWithVersionId = -1
		SET @ObjectTypeWithVersionId = -1
		SET @StringSerializedId = 'Fake'
		SET @StringSerializedObject = 'Fake'
		SET @ObjectDateTime = GETUTCDATE()
		SET @RecordDateTime = GETUTCDATE()
		SET @TagIdsXml = null
	END
END

			
GO

