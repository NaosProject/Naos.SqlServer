DROP PROCEDURE IF EXISTS [Example].[GetIdAddIfNecessaryTypeWithVersion]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetIdAddIfNecessaryTypeWithVersion](
  @AssemblyQualifiedNameWithVersion [NVARCHAR](2000),
  @Id [INT] OUTPUT
  )
AS
BEGIN

    SELECT
        @Id = [Id]
    FROM [Example].[TypeWithVersion]
        WHERE [AssemblyQualifiedName] = @AssemblyQualifiedNameWithVersion

    IF (@Id IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [GetIdAddIfNecTypeWithVerTrans]
        BEGIN TRY
            SELECT
                @Id = [Id]
            FROM [Example].[TypeWithVersion]
                WHERE [AssemblyQualifiedName] = @AssemblyQualifiedNameWithVersion

            IF (@Id IS NULL)
            BEGIN
                INSERT INTO [Example].[TypeWithVersion]
                (
                     [AssemblyQualifiedName]
                   , [RecordCreatedUtc]
                )
                VALUES
                (
                      @AssemblyQualifiedNameWithVersion
                    , GETUTCDATE()
                )

                SET @Id = SCOPE_IDENTITY()
            END

            COMMIT TRANSACTION [GetIdAddIfNecTypeWithVerTrans]
        END TRY
        BEGIN CATCH
            SET @Id = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [GetIdAddIfNecTypeWithVerTrans]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END
END

GO

