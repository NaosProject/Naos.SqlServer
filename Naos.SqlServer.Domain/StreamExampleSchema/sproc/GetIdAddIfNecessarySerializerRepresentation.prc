DROP PROCEDURE IF EXISTS [Example].[GetIdAddIfNecessarySerializerRepresentation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetIdAddIfNecessarySerializerRepresentation](
  @ConfigTypeWithoutVersionId AS [INT]
, @ConfigTypeWithVersionId AS [INT]
, @SerializationKind [VARCHAR](50)
, @SerializationFormat AS [VARCHAR](50)
, @CompressionKind AS [VARCHAR](50)
, @UnregisteredTypeEncounteredStrategy AS [VARCHAR](50)
, @Id AS [INT] OUTPUT
)
AS
BEGIN
    SELECT
        @Id = [Id]
    FROM [Example].[SerializerRepresentation]
        WHERE [SerializationConfigurationTypeWithVersionId] = @ConfigTypeWithVersionId
          AND [SerializationConfigurationTypeWithoutVersionId] = @ConfigTypeWithoutVersionId
          AND [SerializationKind] = @SerializationKind
          AND [SerializationFormat] = @SerializationFormat
          AND [CompressionKind] = @CompressionKind
          AND [UnregisteredTypeEncounteredStrategy] = @UnregisteredTypeEncounteredStrategy

    IF (@Id IS NULL)
    BEGIN
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
        BEGIN TRANSACTION [GetIdAddIfSerializerRepTran]
        BEGIN TRY
            SELECT
                @Id = [Id]
            FROM [Example].[SerializerRepresentation]
                WHERE [SerializationConfigurationTypeWithVersionId] = @ConfigTypeWithVersionId
                  AND [SerializationConfigurationTypeWithoutVersionId] = @ConfigTypeWithoutVersionId
                  AND [SerializationKind] = @SerializationKind
                  AND [SerializationFormat] = @SerializationFormat
                  AND [CompressionKind] = @CompressionKind
                  AND [UnregisteredTypeEncounteredStrategy] = @UnregisteredTypeEncounteredStrategy

              IF (@Id IS NULL)
              BEGIN
                  INSERT INTO [Example].[SerializerRepresentation] (
                    [SerializationKind]
                  , [SerializationFormat]
                  , [SerializationConfigurationTypeWithoutVersionId]
                  , [SerializationConfigurationTypeWithVersionId]
                  , [CompressionKind]
                  , [UnregisteredTypeEncounteredStrategy]
                  , [RecordCreatedUtc]
                  ) VALUES (
                    @SerializationKind
                  , @SerializationFormat
                  , @ConfigTypeWithoutVersionId
                  , @ConfigTypeWithVersionId
                  , @CompressionKind
                  , @UnregisteredTypeEncounteredStrategy
                  , GETUTCDATE()
                  )

                  SET @Id = SCOPE_IDENTITY()
              END
            COMMIT TRANSACTION [GetIdAddIfSerializerRepTran]
        END TRY
        BEGIN CATCH
            SET @Id = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [GetIdAddIfSerializerRepTran]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END
END

GO

