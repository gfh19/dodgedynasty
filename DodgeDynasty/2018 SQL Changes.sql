/* Below run in Production on 6/30/18 */

SET XACT_ABORT ON
BEGIN TRANSACTION;




ALTER TABLE [dbo].[AutoImport]
ADD [IsApi] bit NOT NULL DEFAULT(0);
GO


/* ESPN Top 200 */
UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 200',
	ImportUrl = 'http://www.espn.com/fantasy/football/story/_/page/18RanksPreseason200nonPPR/2018-fantasy-football-non-ppr-rankings-top-200',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1

/* Yahoo */
UPDATE [dbo].[AutoImport]
SET ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?experts=show&sport=NFL&year=2018&week=0&id=1054&position=ALL&type=ST&scoring=STD&filters=7:8:9:285:699&widget=ST&callback=FPW.rankingsCB',
	LastUpdateTimestamp = getdate(),
	IsApi = 1
WHERE AutoImportId = 5


/* Mark Todd Gurley II TPID (871) - Done */
/* Consolidate Ronald Jones / II - Done */
/* Mark Marvin Jones Jr. 
TPID (188) - Done */

/* Yahoo (SF 49ers)... */
/* Test with
Marquez Valdes-Scantling
Damore'ea Stringfellow
Tampa Bay Buccaneers

*/




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
