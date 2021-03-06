﻿/* Below run in Production on 8/13/16 */

/* 8/13/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;


ALTER TABLE [dbo].[User]
ADD [LoginDomain] varchar(30);
GO

ALTER TABLE [dbo].[User]
ADD [LoginUserAgent] varchar(512);
GO


UPDATE [dbo].[Highlight]
SET HighlightValue = '#CCAB42'
WHERE HighlightName = 'Gold'






COMMIT TRANSACTION;






/* Below run in Production on 8/11/16 */

/* 8/10/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



CREATE NONCLUSTERED INDEX [IX_TruePlayerId] ON [dbo].[Player]
(
	[TruePlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO






/****** Object:  Table [dbo].[ErrorLog]    Script Date: 8/10/2016 11:16:39 PM ******/
DROP TABLE [dbo].[ErrorLog]
GO

/****** Object:  Table [dbo].[ErrorLog]    Script Date: 8/10/2016 11:16:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ErrorLog](
	[ErrorId] [int] IDENTITY(1,1) NOT NULL,
	[ErrorType] [varchar](20) NULL,
	[MessageText] [varchar](1000) NOT NULL,
	[StackTrace] [varchar](3000) NULL,
	[Request] [varchar](500) NULL,
	[UserName] [varchar](20) NULL,
	[DraftId] [int] NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO










COMMIT TRANSACTION;





/* Below run in Production on 8/2/16 */

/* 7/29/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



UPDATE [dbo].[CssColor]
SET ColorValue = '#4477FB'
WHERE ClassName = 'blue'

UPDATE [dbo].[CssColor]
SET ColorValue = 'purple'
WHERE ClassName = 'purple'

UPDATE [dbo].[CssColor]
SET ColorValue = '#B36E39'
WHERE ClassName = 'brown'

UPDATE [dbo].[CssColor]
SET ColorValue = '#CCAB42'
WHERE ClassName = 'gold'




ALTER TABLE [dbo].[Highlight]
ADD [HighlightOrder] smallint NULL
GO


INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Dark Grey','bg-dark-grey','darkgrey',NULL,getdate(),getdate())
GO


INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Purple','bg-purple','purple',NULL,getdate(),getdate())
GO


INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Maroon','bg-maroon','maroon',NULL,getdate(),getdate())
GO


INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Peach','bg-peach','#FFDAB9',NULL,getdate(),getdate())
GO


INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Gold','bg-gold','#E7C531',NULL,getdate(),getdate())
GO



--DELETE FROM [dbo].[Highlight] WHERE HighlightName = 'White'
/* 8/2/16 Changed my mind again! */
/*
INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[HighlightOrder],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('White','bg-white','white',NULL,getdate(),getdate())
GO

*/


UPDATE [dbo].[Highlight] SET HighlightOrder = 1 WHERE HighlightName = 'Yellow'
UPDATE [dbo].[Highlight] SET HighlightOrder = 2 WHERE HighlightName = 'Lime'
UPDATE [dbo].[Highlight] SET HighlightOrder = 3 WHERE HighlightName = 'Cyan'
UPDATE [dbo].[Highlight] SET HighlightOrder = 4 WHERE HighlightName = 'Orange'
UPDATE [dbo].[Highlight] SET HighlightOrder = 5 WHERE HighlightName = 'Pink'
UPDATE [dbo].[Highlight] SET HighlightOrder = 6 WHERE HighlightName = 'Red'
UPDATE [dbo].[Highlight] SET HighlightOrder = 7 WHERE HighlightName = 'Magenta'
UPDATE [dbo].[Highlight] SET HighlightOrder = 8 WHERE HighlightName = 'Light Blue'
UPDATE [dbo].[Highlight] SET HighlightOrder = 9 WHERE HighlightName = 'Blue'
UPDATE [dbo].[Highlight] SET HighlightOrder = 10 WHERE HighlightName = 'Green'
UPDATE [dbo].[Highlight] SET HighlightOrder = 11 WHERE HighlightName = 'Brown'
UPDATE [dbo].[Highlight] SET HighlightOrder = 12 WHERE HighlightName = 'Dark Grey'
UPDATE [dbo].[Highlight] SET HighlightOrder = 13 WHERE HighlightName = 'Gold'
UPDATE [dbo].[Highlight] SET HighlightOrder = 14 WHERE HighlightName = 'Peach'
UPDATE [dbo].[Highlight] SET HighlightOrder = 15 WHERE HighlightName = 'Purple'
UPDATE [dbo].[Highlight] SET HighlightOrder = 16 WHERE HighlightName = 'Maroon'
UPDATE [dbo].[Highlight] SET HighlightOrder = 17 WHERE HighlightName = 'Grey (clear)'
UPDATE [dbo].[Highlight] SET HighlightOrder = 18 WHERE HighlightName = 'White'
UPDATE [dbo].[Highlight] SET HighlightOrder = 19 WHERE HighlightName = 'Black'



