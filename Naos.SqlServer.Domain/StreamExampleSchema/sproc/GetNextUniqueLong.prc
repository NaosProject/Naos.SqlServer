DROP PROCEDURE IF EXISTS [Example].[GetNextUniqueLong]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetNextUniqueLong](
  @Value [BIGINT] OUTPUT
)
AS
BEGIN

BEGIN TRANSACTION [GetNextUniqueLongTran]
  BEGIN TRY
	  BEGIN
	      INSERT INTO [Example].[NextUniqueLong] WITH (TABLOCKX) (
		    [RecordCreatedUtc]
		  ) VALUES (
		    GETUTCDATE()
		  )

	      SET @Value = SCOPE_IDENTITY()
	  END

      COMMIT TRANSACTION [GetNextUniqueLongTran]

  END TRY

  BEGIN CATCH
      SET @Value = NULL
      DECLARE @ErrorMessage nvarchar(max), 
              @ErrorSeverity int, 
              @ErrorState int

      SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

      IF (@@trancount > 0)
      BEGIN
         ROLLBACK TRANSACTION [GetNextUniqueLongTran]
      END
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
  END CATCH
END
GO

