﻿/* Below run in Production on 6/16/2022 */



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





/* Below run in Production on 7/28/2021 */


--1

SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/21/NFLDK2021_CS_NonPPR300.pdf',  --Sad Day
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Fantasypros - Standard',
	ImportUrl = 'http://startdrafting.com/docs/Rankings/FprosStandard.html',  --Sadder Day
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 2

UPDATE [dbo].[AutoImport]
SET RankName = 'Fantasypros - Dynasty',
	ImportUrl = 'http://startdrafting.com/docs/Rankings/FprosDynasty.html',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 6


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2021&week=0&id=1054&position=ALL&type=ST&scoring=STD&filters=7:9:285:699:747',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5



COMMIT TRANSACTION;



/*
TODO:
7/27/2021
- Willie Snead IV
- TE-BOW TE-BOW TE-BOW TE-BOW!

2290	Jakeem Grant Sr.
2289	Richie James Jr.
2288	Pooka Williams Jr.
2253	Lynn Bowden Jr.
2252	Steven Sims Jr.
2233	Keelan Cole Sr.

- Jeff Wilson Jr.
- Melvin Gordon III
- Allen Robinson II
- Patrick Mahomes II
	- Fix this right now
	- Because of trim.... "Patrick Mahomes II "


  NEXT YEAR:
  Don't forget to re-order champion nameplate!
  www.trophypartner.com
  #NP100, Silver, 2" x 0.75"
  (Or take trophy to local shop & ask to reproduce cheaper)
  Check font

  - DELETE extra Washington Redskins "WAS" Defense players (if necessary)! 
  - Deactivate All But 32 DEFs
  - BYE WEEKS!
  - Also, when AutoImporting ranks, start with a Fantasypros one for better player names
  - Maybe clean out some Archive/History tables?
*/

