DROP PROCEDURE IF EXISTS [Example].[PutHandling]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[PutHandling](
  @Concern AS [NVARCHAR](450)
, @Details AS [NVARCHAR](MAX)
, @RecordId AS [BIGINT]
, @NewStatus AS [VARCHAR](50)
, @AcceptableCurrentStatusesXml AS [Xml]
, @TagIdsXml AS [Xml]
, @Id AS [BIGINT] OUTPUT
)
AS
BEGIN

DECLARE @CurrentStatus [VARCHAR](50)
SELECT TOP 1 @CurrentStatus = Status
    FROM [Example].[Handling]
    WHERE [Concern] = @Concern AND [RecordId] = @RecordId
    ORDER BY [Id] DESC

IF @CurrentStatus IS NULL
BEGIN
    SET @CurrentStatus = 'Requested'
END
--TODO: should we guard against this changing while inserting? (exclusive table lock for a time to live, et al)
DECLARE @CurrentStatusAccepted BIT

SELECT @CurrentStatusAccepted = 1 FROM
[Example].[GetTagsTableVariableFromTagsXml](@AcceptableCurrentStatusesXml) t
WHERE [TagValue] = @CurrentStatus

IF (@CurrentStatusAccepted = 0)
BEGIN
      DECLARE @NotValidStatusErrorMessage nvarchar(max), 
              @NotValidStatusErrorSeverity int, 
              @NotValidStatusErrorState int

      SELECT @NotValidStatusErrorMessage = 'Invalid current status: ' + @CurrentStatus + ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @NotValidStatusErrorSeverity = ERROR_SEVERITY(), @NotValidStatusErrorState = ERROR_STATE()
      RAISERROR (@NotValidStatusErrorMessage, @NotValidStatusErrorSeverity, @NotValidStatusErrorState)
END
 
BEGIN TRANSACTION [PutHandlingTransaction]
  BEGIN TRY
	      DECLARE @RecordCreatedUtc [DATETIME2]
	      SET @RecordCreatedUtc = GETUTCDATE()
	      INSERT INTO [Example].[Handling] WITH (TABLOCKX) -- get an exclusive lock to prevent other from doing same
           (
		    [Concern]
		  , [RecordId]
		  , [Status]
		  , [Details]
		  , [RecordCreatedUtc]
		  ) VALUES (
	        @Concern
	      , @RecordId
	      , @NewStatus
	      , @Details
		  , @RecordCreatedUtc
		  )

	      SET @Id = SCOPE_IDENTITY()
          
	      INSERT INTO [Example].[HandlingTag] (
		    [HandlingId]
		  , [TagId]
		  , [RecordCreatedUtc]
		  )
         SELECT 
  		    @Id
		  , t.[TagValue]
		  , @RecordCreatedUtc
         FROM [Example].[GetTagsTableVariableFromTagIdsXml](@TagIdsXml) t

      COMMIT TRANSACTION [PutHandlingTransaction]

  END TRY

  BEGIN CATCH
      SET @Id = NULL
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [PutHandlingTransaction]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END
GO

