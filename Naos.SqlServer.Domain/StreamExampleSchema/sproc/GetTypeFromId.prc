DROP PROCEDURE IF EXISTS [Example].[GetTypeFromId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetTypeFromId](
    @Id [INT]
  , @Versioned [BIT]
  , @AssemblyQualifiedName [NVARCHAR](2000) OUTPUT
)
AS
BEGIN
    IF (@Versioned = 1)
    BEGIN
        SELECT 
	            @AssemblyQualifiedName = [AssemblyQualifiedName]
	        FROM [Example].[TypeWithVersion] WHERE [Id] = @Id
    END
    ELSE
    BEGIN
        SELECT 
	            @AssemblyQualifiedName = [AssemblyQualifiedName]
	        FROM [Example].[TypeWithoutVersion] WHERE [Id] = @Id
    END
END
GO

