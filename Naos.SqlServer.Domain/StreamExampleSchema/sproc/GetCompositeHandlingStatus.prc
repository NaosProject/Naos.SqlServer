DROP PROCEDURE IF EXISTS [Example].[GetCompositeHandlingStatus]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetCompositeHandlingStatus](
  @Concern AS [VARCHAR](50)
, @TagIdsXml AS [XML]
, @Status AS [VARCHAR](50) OUTPUT
)
AS
BEGIN
    SELECT TOP 1
        @Status = h1.[Status]
    FROM [Example].[HandlingTag] ht
    INNER JOIN [Example].[Handling] h1
        ON h1.[Id] = ht.[HandlingId]
    LEFT OUTER JOIN [Example].[Handling] h2
        ON h1.[RecordId] = h2.[RecordId] AND h1.[Id] < h2.[Id]
    LEFT JOIN [Example].[GetStatusSortOrderTableVariable]() s
        ON s.[Status] = h1.[Status]
    WHERE h2.[Id] IS NULL AND h1.[Concern] = @Concern
        AND ht.[TagId] IN (SELECT [TagValue] FROM [Example].[GetTagsTableVariableFromTagIdsXml](@TagIdsXml))
    ORDER BY s.[SortOrder] DESC

    IF (@Status IS NULL)
    BEGIN
        SET @Status = 'None'
    END
END
GO

