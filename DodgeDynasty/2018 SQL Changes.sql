/* Below run in Production on 7/26/18 */




SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'http://www.espn.com/fantasy/football/story/_/page/18RanksPreseason300nonPPR/2018-fantasy-football-non-ppr-rankings-top-300',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1



UPDATE [dbo].[Rank]
SET RankName = 'ESPN Top 300',
	Url = 'http://www.espn.com/fantasy/football/story/_/page/18RanksPreseason300nonPPR/2018-fantasy-football-non-ppr-rankings-top-300',
	LastUpdateTimestamp = getdate()
WHERE RankId = 82



COMMIT TRANSACTION;







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


/* Mark Todd Gurley II TPID (871) */
/* Consolidate Ronald Jones / II */
/* Mark Marvin Jones Jr. 
TPID (188) - Done */

/* Yahoo (SF 49ers)... */
/* Test with
Marquez Valdes-Scantling
Damore'ea Stringfellow
Tampa Bay Buccaneers
Equanimeous St. Brown

*/

/*
  NEXT YEAR 2019:
  Don't forget to re-order champion nameplate!
  www.trophypartner.com
  #NP100, Silver, 2" x 0.75"
  (Or take trophy to local shop & ask to reproduce cheaper)

*/


COMMIT TRANSACTION;
