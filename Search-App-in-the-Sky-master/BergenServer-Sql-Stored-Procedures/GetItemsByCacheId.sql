USE [RedButtonBase]
GO

/****** Object:  StoredProcedure [dbo].[GetItemByCacheId]    Script Date: 02/28/2015 10:38:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetItemByCacheId] @CacheId nvarchar(255) = NULL	
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM Findings WHERE cacheid = @CacheId
END





GO


