USE [ngkdb]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Добавляет запись о произошедшем системном событии в таблицу SystemEventsLog
-- =============================================
CREATE PROCEDURE AddSystemEvent 
	-- IN
	@SystemEventCode int, 
	@Message nvarchar(500),
	@Created datetime2(7),
	@Category tinyint,
	@HasRead bit = FALSE,
	-- OUT
	@Id int OUTPUT 
	
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[SystemEventsLog] ([SystemEventCode], [Message], [Created], [Category], [HasRead])
	VALUES
		(@SystemEventCode, @Message, @Created, @Category, @HasRead);
		
	SELECT @Id = SCOPE_IDENTITY()
		
END
GO