UPDATE [dbo].[Highlight]
SET HighlightValue = '#FFB0CB'
WHERE HighlightName = 'Pink'


UPDATE [dbo].[Highlight]
SET HighlightValue = '#E7C531'
WHERE HighlightName = 'Gold'




COMMIT TRANSACTION;





/* Below run in Production on 7/27/16 */

/* 7/24/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;





ALTER TABLE [dbo].[Draft]
ADD [PickTimeSeconds] smallint NOT NULL DEFAULT(120)
GO






COMMIT TRANSACTION;








/* Below Run in Production on 7/22/16 */

/* 7/10/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;


INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Fantasypros - Dynasty','https://www.fantasypros.com/nfl/rankings/dynasty-overall.php',getdate(),getdate())
GO





ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandBUP] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideBUP] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [BUPId] int NULL
GO







COMMIT TRANSACTION;










/* Below Run in Production on 7/10/16 */

/* 7/7/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



INSERT INTO dbo.ByeWeek VALUES (2016, 'ARI', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'ATL', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'BAL', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'BUF', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'CAR', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'CHI', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'CIN', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'CLE', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'DAL', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'DEN', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'DET', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'GB', '4', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'HOU', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'IND', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'JAX', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'KC', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'LA', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'MIA', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'MIN', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'NE', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'NO', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'NYG', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'NYJ', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'OAK', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'PHI', '4', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'PIT', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'SD', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'SF', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'SEA', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'TB', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'TEN', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2016, 'WAS', '9', getdate(), getdate());





COMMIT TRANSACTION;











/* Below Run in Production on 7/6/16 */

/* 4/27/16 */
--Guess could've called it "AuditStatus" table

SET XACT_ABORT ON
BEGIN TRANSACTION;



-- DONE
--Dont Run This Script until below has been addressed!
--Players to Update:
	-- Ted Ginn Jr.
	-- Duke Johnson Jr.
	-- Benjamin Cunningham
	-- Terrelle Pryor?
--* ALSO DONT FORGET TO ALTER THE LoadPlayerRanks SP!





USE [Home]
GO

