USE [ngkdb]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetSystemEventsPage
	-- IN
	@PageNumber int, -- номер страницы (начиная от 0...)
	@PageSize tinyint -- количество записей на странице (не менее 1)
	-- OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @records int
	DECLARE @pages int
	DECLARE @Start INT
	DECLARE @End INT

	IF @PageSize < 1
		RETURN(1) -- Недопустимый размер страницы
	IF @PageNumber < 0
		RETURN(2) -- Недопустимый номер страницы
	
	SELECT @records = COUNT(*) FROM [ngkdb].[dbo].[SystemEventsLog]
	
	IF @records <= @PageSize
		BEGIN
			SET @pages = 1
			--PRINT(N'page 1');
		END
	ELSE
		BEGIN
			SET @pages = @records / @PageSize + 1
			--PRINT(N'page more than 1');
		END

	SET @Start = @PageNumber * @PageSize + 1;
	SET @End = (@PageNumber * @PageSize) + @PageSize;
		
	--PRINT(N'Start number = ' + CAST(@Start AS NVARCHAR));	
	--PRINT(N'End number = ' + CAST(@End AS NVARCHAR));
		
	IF  @pages < @PageNumber
		RETURN(3) -- Страница с данным номером не существует

	;WITH NumberedTable AS 
	(
		SELECT *, ROW_NUMBER() OVER (ORDER BY [MessageId]) AS RowNumber
		FROM [ngkdb].[dbo].[SystemEventsLog]
	)
		
	SELECT t.[MessageId], t.[SystemEventCode], t.[Message], t.[Created], t.[Category], t.[HasRead] 
	FROM NumberedTable AS t
	WHERE t.RowNumber BETWEEN @Start AND @End
	
	RETURN(0)
END
GO
