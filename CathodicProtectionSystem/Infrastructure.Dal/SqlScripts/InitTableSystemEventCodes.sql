USE [ngkdb]
GO

INSERT INTO [ngkdb].[dbo].[SystemEventCodes]
           ([SystemEventCode]
           ,[EventName]
           ,[Description])
     VALUES
           (0, N'Undefined', N'Системное событие не определено'),
           (1, N'SystemWasStarted', N'Запуск системы'),
           (2, N'SystemWasStopped', N'Остановка системы')
GO