/****** Object:  Table [dbo].[AudioApi]    Script Date: 7/1/2016 11:31:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AudioApi](
	[AudioApiCode] [varchar](10) NOT NULL,
	[AudioApiKey] [varchar](50) NULL,
	[AudioApiUrl] [varchar](250) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_AudioApi] PRIMARY KEY CLUSTERED 
(
	[AudioApiId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO







/****** Object:  Table [dbo].[AudioCount]    Script Date: 7/2/2016 3:15:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AudioCount](
	[AudioCountId] [int] IDENTITY(1,1) NOT NULL,
	[AudioApiCode] [varchar](10) NOT NULL,
	[CallDate] [date] NOT NULL,
	[CallCount] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_AudioCount] PRIMARY KEY CLUSTERED 
(
	[AudioCountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[AudioCount]  WITH CHECK ADD  CONSTRAINT [FK_AudioCount_AudioApi] FOREIGN KEY([AudioApiCode])
REFERENCES [dbo].[AudioApi] ([AudioApiCode])
GO

ALTER TABLE [dbo].[AudioCount] CHECK CONSTRAINT [FK_AudioCount_AudioApi]
GO






ALTER TABLE [dbo].[LeagueOwner]
ADD [AnnounceAllPicks] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[LeagueOwner]
ADD [AnnouncePrevPick] bit NOT NULL DEFAULT(0)
GO




USE [Home]
GO

INSERT INTO [dbo].[AudioApi]
           ([AudioApiCode],[AudioApiKey],[AudioApiUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('first','dacf8932bf0c48d2883e623eddffee0b','http://api.voicerss.org/?key=dacf8932bf0c48d2883e623eddffee0b&src=<<audiotext>>&hl=en-us&f=16khz_16bit_stereo&c=mp3', getdate(), getdate()),
		   ('second','1858b3683b964a1fabed914b373de36f','http://api.voicerss.org/?key=1858b3683b964a1fabed914b373de36f&src=<<audiotext>>&hl=en-us&f=16khz_16bit_stereo&c=mp3', getdate(), getdate()),
		   ('demo',NULL,'http://www.voicerss.org/controls/speech.ashx?hl=en-us&src=<<audiotext>>&c=mp3&rnd=', getdate(), getdate())
GO






/****** Object:  Table [dbo].[AudioUserCount]    Script Date: 7/3/2016 1:08:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AudioUserCount](
	[AudioUserCountId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DraftId] [int] NOT NULL,
	[PickNum] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_AudioUserCount] PRIMARY KEY CLUSTERED 
(
	[AudioUserCountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [IX_UserDraftPickPlayer]    Script Date: 7/3/2016 1:08:35 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserDraftPickPlayer] ON [dbo].[AudioUserCount]
(
	[UserId] ASC,
	[DraftId] ASC,
	[PickNum] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AudioUserCount]  WITH CHECK ADD  CONSTRAINT [FK_AudioUserCount_AudioUserCount] FOREIGN KEY([AudioUserCountId])
REFERENCES [dbo].[AudioUserCount] ([AudioUserCountId])
GO

ALTER TABLE [dbo].[AudioUserCount] CHECK CONSTRAINT [FK_AudioUserCount_AudioUserCount]
GO

ALTER TABLE [dbo].[AudioUserCount]  WITH CHECK ADD  CONSTRAINT [FK_AudioUserCount_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[AudioUserCount] CHECK CONSTRAINT [FK_AudioUserCount_Draft]
GO

ALTER TABLE [dbo].[AudioUserCount]  WITH CHECK ADD  CONSTRAINT [FK_AudioUserCount_Player] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Player] ([PlayerId])
GO

ALTER TABLE [dbo].[AudioUserCount] CHECK CONSTRAINT [FK_AudioUserCount_Player]
GO

ALTER TABLE [dbo].[AudioUserCount]  WITH CHECK ADD  CONSTRAINT [FK_AudioUserCount_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[AudioUserCount] CHECK CONSTRAINT [FK_AudioUserCount_User]
GO















ALTER TABLE [dbo].[NFLTeam]
ADD [IsActive] bit NOT NULL DEFAULT(1)
GO

UPDATE [dbo].[NFLTeam]
SET IsActive = 0
WHERE TeamAbbr = 'STL'






USE [Home]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AutoImport](
	[AutoImportId] [int] IDENTITY(1,1) NOT NULL,
	[RankName] [varchar](100) NOT NULL,
	[ImportUrl] [varchar](1000) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_AutoImport] PRIMARY KEY CLUSTERED 
(
	[AutoImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





ALTER TABLE [dbo].[Rank]
ADD [AutoImportId] [int] NULL
GO


ALTER TABLE [dbo].[Rank]  WITH CHECK ADD  CONSTRAINT [FK_Rank_AutoImport] FOREIGN KEY([AutoImportId])
REFERENCES [dbo].[AutoImport] ([AutoImportId])
GO

ALTER TABLE [dbo].[Rank] CHECK CONSTRAINT [FK_Rank_AutoImport]
GO



INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('ESPN Top 300','http://espn.go.com/fantasy/football/story/_/id/16287927/fantasy-football-rankings-2016-espn-nfl-rankings-top-300-overall',getdate(), getdate())
GO

INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Fantasypros - Standard','http://fantasypros.com/nfl/rankings/consensus-cheatsheets.php',getdate(), getdate())
GO

INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('ESPN ADP','http://games.espn.go.com/ffl/livedraftresults',getdate(), getdate())
GO

INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Fantasypros ADP','https://www.fantasypros.com/nfl/adp/overall.php',getdate(), getdate())
GO

INSERT INTO [dbo].[AutoImport]
           ([RankName],[ImportUrl],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Yahoo!','https://partners.fantasypros.com/external/widget/nfl-staff-rankings.php?source=2&year=2016&week=0&position=ALL&scoring=STD&ajax=true&width=640',getdate(), getdate())
GO










/* TODO: Handle LA / Sunsetting StL !!! */


