USE [RedButtonBase]
GO

/****** Object:  StoredProcedure [dbo].[CreateItem]    Script Date: 02/28/2015 10:36:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CreateItem] 

@cacheid nvarchar(255) = NULL, 
@title text = NULL, 
@snippet text = NULL, 
@link text = NULL,
@buzzword text = NULL

AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.Findings(cacheid,title,snippet,link,buzzword) VALUES(@cacheid,@title,@snippet,@link,@buzzword)	
END







GO


