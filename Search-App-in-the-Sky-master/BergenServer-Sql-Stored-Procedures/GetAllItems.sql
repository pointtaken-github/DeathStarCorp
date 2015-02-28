USE [RedButtonBase]
GO

/****** Object:  StoredProcedure [dbo].[GetAllItems]    Script Date: 02/28/2015 10:38:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetAllItems]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM Findings
END






GO


