use ngkdb

INSERT INTO [ngkdb].[dbo].[SystemEventsLog]
           ([SystemEventCode]
           ,[Message]
           ,[Created]
           ,[Category]
           ,[HasRead])
     VALUES
           (1, N'������� ��������', '2016-02-10 21:02:09', 0, 0),
           (2, N'������� �����������', '2016-02-10 22:02:09', 0, 0)
GO


