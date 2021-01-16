DROP PROCEDURE IF EXISTS [Example].[GetIdsAddIfNecessaryTagSet]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetIdsAddIfNecessaryTagSet](
  @TagsXml [xml],
  @TagIdsXml [NVARCHAR](MAX) OUTPUT
  )
AS
BEGIN
    
            DECLARE @TagIdsTable TABLE(
			[Id] [BIGINT] NULL,
			[TagKey] [NVARCHAR](450) NULL,
			[TagValue] [NVARCHAR](4000) NULL)


    INSERT INTO @TagIdsTable
        SELECT
            e.[Id]
		  , n.[TagKey]
		  , n.[TagValue]
		FROM [Example].[GetTagsTableVariableFromTagsXml](@TagsXml) n
        LEFT JOIN [Example].[Tag] e ON 
            (n.[TagKey] =  e.[TagKey] AND (n.[TagValue] = e.[TagValue] OR (n.[TagValue] is null and e.[TagValue] is null)))
    IF EXISTS (SELECT TOP 1 * FROM @TagIdsTable t WHERE t.[Id] IS NULL)
    BEGIN
        BEGIN TRANSACTION [GetIdAddIfNecTagSetTrans]
          BEGIN TRY
	        INSERT INTO [Example].[Tag] WITH (TABLOCKX)
            SELECT 
		        n.[TagKey]
		      , n.[TagValue]
		      , GETUTCDATE()
		    FROM @TagIdsTable n
            LEFT JOIN [Example].[Tag] e ON 
            (n.[TagKey] =  e.[TagKey] AND (n.[TagValue] = e.[TagValue] OR (n.[TagValue] is null and e.[TagValue] is null)))
            WHERE e.[Id] IS NULL

            COMMIT TRANSACTION [GetIdAddIfNecTagSetTrans]
        END TRY
        BEGIN CATCH
            SET @TagIdsXml = NULL
            DECLARE @ErrorMessage nvarchar(max), 
                  @ErrorSeverity int, 
                  @ErrorState int

            SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()

            IF (@@trancount > 0)
            BEGIN
                ROLLBACK TRANSACTION [GetIdAddIfNecTagSetTrans]
            END

            RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        END CATCH
    END  

    IF EXISTS (SELECT TOP 1 * FROM @TagIdsTable WHERE [Id] IS NULL)
    BEGIN
        SELECT @TagIdsXml = (SELECT
	        ROW_NUMBER() OVER (ORDER BY n.[TagKey], ISNULL(n.[TagValue], '---NULL---')) AS [@Key],
	        n.Id AS [@Value]
        FROM @TagIdsTable e
        INNER JOIN [Example].[Tag] n ON
                (n.[TagKey] =  e.[TagKey] AND (n.[TagValue] = e.[TagValue] OR (n.[TagValue] is null and e.[TagValue] is null)))
        FOR XML PATH ('Tag'), ROOT('Tags'))
    END
    ELSE
    BEGIN
        SELECT @TagIdsXml = (SELECT
            ROW_NUMBER() OVER (ORDER BY e.[TagKey], ISNULL(e.[TagValue], '---NULL---')) AS [@Key],
	        e.Id AS [@Value]
        FROM @TagIdsTable e
        FOR XML PATH ('Tag'), ROOT('Tags'))
    END
END
GO

