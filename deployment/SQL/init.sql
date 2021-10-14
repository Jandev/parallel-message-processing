SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logging](
	[Id] [uniqueidentifier] NOT NULL,
	[Processor] [nvarchar](100) NOT NULL,
	[MessageId] [int] NOT NULL,
	[Created] [datetime] NOT NULL
) ON [PRIMARY]
GO