INSERT INTO [dbo].[NFLTeam]
     VALUES
           ('LA', 'LA', 'Los Angeles', 'Rams', 'NFC', 'West')
GO

USE [Home]
GO

INSERT INTO [dbo].[Player]
           ([TruePlayerId],[FirstName],[LastName],[Position],[NFLTeam],[DateOfBirth],[IsActive],[IsDrafted],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (144,'Los Angeles','Rams','DEF','LA',NULL,1,1,getdate(),getdate())
GO






ALTER TABLE [dbo].[AdminStatus] DROP CONSTRAINT [FK_AdminStatus_User]
GO

/****** Object:  Table [dbo].[AdminStatus]    Script Date: 4/27/2016 11:38:37 PM ******/
DROP TABLE [dbo].[AdminStatus]
GO

/****** Object:  Table [dbo].[AdminStatus]    Script Date: 4/27/2016 11:38:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdminStatus](
	[UserId] [int] NOT NULL,
	[LastPlayerAdjView] [datetime] NULL,
 CONSTRAINT [PK_AdminStatus] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AdminStatus]  WITH CHECK ADD  CONSTRAINT [FK_AdminStatus_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[AdminStatus] CHECK CONSTRAINT [FK_AdminStatus_User]
GO





ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandAvg] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideAvg] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [IsComparingRanks] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [CompareRankIds] varchar(500) NULL
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [CompRankExpandIds] varchar(500) NULL
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [CompRanksExpandAll] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [ShowAvgCompRanks] bit NOT NULL DEFAULT(0)
GO











COMMIT TRANSACTION;










/* Below Run in Production on 3/19/16 */

/* 3/19/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



/* 3/13/16 */


USE [Home]
GO

ALTER TABLE [dbo].[Role]
ADD [AddTimestamp] datetime NULL
GO

ALTER TABLE [dbo].[Role]
ADD [LastUpdateTimestamp] datetime NULL
GO

UPDATE [dbo].[Role]
SET AddTimestamp = '2014-06-01 00:00:00',
	LastUpdateTimestamp = '2014-06-01 00:00:00'
WHERE RoleId = 1

ALTER TABLE [dbo].[Role]
ALTER COLUMN [RoleDescription] varchar(30) NOT NULL
GO

ALTER TABLE [dbo].[Role]
ALTER COLUMN [AddTimestamp] datetime NOT NULL
GO

ALTER TABLE [dbo].[Role]
ALTER COLUMN [LastUpdateTimestamp] datetime NOT NULL
GO

USE [Home]
GO

INSERT INTO [dbo].[Role] ([RoleDescription], [AddTimestamp], [LastUpdateTimestamp])
     VALUES
           ('Commish', getdate(), getdate())
GO




USE [Home]
GO

ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_User]
GO

ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Role]
GO

/****** Object:  Table [dbo].[UserRole]    Script Date: 3/13/2016 1:48:38 PM ******/
DROP TABLE [dbo].[UserRole]
GO

