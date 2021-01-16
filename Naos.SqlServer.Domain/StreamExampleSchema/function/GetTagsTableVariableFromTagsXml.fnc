DROP FUNCTION IF EXISTS [Example].[GetTagsTableVariableFromTagsXml]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [Example].[GetTagsTableVariableFromTagsXml] (
      @TagsXml [xml]
)
RETURNS TABLE
AS
RETURN 
		      SELECT
		        C.value('(@Key)[1]', '[NVARCHAR](450)') as [TagKey]
		      , [TagValue] = CASE C.value('(@Value)[1]', '[NVARCHAR](4000)')
			     WHEN '---NULL---' THEN NULL
				 ELSE C.value('(@Value)[1]', '[NVARCHAR](4000)')
				 END
		      FROM
			    @TagsXml.nodes('/Tags/Tag') AS T(C)

GO

