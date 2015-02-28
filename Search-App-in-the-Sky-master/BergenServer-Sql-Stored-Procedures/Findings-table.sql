USE [RedButtonBase]
GO

/****** Object:  Table [dbo].[Findings]    Script Date: 02/28/2015 10:40:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Findings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[guid] [uniqueidentifier] NOT NULL,
	[cacheid] [nvarchar](255) NULL,
	[title] [text] NULL,
	[snippet] [text] NULL,
	[link] [text] NULL,
	[buzzword] [text] NULL,
	[timestamp] [datetime] NULL,
 CONSTRAINT [PK_Findings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Findings] ADD  CONSTRAINT [DF_Findings_guid]  DEFAULT (newid()) FOR [guid]
GO

ALTER TABLE [dbo].[Findings] ADD  CONSTRAINT [DF_Findings_timestamp]  DEFAULT (getdate()) FOR [timestamp]
GO


