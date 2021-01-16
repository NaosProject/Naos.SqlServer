DROP PROCEDURE IF EXISTS [Example].[GetSerializerRepresentationFromId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Example].[GetSerializerRepresentationFromId](
    @Id [INT]
  , @SerializationKind [VARCHAR](50) OUTPUT
  , @ConfigTypeWithVersionId [INT] OUTPUT
  , @CompressionKind [VARCHAR](50) OUTPUT
  , @SerializationFormat [VARCHAR](50) OUTPUT
)
AS
BEGIN
SELECT 
	    @SerializationKind = [SerializationKind]
	  , @ConfigTypeWithVersionId = [SerializationConfigurationTypeWithVersionId]
	  , @CompressionKind = [CompressionKind]
	  , @SerializationFormat = [SerializationFormat]
	FROM [Example].[SerializerRepresentation] WHERE [Id] = @Id

END
GO

