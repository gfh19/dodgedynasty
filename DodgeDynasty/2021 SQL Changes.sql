/* Below run in Production on 8/5/2021 */



SET XACT_ABORT ON
BEGIN TRANSACTION;


ALTER TABLE [dbo].[AutoImport]
ADD [IsPdf] bit NOT NULL DEFAULT(0);
GO


COMMIT TRANSACTION;



SET XACT_ABORT ON
BEGIN TRANSACTION;


UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
    IsApi = 0,
	IsPdf = 1
WHERE AutoImportId = 1


COMMIT TRANSACTION;









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
*/


