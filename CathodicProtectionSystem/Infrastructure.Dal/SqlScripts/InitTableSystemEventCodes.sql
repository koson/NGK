USE [ngkdb]
GO

INSERT INTO [ngkdb].[dbo].[SystemEventCodes]
           ([SystemEventCode]
           ,[EventName]
           ,[Description])
     VALUES
           (0, N'Undefined', N'��������� ������� �� ����������'),
           (1, N'SystemWasStarted', N'������ �������'),
           (2, N'SystemWasStopped', N'��������� �������')
GO


