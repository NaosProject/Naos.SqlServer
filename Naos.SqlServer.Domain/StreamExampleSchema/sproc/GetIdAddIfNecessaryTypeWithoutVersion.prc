DROP PROCEDURE IF EXISTS [Example].[GetIdAddIfNecessaryTypeWithoutVersion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetIdAddIfNecessaryTypeWithoutVersion](
  @AssemblyQualifiedNameWithoutVersion [NVARCHAR](2000),
  @Id [INT] OUTPUT
  )
AS
BEGIN

    SELECT
        @Id = [Id]
    FROM [Example].[TypeWithoutVersion]
        WHERE [AssemblyQualifiedName] = @AssemblyQualifiedNameWithoutVersion

    IF (@Id IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [GetIdAddIfNecTypeWithoutVerTrans]
        BEGIN TRY
            SELECT
                @Id = [Id]
            FROM [Example].[TypeWithoutVersion]
                WHERE [AssemblyQualifiedName] = @AssemblyQualifiedNameWithoutVersion

            IF (@Id IS NULL)
            BEGIN
                INSERT INTO [Example].[TypeWithoutVersion]
                (
                     [AssemblyQualifiedName]
                   , [RecordCreatedUtc]
                )
                VALUES
                (
                      @AssemblyQualifiedNameWithoutVersion
                    , GETUTCDATE()
                )

                SET @Id = SCOPE_IDENTITY()
            END

            COMMIT TRANSACTION [GetIdAddIfNecTypeWithoutVerTrans]
        END TRY
        BEGIN CATCH
            SET @Id = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [GetIdAddIfNecTypeWithoutVerTrans]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END
END

GO

