SET XACT_ABORT ON
BEGIN TRANSACTION;

/* 7/24/15 */

/****** Object:  Table [dbo].[ByeWeek]    Script Date: 7/24/2015 11:40:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ByeWeek](
	[ByeWeekId] [int] IDENTITY(1,1) NOT NULL,
	[Year] [smallint] NOT NULL,
	[NFLTeam] [varchar](3) NOT NULL,
	[WeekNum] [smallint] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_ByeWeek] PRIMARY KEY CLUSTERED 
(
	[ByeWeekId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ByeWeek]  WITH CHECK ADD  CONSTRAINT [FK_ByeWeek_NFLTeam] FOREIGN KEY([NFLTeam])
REFERENCES [dbo].[NFLTeam] ([TeamAbbr])
GO

ALTER TABLE [dbo].[ByeWeek] CHECK CONSTRAINT [FK_ByeWeek_NFLTeam]
GO


INSERT INTO dbo.ByeWeek VALUES (2015, 'ARI', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'ATL', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'BAL', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'BUF', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'CAR', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'CHI', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'CIN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'CLE', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'DAL', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'DEN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'DET', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'GB', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'HOU', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'IND', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'JAX', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'KC', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'MIA', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'MIN', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'NE', '4', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'NO', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'NYG', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'NYJ', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'OAK', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'PHI', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'PIT', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'SD', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'SF', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'SEA', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'STL', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'TB', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'TEN', '4', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2015, 'WAS', '8', getdate(), getdate());




COMMIT TRANSACTION;












SET XACT_ABORT ON
BEGIN TRANSACTION;

/* Below Run in Production on 7/23/15 */
/* 7/23/15 */

	-- Mark all IsDrafted players
UPDATE dbo.Player
SET IsDrafted = 1
WHERE PlayerId IN (SELECT PlayerId FROM DraftPick WHERE PlayerId IS NOT NULL)

-- UPDATE Dexter McCluster !!!
-- Set RB to Active, WR to Inactive, have share same TruePlayerId
UPDATE dbo.Player
SET IsActive = 1
WHERE PlayerId = 334

UPDATE dbo.Player
SET TruePlayerId = 334
WHERE PlayerId = 229



/* 7/15/15 */
SET XACT_ABORT ON
BEGIN TRANSACTION;


/*
	TODO BEFORE RUNNING usp_LoadPlayerRanks_V2!
	- Add IsDrafted column to Player			+
	- Expand PlayerAdjustment Action column to 50 char	+
	- Delete existing rankings (FntprsDyn?)		
	- Mark all IsDrafted players
	- Map set of boundary test players
*/

ALTER TABLE dbo.[Player]
ADD [IsDrafted] [bit] NOT NULL DEFAULT(0)
GO


ALTER TABLE dbo.[PlayerAdjustment]
ALTER COLUMN [Action] VARCHAR(50) NULL
GO

/* 7/11/15 */

ALTER TABLE dbo.[Player]
ADD [TruePlayerId] int NULL
GO

UPDATE dbo.[Player]
SET [TruePlayerId] = [PlayerId]
WHERE [TruePlayerId] IS NULL


/****** Object:  UserDefinedFunction [dbo].[GetPlayerName]    Script Date: 7/12/2015 5:12:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetPlayerName] (@PlayerId INT)
RETURNS VARCHAR(51) 
AS BEGIN
    DECLARE @PlayerName VARCHAR(41)

    SELECT @PlayerName = PlayerName FROM dbo.[Player] WHERE PlayerId = @PlayerId

    RETURN @PlayerName
END

GO


ALTER TABLE dbo.[PlayerAdjustment]
ADD [TruePlayerId] int NULL
GO



--Mark all players in DB inactive (except Team Defenses)
UPDATE Player
SET IsActive = 0
WHERE IsActive = 1
	AND NOT (Position = 'DEF'
			AND NFLTeam <> 'FA'
			AND AddTimestamp < '2014-12-31');



UPDATE Player SET FirstName = 'Le''Veon' WHERE FirstName = 'LeVeon'
UPDATE Player SET FirstName = 'Da''Rick' WHERE FirstName = 'DaRick'
UPDATE Player SET FirstName = 'Ka''Deem' WHERE FirstName = 'KaDeem'




/* 7/4/15 */

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