/****** Object:  Table [dbo].[UserRole]    Script Date: 3/13/2016 1:48:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRole](
	[UserRoleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[LeagueId] [int] NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [IX_User]    Script Date: 3/13/2016 2:58:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_User] ON [dbo].[UserRole]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UserRoleLeague]    Script Date: 3/13/2016 1:48:38 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserRoleLeague] ON [dbo].[UserRole]
(
	[UserId] ASC,
	[RoleId] ASC,
	[LeagueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_League] FOREIGN KEY([LeagueId])
REFERENCES [dbo].[League] ([LeagueId])
GO

ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_League]
GO

ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO

ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO

ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO



USE [Home]
GO

INSERT INTO [dbo].[UserRole]
           ([UserId],[RoleId],[LeagueId],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (23, 1, NULL, '2014-06-01 00:00:00', '2014-06-01 00:00:00')
GO

--INSERT INTO [dbo].[UserRole]
--           ([UserId],[RoleId],[LeagueId],[AddTimestamp],[LastUpdateTimestamp])
--     VALUES
--           (23, 2, 1, getdate(), getdate())
--GO

--INSERT INTO [dbo].[UserRole]
--           ([UserId],[RoleId],[LeagueId],[AddTimestamp],[LastUpdateTimestamp])
--     VALUES
--           (23, 2, 4, getdate(), getdate())
--GO

--INSERT INTO [dbo].[UserRole]
--           ([UserId],[RoleId],[LeagueId],[AddTimestamp],[LastUpdateTimestamp])
--     VALUES
--           (23, 2, 5, getdate(), getdate())
--GO

--INSERT INTO [dbo].[UserRole]
--           ([UserId],[RoleId],[LeagueId],[AddTimestamp],[LastUpdateTimestamp])
--     VALUES
--           (23, 2, 6, getdate(), getdate())
--GO






USE [Home]
GO

DELETE FROM PlayerHighlight
DELETE FROM Highlight

DBCC CHECKIDENT('Highlight', RESEED, 0)
DBCC CHECKIDENT('PlayerHighlight', RESEED, 0)




INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Yellow','bg-yellow','yellow',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Lime','bg-lime','lime',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Cyan','bg-cyan','cyan',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Orange','bg-orange','#FFA011',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Red','bg-red','#FF4444',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Pink','bg-pink','pink',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Magenta','bg-magenta','#FF00FF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Light Blue','bg-light-blue','#87CEEB',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Blue','bg-blue','#6767FF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Green','bg-green','#449944',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Brown','bg-brown','#B36E39',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Grey (clear)','bg-grey','#EFEEEF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('White','bg-white','white',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Black','bg-black','black',getdate(),getdate())
GO





COMMIT TRANSACTION;






/* Below Run in Production on 3/9/16 */

/* 3/9/16 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



USE [Home]
GO

ALTER TABLE [dbo].[PlayerHighlight] DROP CONSTRAINT [FK_PlayerHighlight_User]
GO

ALTER TABLE [dbo].[PlayerHighlight] DROP CONSTRAINT [FK_PlayerHighlight_Player]
GO

ALTER TABLE [dbo].[PlayerHighlight] DROP CONSTRAINT [FK_PlayerHighlight_Highlight]
GO

ALTER TABLE [dbo].[PlayerHighlight] DROP CONSTRAINT [FK_PlayerHighlight_Draft]
GO

/****** Object:  Index [IX_DraftId_UserId]    Script Date: 3/6/2016 10:11:44 PM ******/
DROP INDEX [IX_DraftId_UserId] ON [dbo].[PlayerHighlight]
GO

/****** Object:  Table [dbo].[PlayerHighlight]    Script Date: 3/6/2016 10:11:44 PM ******/
DROP TABLE [dbo].[PlayerHighlight]
GO

/****** Object:  Table [dbo].[PlayerHighlight]    Script Date: 3/6/2016 10:11:44 PM ******/
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
	[HighlightClass]  AS ([dbo].[GetHighlightClass]([HighlightId])),
	[RankNum] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_PlayerHighlight] PRIMARY KEY CLUSTERED 
