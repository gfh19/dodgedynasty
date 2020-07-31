/* Below run in Production on 7/26/2020 */


--1 

SET XACT_ABORT ON
BEGIN TRANSACTION;



INSERT INTO [dbo].[NFLTeam]
           ([TeamAbbr],[AbbrDisplay],[LocationName],[TeamName],[Conference],[Division],[IsActive])
     VALUES
           ('LV', 'LV', 'Las Vegas', 'Raiders', 'AFC', 'West', 1)
GO


UPDATE [dbo].[NFLTeam]
SET IsActive = 0
WHERE TeamAbbr = 'OAK'



COMMIT TRANSACTION;





--2

SET XACT_ABORT ON
BEGIN TRANSACTION;


DROP INDEX [IX_AbbrDisplay] ON [dbo].[NFLTeam]
GO



INSERT INTO [dbo].[NFLTeam]
           ([TeamAbbr],[AbbrDisplay],[LocationName],[TeamName],[Conference],[Division],[IsActive])
     VALUES
           ('WSH', 'Was', 'Washington', 'Redskins', 'NFC', 'East', 0)
GO


UPDATE [dbo].[NFLTeam]
SET [TeamName] = 'Football Team'
WHERE TeamAbbr = 'WAS'



COMMIT TRANSACTION;





--3

SET XACT_ABORT ON
BEGIN TRANSACTION;



UPDATE [dbo].[_HistoryPlayer]
SET [NFLTeam] = 'WSH'
WHERE [NFLTeam] = 'WAS'


UPDATE [dbo].[ByeWeek]
SET [NFLTeam] = 'WSH'
WHERE [NFLTeam] = 'WAS'


UPDATE [dbo].[Player]
SET [NFLTeam] = 'WSH'
WHERE [NFLTeam] = 'WAS'


UPDATE [dbo].[PlayerAdjustment]
SET [NewNFLTeam] = 'WSH'
WHERE [NewNFLTeam] = 'WAS'



COMMIT TRANSACTION;







--4

SET XACT_ABORT ON
BEGIN TRANSACTION;


INSERT INTO [dbo].[Player]
           ([TruePlayerId],[FirstName],[LastName],[Position],[NFLTeam],[DateOfBirth],[IsActive],[IsDrafted],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (328, 'Las Vegas', 'Raiders', 'DEF', 'LV', NULL, 1, 0, getdate(), getdate())
GO


INSERT INTO [dbo].[Player]
           ([TruePlayerId],[FirstName],[LastName],[Position],[NFLTeam],[DateOfBirth],[IsActive],[IsDrafted],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (333, 'Washington', 'Football Team', 'DEF', 'WAS', NULL, 1, 0, getdate(), getdate())
GO





COMMIT TRANSACTION;







--5

SET XACT_ABORT ON
BEGIN TRANSACTION;




INSERT INTO dbo.ByeWeek VALUES (2020, 'ARI', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'ATL', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'BAL', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'BUF', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'CAR', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'CHI', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'CIN', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'CLE', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'DAL', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'DEN', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'DET', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'GB', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'HOU', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'IND', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'JAX', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'KC', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'LV', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'LAC', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'LAR', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'MIA', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'MIN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'NE', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'NO', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'NYG', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'NYJ', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'PHI', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'PIT', '8', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'SF', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'SEA', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'TB', '13', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'TEN', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES (2020, 'WAS', '8', getdate(), getdate());





COMMIT TRANSACTION;






--6

SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://www.espn.com/fantasy/football/story/_/id/29083378/mike-clay-2020-updated-top-300-non-ppr-fantasy-football-rankings',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2020&week=0&id=1663&position=ALL&type=ST&scoring=STD&filters=7:8:9:285:699',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5



COMMIT TRANSACTION;


--Update Stephen Hauschka's TPID !
--Update George's NFL Playoff Teams ranks to 2019 !
--TBD - Check for duplicate suffixed players (Jr, Sr, III, etc)


/*
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
*/


