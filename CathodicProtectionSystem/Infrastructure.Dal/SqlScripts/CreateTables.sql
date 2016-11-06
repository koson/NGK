USE [ngkdb]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Удаляем связи между таблицами

-- Удаляем таблицы
IF EXISTS(SELECT * FROM sys.objects 
			WHERE object_id = OBJECT_ID(N'[dbo].[SystemEnentsLog]') AND type in (N'U'))
	DROP TABLE [dbo].[SystemEnentsLog]
GO
IF EXISTS(SELECT * FROM sys.objects
			WHERE object_id = OBJECT_ID(N'[dbo].[SystemEventCodes]') AND type in (N'U'))
	DROP TABLE [dbo].[SystemEventCodes]


-- Создаём таблицы
CREATE TABLE [dbo].[SystemEnentsLog]
(
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[SystemEventCode] [int] NOT NULL, -- код события
	[Message] [nvarchar](500) NULL, -- сообщение при событии
	[Created] [datetime2](7) NULL, -- дата и время создания события
	[Category] [tinyint] NOT NULL, -- Категория вазности сообщения
	[HasRead] [bit], -- Сообщение прочитано
	CONSTRAINT [PK_SystemLog] PRIMARY KEY CLUSTERED ([MessageId] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SystemEventCodes](
	[SystemEventCode] [int] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
 CONSTRAINT [PK_SystemEventCodes] PRIMARY KEY CLUSTERED ([SystemEventCode] ASC)
 WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, 
 ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Создаём связи между таблицами
ALTER TABLE [dbo].[SystemEnentsLog]  WITH CHECK ADD  CONSTRAINT [FK_SystemEnentsLog_SystemEventCodes] FOREIGN KEY([SystemEventCode])
REFERENCES [dbo].[SystemEventCodes] ([SystemEventCode])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[SystemEnentsLog] CHECK CONSTRAINT [FK_SystemEnentsLog_SystemEventCodes]
GO