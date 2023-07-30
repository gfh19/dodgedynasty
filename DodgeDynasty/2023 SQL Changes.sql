
/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */


/* Below run in Production on 7/29/2023 */


SET XACT_ABORT ON
BEGIN TRANSACTION;


ALTER TABLE [dbo].[Draft]
ADD [IsPaused] bit NOT NULL DEFAULT(0);
GO


COMMIT TRANSACTION;



SET XACT_ABORT ON
BEGIN TRANSACTION;

INSERT INTO [dbo].[SiteConfigVar]
           ([VarName],[VarValue],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           ('PushNotificationsKillSwitch', 'false', getdate(), getdate())
GO


COMMIT TRANSACTION;



/* Below run in Production on 7/25/2023 */


SET XACT_ABORT ON
BEGIN TRANSACTION;



INSERT INTO dbo.ByeWeek VALUES ('2023', 'ARI', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'ATL', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'BAL', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'BUF', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'CAR', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'CHI', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'CIN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'CLE', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'DAL', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'DEN', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'DET', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'GB', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'HOU', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'IND', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'JAX', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'KC', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'LV', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'LAC', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'LAR', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'MIA', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'MIN', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'NE', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'NO', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'NYG', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'NYJ', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'PHI', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'PIT', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'SF', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'SEA', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'TB', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'TEN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2023', 'WAS', '14', getdate(), getdate());





COMMIT TRANSACTION;






SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/23/NFL23_CS_Non300.pdf?adddata=2023CS_Non300',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2023&week=0&id=1054&position=ALL&type=ST&scoring=STD&filters=7%3A9%3A285%3A747',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5



COMMIT TRANSACTION;






/* Below run in Production on 7/19/2023 */


SET XACT_ABORT ON
BEGIN TRANSACTION;



/****** Object:  Table [dbo].[Notification]    Script Date: 7/19/2023 8:47:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notification](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[EndPoint] [varchar](1000) NOT NULL,
	[P256dh] [varchar](1000) NOT NULL,
	[Auth] [varchar](1000) NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




COMMIT TRANSACTION;







/* Below run in Production on 6/12/2023 */


SET XACT_ABORT ON
BEGIN TRANSACTION;



/****** Object:  Table [dbo].[Schedule]    Script Date: 6/12/2023 9:54:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Schedule](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Schedule]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[Schedule] CHECK CONSTRAINT [FK_Schedule_User]
GO




/****** Object:  Table [dbo].[ScheduleMatchup]    Script Date: 6/12/2023 9:55:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScheduleMatchup](
	[MatchupId] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [int] NOT NULL,
	[MatchupType] [varchar](35) NOT NULL,
	[AwayTeam] [varchar](35) NOT NULL,
	[HomeTeam] [varchar](35) NOT NULL,
	[Year] [int] NOT NULL,
	[Week] [int] NULL,
	[UserId] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_ScheduleMatchup] PRIMARY KEY CLUSTERED 
(
	[MatchupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ScheduleMatchup]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleMatchup_Schedule] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO

ALTER TABLE [dbo].[ScheduleMatchup] CHECK CONSTRAINT [FK_ScheduleMatchup_Schedule]
GO







COMMIT TRANSACTION;









/*
TODO:

7/25/23
Darrell Henderson Jr
Melvin Gordon III
Mecole Hardman Jr.
Jeff Wilson Jr.
Nathaniel Dell / * Tank Dell *
Kenneth Walker / Ken Walker III / * Kenneth Walker III *


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
  6/13/2023
  Just hit up Trophy World in Mentor

  OLD:
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


