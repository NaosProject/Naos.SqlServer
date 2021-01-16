DROP PROCEDURE IF EXISTS [Example].[GetTagSetFromIds]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetTagSetFromIds](
  @TagIdsXml [xml],
  @TagsXml [NVARCHAR](MAX) OUTPUT
  )
AS
BEGIN      
    SELECT @TagsXml = (SELECT
	    TagKey AS [@Key],
	    ISNULL(TagValue,'---NULL---') AS [@Value]
    FROM [Example].[Tag]    
    WHERE [Id] IN (SELECT TagValue FROM [Example].[GetTagsTableVariableFromTagIdsXml](@TagIdsXml))
    FOR XML PATH ('Tag'), ROOT('Tags'))

END
GO

