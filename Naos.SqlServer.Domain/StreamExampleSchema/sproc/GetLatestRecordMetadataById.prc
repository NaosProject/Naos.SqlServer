DROP PROCEDURE IF EXISTS [Example].[GetLatestRecordMetadataById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetLatestRecordMetadataById](
  @StringSerializedId AS [NVARCHAR](450)
, @IdentifierTypeWithoutVersionIdQuery AS [INT]
, @IdentifierTypeWithVersionIdQuery AS [INT]
, @ObjectTypeWithoutVersionIdQuery AS [INT]
, @ObjectTypeWithVersionIdQuery AS [INT]
, @TypeVersionMatchStrategy AS varchar(10)
, @ExistingRecordNotEncounteredStrategy AS varchar(50)
, @InternalRecordId AS [BIGINT] OUTPUT
, @SerializerRepresentationId AS [INT] OUTPUT
, @IdentifierTypeWithVersionId AS [INT] OUTPUT
, @ObjectTypeWithVersionId AS [INT] OUTPUT
, @ObjectDateTime AS [DATETIME2] OUTPUT
, @RecordDateTime AS [DATETIME2] OUTPUT
, @TagIdsXml AS [NVARCHAR](MAX) OUTPUT
)
AS
BEGIN
    SELECT TOP 1
	   @SerializerRepresentationId = [SerializerRepresentationId]
	 , @IdentifierTypeWithVersionId = [IdentifierTypeWithVersionId]
	 , @ObjectTypeWithVersionId = [ObjectTypeWithVersionId]
	 , @InternalRecordId = [Id]
	 , @RecordDateTime = [RecordCreatedUtc]
	 , @ObjectDateTime = [ObjectDateTimeUtc]
	FROM [Example].[Record]
	WHERE [StringSerializedId] = @StringSerializedId
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
	ORDER BY [Id] DESC
    IF (@InternalRecordId IS NULL)
    BEGIN
        SET @SerializerRepresentationId = -1
	    SET @IdentifierTypeWithVersionId = -1
	    SET @ObjectTypeWithVersionId = -1
	    SET @InternalRecordId = -1
	    SET @ObjectDateTime = NULL
	    SET @TagIdsXml = NULL
	    SET @RecordDateTime = GETUTCDATE()
    END
    ELSE
    BEGIN
        SELECT @TagIdsXml = (SELECT
              ROW_NUMBER() OVER (ORDER BY [Id]) AS [@Key]
		    , [TagId] AS [@Value]
	    FROM [Example].[RecordTag]
	    WHERE [RecordId] = @InternalRecordId
	    FOR XML PATH ('Tag'), ROOT('Tags'))
    END
END

			
GO

