SET XACT_ABORT ON
BEGIN TRANSACTION;

/* 7/4/15 */

/****** Object:  Table [dbo].[DraftPickHistory]    Script Date: 7/4/2015 2:24:50 PM ******/
DROP TABLE [dbo].[DraftPickHistory]
GO



ALTER TABLE [dbo].[Draft] WITH CHECK ADD  CONSTRAINT [FK_Draft_League] FOREIGN KEY([LeagueId])
REFERENCES [dbo].[League] ([LeagueId])
GO

ALTER TABLE [dbo].[Draft] CHECK CONSTRAINT [FK_Draft_League]
GO

ALTER TABLE [dbo].[Draft] WITH CHECK ADD  CONSTRAINT [FK_Draft_Winner] FOREIGN KEY([WinnerId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[Draft] CHECK CONSTRAINT [FK_Draft_Winner]
GO

ALTER TABLE [dbo].[Draft] WITH CHECK ADD  CONSTRAINT [FK_Draft_RunnerUp] FOREIGN KEY([RunnerUpId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[Draft] CHECK CONSTRAINT [FK_Draft_RunnerUp]
GO


ALTER TABLE [dbo].[DraftChat] WITH CHECK ADD  CONSTRAINT [FK_DraftChat_User] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[User] ([UserId])
GO


ALTER TABLE [dbo].[DraftOwner] WITH CHECK ADD  CONSTRAINT [FK_DraftOwner_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[DraftOwner] WITH CHECK ADD  CONSTRAINT [FK_DraftOwner_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO


ALTER TABLE [dbo].[DraftPick] WITH CHECK ADD  CONSTRAINT [FK_DraftPick_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO


ALTER TABLE [dbo].[DraftRank] WITH CHECK ADD  CONSTRAINT [FK_DraftRank_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[DraftRank] WITH CHECK ADD  CONSTRAINT [FK_DraftRank_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO


ALTER TABLE [dbo].[LeagueOwner] WITH CHECK ADD  CONSTRAINT [FK_LeagueOwner_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO


ALTER TABLE [dbo].[PlayerAdjustment] WITH CHECK ADD  CONSTRAINT [FK_PlayerAdjustment_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO



COMMIT TRANSACTION;













/* Below Run in Production on 6/29/15 */
BEGIN TRANSACTION;



ALTER TABLE dbo.[Message]
ADD [LeagueName] AS dbo.GetLeagueName(LeagueId)
GO



/* MIGHT HAVE TO DO THIS VIA DESIGN (FOR SOME REASON...) */

ALTER TABLE dbo.[Draft]
ALTER COLUMN [LeagueId] [int] NOT NULL
GO




/* 6/27/2015 */


USE [Home]
GO

/****** Object:  UserDefinedFunction [dbo].[GetUserNickName] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetUserNickName] (@UserId INT)
RETURNS VARCHAR(41) 
AS BEGIN
    DECLARE @NickName VARCHAR(41)

    SELECT @NickName = NickName FROM dbo.[User] WHERE UserId = @UserId

    RETURN @NickName
END

GO




/****** Object:  Table [dbo].[DraftChat]    Script Date: 6/27/2015 5:29:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DraftChat](
	[DraftChatId] [int] IDENTITY(1,1) NOT NULL,
	[DraftId] [int] NOT NULL,
	[LeagueId] [int] NOT NULL,
	[AuthorId] [int] NOT NULL,
	[NickName]  AS ([dbo].[GetUserNickName]([AuthorId])),
	[MessageText] [varchar](200) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_DraftChat] PRIMARY KEY CLUSTERED 
(
	[DraftChatId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[DraftChat]  WITH CHECK ADD  CONSTRAINT [FK_DraftChat_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[DraftChat] CHECK CONSTRAINT [FK_DraftChat_Draft]
GO

ALTER TABLE [dbo].[DraftChat]  WITH CHECK ADD  CONSTRAINT [FK_DraftChat_League] FOREIGN KEY([LeagueId])
REFERENCES [dbo].[League] ([LeagueId])
GO

ALTER TABLE [dbo].[DraftChat] CHECK CONSTRAINT [FK_DraftChat_League]
GO




 
CREATE FUNCTION dbo.GetUserFullName (@UserId INT)
RETURNS VARCHAR(41) 
AS BEGIN
    DECLARE @FullName VARCHAR(41)

    SELECT @FullName = FullName FROM dbo.[User] WHERE UserId = @UserId

    RETURN @FullName
END
GO





/****** Object:  Table [dbo].[Message] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Message](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[AuthorId] [int] NOT NULL,
	[AuthorName]  AS ([dbo].[GetUserFullName]([AuthorId])),
	[Title] [varchar](50) NULL,
	[MessageText] [varchar](1000) NOT NULL,
	[AllUsers] [bit] NOT NULL,
	[LeagueId] [int] NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Message] ADD  CONSTRAINT [DF_Message_AllUsers]  DEFAULT ((0)) FOR [AllUsers]
GO

ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_League] FOREIGN KEY([LeagueId])
REFERENCES [dbo].[League] ([LeagueId])
GO

ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_League]
GO

ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_User] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_User]
GO



ALTER TABLE dbo.[User]
ADD [LastMessageView] [datetime] NULL
GO




COMMIT TRANSACTION;




















/* Below Run in Production on 5/12/15 */
BEGIN TRANSACTION;

/* 4/4/2015 */

ALTER TABLE [dbo].[League]
ADD [AddTimestamp] datetime NULL
GO

UPDATE [dbo].[League]
SET [AddTimestamp] = [LastUpdateTimestamp]
WHERE [LastUpdateTimestamp] IS NOT NULL
GO

CREATE FUNCTION dbo.GetLeagueName (@LeagueId INT)
RETURNS VARCHAR(50) 
AS BEGIN
    DECLARE @LeagueName VARCHAR(50)

    SELECT @LeagueName = LeagueName FROM dbo.League WHERE LeagueId = @LeagueId

    RETURN @LeagueName
END
GO

ALTER TABLE dbo.Draft
ADD LeagueName AS dbo.GetLeagueName(LeagueId)
GO


/* 4/6/2015 */

ALTER TABLE dbo.[User]
ADD [FullName]  AS (([FirstName]+' ')+[LastName]) PERSISTED NOT NULL
GO

ALTER TABLE dbo.[LeagueOwner]
ADD [IsActive] [bit] NOT NULL DEFAULT(1)
GO


/* 4/8/15 */

UPDATE lo
SET lo.CssClass = u.UserName
FROM dbo.LeagueOwner lo
INNER JOIN dbo.[User] u ON lo.UserId = u.UserId


/* 4/12/15 */

ALTER TABLE dbo.[Draft]
ADD [AddTimestamp] [datetime] NULL
GO

UPDATE Draft
SET AddTimestamp = LastUpdateTimestamp
GO

ALTER TABLE dbo.[Draft]
ALTER COLUMN [AddTimestamp] [datetime] NOT NULL
GO

/* 4/19/15 */

ALTER TABLE dbo.[Draft]
ADD [WinnerId] [int] NULL,
	[RunnerUpId] [int] NULL,
	[HasCoWinners] [bit] NULL
GO



/* 4/25/15 */

ALTER TABLE dbo.[User]
ADD [IsActive] [bit] NOT NULL DEFAULT(1),
	[LastUpdateTimestamp] [datetime] NULL
GO

EXEC sp_RENAME '[User].AddDateTime', 'AddTimestamp', 'COLUMN'
GO

UPDATE dbo.[User]
SET LastUpdateTimestamp = AddTimestamp
GO

ALTER TABLE dbo.[User]
ALTER COLUMN [LastUpdateTimestamp] [datetime] NOT NULL
ALTER TABLE dbo.[User]
ALTER COLUMN [AddTimestamp] [datetime] NOT NULL
GO

ALTER TABLE dbo.[User]
ADD [NickName] varchar(10) NULL
GO

UPDATE u
SET u.NickName = o.NickName
FROM dbo.[User] u, dbo.[Owner] o
WHERE u.UserId = o.UserId
GO

DROP TABLE [dbo].[Owner]
GO

DROP TABLE [dbo].[DraftRound]
GO

DROP TABLE [dbo].[DraftOrder]
GO

DROP TABLE [dbo].[BadTeamsDraft]
GO

EXEC sp_RENAME '[DraftRank].OwnerId', 'UserId', 'COLUMN'
GO

ALTER TABLE dbo.[LeagueOwner]
DROP COLUMN [OwnerId]
GO

EXEC sp_RENAME '[DraftOwner].OwnerId', 'UserId', 'COLUMN'
GO

EXEC sp_RENAME '[DraftPick].OwnerId', 'UserId', 'COLUMN'
GO

EXEC sp_RENAME '[DraftPickHistory].OwnerId', 'UserId', 'COLUMN'
GO

ALTER TABLE [dbo].[LeagueOwner]
ADD LeagueName AS dbo.GetLeagueName(LeagueId)
GO


/* 4/25/15 */

/****** Object:  Table [dbo].[CssColor]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CssColor](
	[ClassName] [varchar](20) NOT NULL,
	[ColorText] varchar(20) NOT NULL,
	[ColorValue] [varchar](7) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_ClassName] PRIMARY KEY CLUSTERED 
(
	[ClassName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [dbo].[CssColor]
           ([ClassName],[ColorText],[ColorValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES	 
           ('_none','None (Black)','#000000',getdate(),getdate()),
           ('aqua','Aqua','#19E1CC',getdate(),getdate()),
           ('blue','Blue','#638BE7',getdate(),getdate()),
           ('brown','Brown','#C58A4F',getdate(),getdate()),
           ('dark-gray','Dark Gray','#5D5E65',getdate(),getdate()),
           ('gold','Gold','#FDBA31',getdate(),getdate()),
           ('gray','Gray','#B2B2B2',getdate(),getdate()),
           ('green','Green','#50A57F',getdate(),getdate()),
           ('light-blue','Light Blue','#87CEEB',getdate(),getdate()),
           ('light-green','Light Green','#00FF00',getdate(),getdate()),
           ('maroon','Maroon','#B00000',getdate(),getdate()),
           ('orange','Orange','#FF612B',getdate(),getdate()),
           ('peach','Peach','#FFCC99',getdate(),getdate()),
           ('pink','Pink','#FF69B4',getdate(),getdate()),
           ('purple','Purple','#551A8B',getdate(),getdate()),
           ('red','Red','#FF0000',getdate(),getdate()),
           ('white', 'White','#FFFFFF',getdate(),getdate()),
           ('yellow','Yellow','#FFE100',getdate(),getdate())
GO

UPDATE LeagueOwner SET CssClass = 'light-green' WHERE CssClass = 'brian'
UPDATE LeagueOwner SET CssClass = 'orange' WHERE CssClass = 'dave'
UPDATE LeagueOwner SET CssClass = 'aqua' WHERE CssClass = 'hawkeman'
UPDATE LeagueOwner SET CssClass = 'pink' WHERE CssClass = 'jen'
UPDATE LeagueOwner SET CssClass = 'yellow' WHERE CssClass = 'jeremiah'
UPDATE LeagueOwner SET CssClass = 'gray' WHERE CssClass = 'joey'
UPDATE LeagueOwner SET CssClass = 'blue' WHERE CssClass = 'meat'
UPDATE LeagueOwner SET CssClass = 'red' WHERE CssClass = 'pohlmann'
UPDATE LeagueOwner SET CssClass = 'brown' WHERE CssClass = 'shannon'
UPDATE LeagueOwner SET CssClass = 'green' WHERE CssClass = 'steve'
UPDATE LeagueOwner SET CssClass = 'purple' WHERE CssClass = 'heckler'
UPDATE LeagueOwner SET CssClass = 'maroon' WHERE CssClass = 'holda'
UPDATE LeagueOwner SET CssClass = 'yellow' WHERE CssClass = 'trevor'
UPDATE LeagueOwner SET CssClass = 'brown' WHERE CssClass = 'mitchell'


ALTER TABLE [dbo].[LeagueOwner]  WITH CHECK ADD  CONSTRAINT [FK_LeagueOwner_League] FOREIGN KEY([LeagueId])
REFERENCES [dbo].[League] ([LeagueId])
GO

ALTER TABLE [dbo].[LeagueOwner] CHECK CONSTRAINT [FK_LeagueOwner_League]
GO


ALTER TABLE [dbo].[LeagueOwner]  WITH CHECK ADD  CONSTRAINT [FK_LeagueOwner_CssColor] FOREIGN KEY([CssClass])
REFERENCES [dbo].[CssColor] ([ClassName])
GO

ALTER TABLE [dbo].[LeagueOwner] CHECK CONSTRAINT [FK_LeagueOwner_CssColor]
GO





COMMIT TRANSACTION;