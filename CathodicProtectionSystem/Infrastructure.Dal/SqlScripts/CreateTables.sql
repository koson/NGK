USE [ngkdb]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ������� ����� ����� ���������

-- ������� �������
IF EXISTS(SELECT * FROM sys.objects 
			WHERE object_id = OBJECT_ID(N'[dbo].[SystemEnentsLog]') AND type in (N'U'))
	DROP TABLE [dbo].[SystemEnentsLog]
GO
IF EXISTS(SELECT * FROM sys.objects
			WHERE object_id = OBJECT_ID(N'[dbo].[SystemEventCodes]') AND type in (N'U'))
	DROP TABLE [dbo].[SystemEventCodes]


-- ������ �������
CREATE TABLE [dbo].[SystemEnentsLog]
(
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[SystemEventCode] [int] NOT NULL, -- ��� �������
	[Message] [nvarchar](500) NULL, -- ��������� ��� �������
	[Created] [datetime2](7) NULL, -- ���� � ����� �������� �������
	[Category] [tinyint] NOT NULL, -- ��������� �������� ���������
	[HasRead] [bit], -- ��������� ���������
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

-- ������ ����� ����� ���������
ALTER TABLE [dbo].[SystemEnentsLog]  WITH CHECK ADD  CONSTRAINT [FK_SystemEnentsLog_SystemEventCodes] FOREIGN KEY([SystemEventCode])
REFERENCES [dbo].[SystemEventCodes] ([SystemEventCode])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[SystemEnentsLog] CHECK CONSTRAINT [FK_SystemEnentsLog_SystemEventCodes]
GO