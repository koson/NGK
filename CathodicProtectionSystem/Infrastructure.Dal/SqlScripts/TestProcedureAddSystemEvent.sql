USE ngkdb

DECLARE @RC int
DECLARE @SystemEventCode int
DECLARE @Message nvarchar(500)
DECLARE @Created datetime2(7)
DECLARE @Category tinyint
DECLARE @HasRead bit
DECLARE @Id int

-- TODO: Set parameter values here.
SET @SystemEventCode = 1;
SET @Message = N'Test message';
SET @Created = GETDATE();
SET @Category = 1;
SET @HasRead = 0;

EXECUTE @RC = [AddSystemEvent] 
   @SystemEventCode
  ,@Message
  ,@Created
  ,@Category
  ,@HasRead
  ,@Id OUTPUT
 
  
 PRINT(N'Return code = ' + CONVERT(varchar(10), @RC) + N' '
	 + N'ID = ' + CONVERT(varchar(10), @Id));
  
GO


