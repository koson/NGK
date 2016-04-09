USE [ngkdb]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ������� �������
IF  EXISTS (SELECT * FROM sys.objects 
			WHERE object_id = OBJECT_ID(N'[dbo].[SystemLog]') AND type in (N'U'))
	DROP TABLE [dbo].[SystemLog]
GO

-- ������ �������
CREATE TABLE [dbo].[SystemLog]
(
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](500) NULL,
	[Created] [datetime2](7) NULL,
	[Category] [tinyint] NOT NULL, -- ��������� �������� ���������
	[HasRead] [bit], -- ��������� ���������
	CONSTRAINT [PK_SystemLog] PRIMARY KEY CLUSTERED ([MessageId] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO