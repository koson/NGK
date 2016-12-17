USE [ngkdb]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	¬озвращает количество записей в таблице SystemEventsLog
-- =============================================
CREATE PROCEDURE GetSystemEventsCount
	@Count int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT @Count = COUNT(*) FROM [ngkdb].[dbo].[SystemEventsLog]
END
GO
