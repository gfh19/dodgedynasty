

/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */




/* Below run in Production on 7/30/2022 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



INSERT INTO dbo.ByeWeek VALUES ('2022', 'ARI', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'ATL', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'BAL', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'BUF', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'CAR', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'CHI', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'CIN', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'CLE', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'DAL', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'DEN', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'DET', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'GB', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'HOU', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'IND', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'JAX', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'KC', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'LV', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'LAC', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'LAR', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'MIA', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'MIN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'NE', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'NO', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'NYG', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'NYJ', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'PHI', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'PIT', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'SF', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'SEA', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'TB', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'TEN', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2022', 'WAS', '14', getdate(), getdate());




COMMIT TRANSACTION;







SET XACT_ABORT ON
BEGIN TRANSACTION;




/****** Object:  Table [dbo].[DraftHighlight]    Script Date: 7/22/2022 10:40:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DraftHighlight](
	[DraftHighlightId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DraftYear] [smallint] NOT NULL,
	[DraftId] [int] NULL,
	[QueueName] [varchar](25) NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_DraftHighlight] PRIMARY KEY CLUSTERED 
(
	[DraftHighlightId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DraftHighlight]  WITH CHECK ADD  CONSTRAINT [FK_DraftHighlight_Draft] FOREIGN KEY([DraftId])
REFERENCES [dbo].[Draft] ([DraftId])
GO

ALTER TABLE [dbo].[DraftHighlight] CHECK CONSTRAINT [FK_DraftHighlight_Draft]
GO

ALTER TABLE [dbo].[DraftHighlight]  WITH CHECK ADD  CONSTRAINT [FK_DraftHighlight_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[DraftHighlight] CHECK CONSTRAINT [FK_DraftHighlight_User]
GO


ALTER TABLE [dbo].[DraftHighlight]
ADD	[AddTimestamp] [datetime] NOT NULL;
GO

ALTER TABLE [dbo].[DraftHighlight]
ADD	[LastUpdateTimestamp] [datetime] NOT NULL;
GO




/****** Object:  Table [dbo].[PlayerHighlight]    Script Date: 7/22/2022 10:55:35 PM ******/
ALTER TABLE [dbo].[PlayerHighlight]
ADD [DraftHighlightId] [int] NULL;
GO


/****** Object:  Index [UQ_DraftUserPlayer]    Script Date: 7/22/2022 10:57:11 PM ******/
ALTER TABLE [dbo].[PlayerHighlight] DROP CONSTRAINT [UQ_DraftUserPlayer]
GO