(
	[PlayerHighlightId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_DraftUserPlayer] UNIQUE NONCLUSTERED 
(
	[DraftId] ASC,
	[UserId] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Index [IX_DraftId_UserId]    Script Date: 3/6/2016 10:11:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_DraftId_UserId] ON [dbo].[PlayerHighlight]
(
	[DraftId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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





USE [Home]
GO

/****** Object:  Index [IX_UserId]    Script Date: 3/9/2016 1:03:38 AM ******/
DROP INDEX [IX_UserId] ON [dbo].[PlayerRankOption]
GO

/****** Object:  Table [dbo].[PlayerRankOption]    Script Date: 3/9/2016 1:03:38 AM ******/
DROP TABLE [dbo].[PlayerRankOption]
GO

/****** Object:  Table [dbo].[PlayerRankOption]    Script Date: 3/9/2016 1:03:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PlayerRankOption](
	[PlayerRankOptionId] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[RankId] [int] NULL,
	[DraftId] [int] NULL,
	[ExpandOverall] [bit] NOT NULL,
	[ExpandQB] [bit] NOT NULL,
	[ExpandRB] [bit] NOT NULL,
	[ExpandWRTE] [bit] NOT NULL,
	[ExpandDEF] [bit] NOT NULL,
	[ExpandK] [bit] NOT NULL,
	[ExpandQueue] [bit] NOT NULL,
	[HideOverall] [bit] NOT NULL,
	[HideQB] [bit] NOT NULL,
	[HideRB] [bit] NOT NULL,
	[HideWRTE] [bit] NOT NULL,
	[HideDEF] [bit] NOT NULL,
	[HideK] [bit] NOT NULL,
	[HideQueue] [bit] NOT NULL,
	[ShowHighlighting] [bit] NOT NULL,
	[LockHighlighting] [bit] NOT NULL,
	[HighlightColor] [varchar](50) NULL,
	[AddTimestamp] [datetime] NULL,
	[LastUpdateTimestamp] [datetime] NULL,
 CONSTRAINT [PK_PlayerRankOption_1] PRIMARY KEY CLUSTERED 
(
	[PlayerRankOptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Index [IX_UserId]    Script Date: 3/9/2016 1:03:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[PlayerRankOption]
(
	[PlayerRankOptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO









COMMIT TRANSACTION;











/* Below Run in Production on 3/6/16 */

/* 3/3/16 */

USE [Home]
GO

/****** Object:  Table [dbo].[Highlight]    Script Date: 3/6/2016 2:27:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Highlight](
	[HighlightId] [int] IDENTITY(1,1) NOT NULL,
	[HighlightName] [varchar](50) NOT NULL,
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

CREATE FUNCTION [dbo].[GetHighlightClass] (@HighlightId INT)
RETURNS VARCHAR(51) 
AS BEGIN
    DECLARE @HighlightClass VARCHAR(50)

    SELECT @HighlightClass = HighlightClass FROM [dbo].[Highlight] WHERE HighlightId = @HighlightId

    RETURN @HighlightClass
END

GO






USE [Home]
GO

/****** Object:  Table [dbo].[PlayerHighlight]    Script Date: 3/5/2016 3:56:25 PM ******/
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
	[HighlightClass]  AS ([dbo].[GetHighlightClass]([HighlightId])),
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

/****** Object:  Index [IX_DraftId_UserId]    Script Date: 3/5/2016 3:56:25 PM ******/
CREATE NONCLUSTERED INDEX [IX_DraftId_UserId] ON [dbo].[PlayerHighlight]
(
	[DraftId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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






USE [Home]
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Yellow','bg-yellow','yellow',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Lime','bg-lime','lime',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Cyan','bg-cyan','cyan',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Orange','bg-orange','#FFA011',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Red','bg-red','#FF4444',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Pink','bg-pink','pink',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Magenta','bg-magenta','#FF00FF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Light Blue','bg-light-blue','#87CEEB',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Blue','bg-blue','#6767FF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Green','bg-green','#449944',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Brown','bg-brown','#B36E39',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Grey (clear)','bg-grey','#EFEEEF',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('White','bg-white','white',getdate(),getdate())
GO

INSERT INTO [dbo].[Highlight]
           ([HighlightName],[HighlightClass],[HighlightValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('Black','bg-black','black',getdate(),getdate())
GO




COMMIT TRANSACTION;


--TODO:  Add the Indexes I put in the DB for PlayerHighlight !!!
--TODO:  Add the Index & Uniqueness constraint for HighlightClass in Highlight table
-- NOTE:  DraftYear cannot be null going forward


/* ... Below Run in Production on ... */