
/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */







/* Below run in Production on 8/2/26 */

SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300 Standard',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/26/NFL26_CS_Non300.pdf?adddata=2026CS_Non300',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo! (Half-PPR)',
	ImportUrl = 'https://sports.yahoo.com/api/fanPro/?sport=NFL&position=ALL&filters=9%3A317%3A747%3A7604&experts=show&expert=7261&scoring=HALF&type=ST&week=0&wtype=ST&year=2026',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5


UPDATE [dbo].[AutoImport]
SET RankName = 'FantasyPros ADP',
	ImportUrl = 'http://startdrafting.com/docs/Rankings/FprosADP.html',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 4




COMMIT TRANSACTION;









/*
TODO:

8/4/25
	Chig Okonkwo
    Cameron Ward (make Cam Ward inactive)
    Deebo Samuel Sr.






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
        - 8/18/25:  Now use https://www.footballguys.com/article/2025-nfl-schedule-bye-weeks

        (- OLD: http://www.fantasypros.com/nfl/bye-weeks.php
           - No more jQuery format, just copy/paste from website to spreadsheet now)
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


