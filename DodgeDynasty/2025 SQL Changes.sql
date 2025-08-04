
/*TODO:  Make DraftYear in Draft table NOT NULL */
/*TODO:  Move existing PlayerHighlights to Archive and delete existing */
/*TODO:  Convert PlayerRankOptions SQL storage to Json blob (plus ?all? IDs as columns? */





/* Below run in Production on  */

SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300 (Non-PPR)',
	ImportUrl = 'https://g.espncdn.com/s/ffldraftkit/25/NFL25_CS_Non300.pdf?adddata=2025CS_Non300',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'FantasyPros (Non-PPR)',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 2


UPDATE [dbo].[AutoImport]
SET RankName = 'FantasyPros ADP',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 4


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo! (Half-PPR)',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?position=ALL&sport=NFL&year=2025&week=0&experts=show&id=1663&type=ST&scoring=HALF&filters=9%3A317%3A747&widget=ST',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5


UPDATE [dbo].[AutoImport]
SET RankName = 'FantasyPros Dynasty (PPR)',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 6


INSERT INTO [dbo].[AutoImport]
           ([RankName]
           ,[ImportUrl]
           ,[IsApi]
           ,[AddTimestamp]
           ,[LastUpdateTimestamp]
           ,[IsPdf])
     VALUES
           ('FantasyPros (PPR)'
           ,'https://www.fantasypros.com/nfl/rankings/ppr-cheatsheets.php'
           ,0
           ,getdate()
           ,getdate()
           ,0)
GO



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


