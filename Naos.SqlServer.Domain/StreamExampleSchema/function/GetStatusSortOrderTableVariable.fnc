DROP FUNCTION IF EXISTS [Example].[GetStatusSortOrderTableVariable]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [Example].[GetStatusSortOrderTableVariable]()
    RETURNS TABLE
    AS
    RETURN 
    SELECT [SortOrder], [Status] FROM 
(VALUES
	  (0, 'Completed')
	, (1, 'None')
	, (2, 'RetryFailed')
	, (3, 'Requested')
	, (4, 'Unknown')
	, (5, 'SelfCanceledRunning')
	, (6, 'CanceledRunning')
	, (7, 'Canceled')
	, (8, 'Running')
	, (9, 'Failed')
	, (10, 'Blocked')
) x([SortOrder], [Status])

GO

