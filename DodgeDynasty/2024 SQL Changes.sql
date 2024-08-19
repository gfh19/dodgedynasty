
/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */




/* Below run in Production on 8/9/24 */



SET XACT_ABORT ON
BEGIN TRANSACTION;


INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'Cleveland Browns', 'Brook Park Browns', getdate(), getdate())
GO


INSERT INTO [dbo].[PlayerAudio]
           ([TruePlayerId],[PlayerName],[PlayerNameAudio],[AddTimestamp],[LastUpdateTimestamp])
     VALUES
           (NULL, 'Amon-Ra St. Brown', 'Amon-Rah Saint Brown', getdate(), getdate())
GO


COMMIT TRANSACTION;








/* Below run in Production on 7/14/24 */


SET XACT_ABORT ON
BEGIN TRANSACTION;


INSERT INTO dbo.ByeWeek VALUES ('2024', 'ARI', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'ATL', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'BAL', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'BUF', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'CAR', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'CHI', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'CIN', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'CLE', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'DAL', '7', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'DEN', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'DET', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'GB', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'HOU', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'IND', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'JAX', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'KC', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'LV', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'LAC', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'LAR', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'MIA', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'MIN', '6', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'NE', '14', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'NO', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'NYG', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'NYJ', '12', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'PHI', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'PIT', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'SF', '9', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'SEA', '10', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'TB', '11', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'TEN', '5', getdate(), getdate());
INSERT INTO dbo.ByeWeek VALUES ('2024', 'WAS', '14', getdate(), getdate());




COMMIT TRANSACTION;





/* Below run in Production on 7/13/24 */

SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/24/NFL24_CS_Non300.pdf?adddata=2024CS_Non300',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


/* Yahoo replaced below */
/*
UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2024&week=0&position=ALL&type=ST&scoring=HALF&filters=7%3A9%3A285%3A747',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5
*/



COMMIT TRANSACTION;



/* Below run in Production on 8/17/24 */

SET XACT_ABORT ON
BEGIN TRANSACTION;

/* Correct HALF call matching Yahoo web page */
UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?position=ALL&sport=NFL&year=2024&week=0&experts=show&id=1663&type=ST&scoring=HALF&filters=7%3A9%3A285%3A747%3A4338&widget=ST',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5


COMMIT TRANSACTION;








/*
TODO:

7/13/24
Ray-Ray McCloud III





  NEXT YEAR:
  6/13/2023
  Just hit up Trophy World in Mentor

  (OLD:
  Don't forget to re-order champion nameplate!
  www.trophypartner.com
  #NP100, Silver, 2" x 0.75"
  (Or take trophy to local shop & ask to reproduce cheaper)
  Check font)

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


