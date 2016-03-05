SET XACT_ABORT ON
BEGIN TRANSACTION;

/* 3/3/15 */

USE [Home]
GO






/****** Object:  Table [dbo].[Highlight]    Script Date: 3/3/2016 10:58:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Highlight](
	[HighlightId] [int] IDENTITY(1,1) NOT NULL,
	[HighlightClass] [varchar](50) NOT NULL,
	[HighlightValue] [varchar](50) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Highlight] PRIMARY KEY CLUSTERED 
(
	[HighlightId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO






USE [Home]
GO

/****** Object:  UserDefinedFunction [dbo].[GetHighlightValue]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetHighlightValue] (@HighlightId INT)
RETURNS VARCHAR(51) 
AS BEGIN
    DECLARE @HighlightValue VARCHAR(25)

    SELECT @HighlightValue = HighlightValue FROM [dbo].[Highlight] WHERE HighlightId = @HighlightId

    RETURN @HighlightValue
END

GO






USE [Home]
GO

/****** Object:  Table [dbo].[PlayerHighlight]    Script Date: 3/3/2016 11:21:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PlayerHighlight](
	[PlayerHighlightId] [int] IDENTITY(1,1) NOT NULL,
	[DraftId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[HighlightId] [int] NOT NULL,
	[HighlightValue]  AS ([dbo].[GetHighlightValue]([HighlightId])),
	[RankNum] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_PlayerHighlight] PRIMARY KEY CLUSTERED 
(
	[PlayerHighlightId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PlayerHighlight]  WITH CHECK ADD  CONSTRAINT [FK_PlayerHighlight_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[PlayerHighlight] CHECK CONSTRAINT [FK_PlayerHighlight_Draft]
GO

ALTER TABLE [dbo].[PlayerHighlight]  WITH CHECK ADD  CONSTRAINT [FK_PlayerHighlight_Highlight] FOREIGN KEY([HighlightId])
REFERENCES [dbo].[Highlight] ([HighlightId])
GO

ALTER TABLE [dbo].[PlayerHighlight] CHECK CONSTRAINT [FK_PlayerHighlight_Highlight]
GO

ALTER TABLE [dbo].[PlayerHighlight]  WITH CHECK ADD  CONSTRAINT [FK_PlayerHighlight_Player] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Player] ([PlayerId])
GO

ALTER TABLE [dbo].[PlayerHighlight] CHECK CONSTRAINT [FK_PlayerHighlight_Player]
GO

ALTER TABLE [dbo].[PlayerHighlight]  WITH CHECK ADD  CONSTRAINT [FK_PlayerHighlight_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[PlayerHighlight] CHECK CONSTRAINT [FK_PlayerHighlight_User]
GO











--Why is RankNum on PlayerRank table Nullable?  Mistake?


COMMIT TRANSACTION;



/* ... Below Run in Production on ... */