/****** Object:  Index [UQ_DraftUserPlayer]    Script Date: 7/22/2022 10:57:11 PM ******/
ALTER TABLE [dbo].[PlayerHighlight] ADD  CONSTRAINT [UQ_DraftUserPlayer] UNIQUE NONCLUSTERED 
(
	[DraftId] ASC,
	[UserId] ASC,
	[PlayerId] ASC,
	[DraftHighlightId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[PlayerHighlight]  WITH CHECK ADD  CONSTRAINT [FK_PlayerHighlight_DraftHighlight] FOREIGN KEY([DraftHighlightId])
REFERENCES [dbo].[DraftHighlight] ([DraftHighlightId])
GO





COMMIT TRANSACTION;







/* Below run in Production on 7/22/2022 */



SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/22/NFLDK2022_CS_NonPPR300.pdf',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2022&week=0&id=1663&position=ALL&type=ST&scoring=STD&filters=7:9:285:699:747',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5



COMMIT TRANSACTION;





/* Below run in Production on 7/22/2022 */


SET XACT_ABORT ON
BEGIN TRANSACTION;

--Purging:

DELETE FROM [dbo].[_ArchivePlayerHighlight]
DELETE FROM [dbo].[_HistoryDraftPick]
DELETE FROM [dbo].[_HistoryPlayer]
DELETE FROM [dbo].[PlayerRankHistory]


COMMIT TRANSACTION;











/* Below run in Production on 7/22/2022 */



SET XACT_ABORT ON
BEGIN TRANSACTION;


UPDATE [dbo].[NFLTeam]
SET TeamName = 'Commanders'
WHERE TeamId = '32'


INSERT INTO [dbo].[NFLTeam]
           ([TeamAbbr]
           ,[AbbrDisplay]
           ,[LocationName]
           ,[TeamName]
           ,[Conference]
           ,[Division]
           ,[IsActive])
     VALUES
           ('WFT'
           ,'Was'
           ,'Washington'
           ,'Football Team'
           ,'NFC'
           ,'East'
           ,0)
GO


UPDATE Player
SET NFLTeam = 'WFT'
WHERE NFLTeam = 'WAS'



UPDATE Player
SET IsActive = 0
WHERE PlayerName = 'Washington Football Team'



INSERT INTO [dbo].[Player]
           ([TruePlayerId]
           ,[FirstName]
           ,[LastName]
           ,[Position]
           ,[NFLTeam]
           ,[DateOfBirth]
           ,[IsActive]
           ,[IsDrafted]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp])
     VALUES
           (333
           ,'Washington'
           ,'Commanders'
           ,'DEF'
           ,'WAS'
           ,NULL
           ,1
           ,0
           ,getdate()
           ,getdate())
GO





COMMIT TRANSACTION;






/* Below run in Production on 6/16/2022 */



SET XACT_ABORT ON
BEGIN TRANSACTION;


/****** Object:  Table [dbo].[UserPreferences]    Script Date: 6/16/2022 8:30:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserPreferences](
	[UserId] [int] NOT NULL,
	[DraftShowPositionColors] [varchar](10) NULL,
	[TeamsShowPositionColors] [varchar](10) NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_UserPreferences] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserPreferences]  WITH CHECK ADD  CONSTRAINT [FK_UserPreferences_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[UserPreferences] CHECK CONSTRAINT [FK_UserPreferences_User]
GO




/****** Object:  Table [dbo].[UserApplicant]    Script Date: 6/16/2022 8:10:27 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserApplicant]') AND type in (N'U'))
DROP TABLE [dbo].[UserApplicant]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfile]') AND type in (N'U'))
DROP TABLE [dbo].[UserProfile]
GO



COMMIT TRANSACTION;




/* For source control history if ever needed these were the table create scripts:



/****** Object:  Table [dbo].[UserApplicant]    Script Date: 6/16/2022 8:11:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserApplicant](
	[UserId] [varchar](20) NOT NULL,
	[Pword] [varchar](20) NOT NULL,
	[FirstName] [varchar](30) NOT NULL,
	[LastName] [varchar](30) NOT NULL,
	[Email] [varchar](100) NULL,
	[Address1] [varchar](100) NULL,
	[Address2] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Secret] [varchar](250) NULL,
	[Accepted] [char](1) NOT NULL,
	[Rejected] [char](1) NOT NULL,
	[AddDateTime] [datetime] NULL,
 CONSTRAINT [PK_UserApplicant] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserApplicant] ADD  CONSTRAINT [DF_UserApplicant_Accepted]  DEFAULT ('N') FOR [Accepted]
GO

ALTER TABLE [dbo].[UserApplicant] ADD  CONSTRAINT [DF_UserApplicant_Rejected]  DEFAULT ('N') FOR [Rejected]
GO




/****** Object:  Table [dbo].[UserProfile]    Script Date: 6/16/2022 8:12:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
 CONSTRAINT [PK__UserProf__1788CC4C36B12243] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



*/









/* Below run in Production on 6/12/2022 */



SET XACT_ABORT ON
BEGIN TRANSACTION;


/****** Object:  Table [dbo].[PlayerAudio]    Script Date: 6/12/2022 7:39:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PlayerAudio](
	[PlayerAudioId] [int] IDENTITY(1,1) NOT NULL,
	[TruePlayerId] [int] NULL,
	[PlayerName] [varchar](51) NULL,
	[PlayerNameAudio] [varchar](100) NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_PlayerAudio] PRIMARY KEY CLUSTERED 
(
	[PlayerAudioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (123, 'Ben Roethlisberger', 'Alleged Sex Offender Ben Rawthlisberger', getdate(), getdate())
GO

INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'Los Angeles Chargers', 'San Diego I Mean Los Angeles Chargers', getdate(), getdate())
GO

INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'San Francisco 49ers', 'San Francisco Forty-Niners', getdate(), getdate())
GO

INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'Washington Commanders', 'Washington Redskins Football Team Commanders', getdate(), getdate())
GO

INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (1299, 'Deshaun Watson', 'Massage Enthuziest Deshaun Watson', getdate(), getdate())
GO

INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'George Kittle', 'Little George Kittle', getdate(), getdate())
GO




COMMIT TRANSACTION;








/*********** BELOW NOT RUN YET !!!!! ***********/

/* (Below not revisited yet, revisit in August when ranks available) */
/* Below run in Production on 7/31/2021 */



SET XACT_ABORT ON
BEGIN TRANSACTION;



INSERT INTO dbo.ByeWeek VALUES ('2021', 'ARI', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'ATL', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'BAL', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'BUF', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'CAR', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'CHI', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'CIN', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'CLE', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'DAL', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'DEN', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'DET', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'GB', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'HOU', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'IND', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'JAX', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'KC', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'LV', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'LAC', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'LAR', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'MIA', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'MIN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'NE', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'NO', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'NYG', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'NYJ', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'PHI', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'PIT', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'SF', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'SEA', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'TB', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'TEN', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2021', 'WAS', '9', getdate(), getdate());




COMMIT TRANSACTION;









/*
TODO:
7/22/2022
Player names to look out for:
- Demetric Felton Jr - RB
- Dee Eskridge
- Cedrick Wilson Jr - Mia
- WILLIAM FULLER V (cmon man)
- Joshua Palmer
- Robbie Anderson
- Allen Robinson II


- Keep an eye on:
  - Ken Walker III
  - Brian Robinson Jr.




  NEXT YEAR:
  Don't forget to re-order champion nameplate!
  www.trophypartner.com
  #NP100, Silver, 2" x 0.75"
  (Or take trophy to local shop & ask to reproduce cheaper)
  Check font

  - Deactivate All But 32 DEFs
  - BYE WEEKS!
        - e.g. \Google Drive\Fantasy Football\2022 Bye Weeks.xlsx
        - http://www.fantasypros.com/nfl/bye-weeks.php
           - No more jQuery format, just copy/paste from website to spreadsheet now
  - Also, when AutoImporting ranks, start with a Fantasypros one for better player names
  - Maybe clean out some Archive/History tables?
        7/22/2022:
        Purging:
DELETE FROM [dbo].[_ArchivePlayerHighlight]
DELETE FROM [dbo].[_HistoryDraftPick]
DELETE FROM [dbo].[_HistoryPlayer]
DELETE FROM [dbo].[PlayerRankHistory]

(Counts respectively were:  683, 2542, 2527, 23953)
([dbo].[DraftPickHistory] was already empty)
*/


