
/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */



/* Below run in Production on 7/19/2023 */


SET XACT_ABORT ON
BEGIN TRANSACTION;

ALTER TABLE [dbo].[Notification]
ADD [JsonBody] [varchar](2000) NULL;
GO


COMMIT TRANSACTION;


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


