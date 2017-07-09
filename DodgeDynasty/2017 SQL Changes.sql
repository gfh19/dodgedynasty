/* Below run in Production on 7/8/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;




USE [Home]
GO

/****** Object:  Table [dbo].[_ArchivePlayerHighlight]    Script Date: 7/8/2017 2:05:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[_ArchivePlayerHighlight](
	[ArchivePlayerHighlightId] [int] IDENTITY(1,1) NOT NULL,
	[PlayerHighlightId] [int] NOT NULL,
	[DraftId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[HighlightId] [int] NOT NULL,
	[RankNum] [int] NOT NULL,
	[AddTimestamp] [datetime] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
	[ArchiveAddTimestamp] [datetime] NULL
) ON [PRIMARY]

GO




/* Archive Player Highlights ! */

INSERT INTO [dbo].[_ArchivePlayerHighlight]
           ([PlayerHighlightId]
           ,[DraftId]
           ,[UserId]
           ,[PlayerId]
           ,[HighlightId]
           ,[RankNum]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp]
           ,[ArchiveAddTimestamp])
     SELECT [PlayerHighlightId]
           ,[DraftId]
           ,[UserId]
           ,[PlayerId]
           ,[HighlightId]
           ,[RankNum]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp]
           ,getdate()
		FROM dbo.PlayerHighlight
GO



DELETE FROM dbo.PlayerHighlight







COMMIT TRANSACTION;









/* Below run in Production on 7/8/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;





INSERT INTO [dbo].[Role]
           ([RoleDescription]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp])
     VALUES
           ('Guest'
           ,getdate()
           ,getdate())
GO



INSERT INTO [dbo].[UserRole]
           ([UserId]
           ,[RoleId]
           ,[LeagueId]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp])
     VALUES
           (43
           ,3
           ,NULL
           ,getdate()
           ,getdate())
GO





INSERT INTO [dbo].[NFLTeam]
           ([TeamAbbr],[AbbrDisplay],[LocationName],[TeamName],[Conference],[Division],[IsActive])
     VALUES
           ('LAR', 'LAR', 'Los Angeles', 'Rams', 'NFC', 'West', 1)
GO


UPDATE [dbo].[Player]
SET [NFLTeam] = 'LAR'
WHERE [NFLTeam] = 'LA'
GO


UPDATE [dbo].[ByeWeek]
   SET [NFLTeam] = 'LAR'
 WHERE [NFLTeam] = 'LA'
GO


DELETE FROM [dbo].[NFLTeam]
WHERE TeamAbbr = 'LA'
GO




INSERT INTO [dbo].[NFLTeam]
           ([TeamAbbr],[AbbrDisplay],[LocationName],[TeamName],[Conference],[Division],[IsActive])
     VALUES
           ('LAC', 'LAC', 'Los Angeles', 'Chargers', 'AFC', 'West', 1)
GO


/* No do not update all old SD -> LAC, this was a mistake */
/*
DO NOT UPDATE [dbo].[Player]
SET [NFLTeam] = 'LAC'
WHERE [NFLTeam] = 'SD'
*/


UPDATE [dbo].[NFLTeam]
SET IsActive = 0
WHERE TeamAbbr = 'SD'




/* ESPN Top 200 */
UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 200',
	ImportUrl = 'http://www.espn.com/fantasy/football/story/_/page/17RanksPreseason200nonPPR/2017-fantasy-football-standard-rankings-non-ppr-top-200',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1

/* Yahoo */
UPDATE [dbo].[AutoImport]
SET ImportUrl = 'https://partners.fantasypros.com/external/widget/nfl-staff-rankings.php?source=2&year=2017&week=0&position=ALL&scoring=STD&ajax=true&width=640',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5


/* ADD LOS ANGELES CHARGERS DEFENSE, TPID 331 !!! - Done */
/* ADD MIKE WILLIAMS (LAC), NOT TPID 201 !!! - Done */





COMMIT TRANSACTION;







/* Below run in Production on 7/6/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;






CREATE NONCLUSTERED INDEX [IX_RankId] ON [dbo].[PlayerRank]
(
	[RankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO






COMMIT TRANSACTION;







/* Below run in Production on 6/10/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;





ALTER TABLE [dbo].[DraftRank]  WITH CHECK ADD  CONSTRAINT [FK_DraftRank_Rank] FOREIGN KEY([RankId])
REFERENCES [dbo].[Rank] ([RankId])
GO

ALTER TABLE [dbo].[DraftRank] CHECK CONSTRAINT [FK_DraftRank_Rank]
GO


DELETE
  FROM [dbo].[DraftRank]
  WHERE RankId = 68

DELETE
FROM [dbo].[Rank]
  WHERE RankId = 68


DELETE
  FROM [dbo].[DraftRank]
  WHERE RankId = 69

DELETE
FROM [dbo].[Rank]
  WHERE RankId = 69




COMMIT TRANSACTION;




/* Below run in Production on 6/10/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



ALTER TABLE [dbo].[League]
ADD [NumRounds] smallint NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET NumRounds = 15
WHERE LeagueId IN (1, 2, 7)

UPDATE [dbo].[League]
SET NumRounds = 17
WHERE LeagueId IN (8)

UPDATE [dbo].[League]
SET NumRounds = 4
WHERE LeagueId IN (3, 6, 9)

UPDATE [dbo].[League]
SET NumRounds = 6
WHERE LeagueId IN (4)

UPDATE [dbo].[League]
SET NumRounds = 3
WHERE LeagueId IN (5)

UPDATE [dbo].[League]
SET NumRounds = 8
WHERE LeagueId IN (10)

ALTER TABLE [dbo].[League]
ADD [NumKeepers] smallint NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET NumKeepers = 0

UPDATE [dbo].[League]
SET NumKeepers = 6
WHERE LeagueId IN (1)

UPDATE [dbo].[League]
SET NumKeepers = 1
WHERE LeagueId IN (2)

ALTER TABLE [dbo].[League]
ADD [Format] varchar(10) NOT NULL DEFAULT('snake')
GO

UPDATE [dbo].[League]
SET [Format] = 'snake'

UPDATE [dbo].[League]
SET [Format] = 'repeat'
WHERE LeagueId IN (1)

ALTER TABLE [dbo].[League]
ADD [CombineWRTE] bit NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET [CombineWRTE] = 0

UPDATE [dbo].[League]
SET [CombineWRTE] = 1
WHERE LeagueId IN (1, 2, 4, 8)

ALTER TABLE [dbo].[League]
ADD [PickTimeSeconds] smallint NOT NULL DEFAULT(120)
GO



ALTER TABLE [dbo].[Draft]
ADD [CombineWRTE] bit NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[Draft]
SET [CombineWRTE] = 0

UPDATE [dbo].[Draft]
SET [CombineWRTE] = 1
WHERE LeagueId IN (1, 2, 4, 8)

ALTER TABLE [dbo].[Draft]
DROP COLUMN [NumOwners];
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [NumRounds] smallint NOT NULL;
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [NumKeepers] smallint NOT NULL;
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [Format] varchar(10) NOT NULL
GO


/* DON'T FORGET to set defaults for NumRounds, NumKeepers, Format, PickTimeSeconds, and CombineWRTE for all existing leagues !!! */


ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandWR] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideWR] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandTE] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideTE] bit NOT NULL DEFAULT(0)
GO





COMMIT TRANSACTION;
