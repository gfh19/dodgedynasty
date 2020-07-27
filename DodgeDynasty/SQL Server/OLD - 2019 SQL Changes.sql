/* Below run in Production on 8/24/2019 */


SET XACT_ABORT ON
BEGIN TRANSACTION;


UPDATE [DB_70949_home].[dbo].[AudioApi]
SET [AudioApiKey]='853de4c40322457fb745a0a1d5ec7dd9',
  [AudioApiUrl]='http://api.voicerss.org/?key=853de4c40322457fb745a0a1d5ec7dd9&src=<<audiotext>>&hl=en-us&f=16khz_16bit_stereo&c=mp3'
WHERE [AudioApiCode]='first'


UPDATE [DB_70949_home].[dbo].[AudioApi]
SET [AudioApiKey]='3862e869b067430e9422d26904697876',
  [AudioApiUrl]='http://api.voicerss.org/?key=3862e869b067430e9422d26904697876&src=<<audiotext>>&hl=en-us&f=16khz_16bit_stereo&c=mp3'
WHERE [AudioApiCode]='second'



COMMIT TRANSACTION;







/* Below run in Production on 8/3/2019 */




SET XACT_ABORT ON
BEGIN TRANSACTION;

UPDATE [dbo].[AutoImport]
SET RankName = 'ESPN Top 300',
	ImportUrl = 'https://www.espn.com/fantasy/football/story/_/id/26692058/fantasy-football-updated-2019-non-ppr-rankings-mike-clay',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 1


UPDATE [dbo].[AutoImport]
SET RankName = 'Yahoo!',
	ImportUrl = 'https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2019&week=0&id=1663&position=ALL&type=ST&scoring=STD&filters=7:8:9:285:699',
	LastUpdateTimestamp = getdate()
WHERE AutoImportId = 5



COMMIT TRANSACTION;




/*
  NEXT YEAR:
  Don't forget to re-order champion nameplate!
  www.trophypartner.com
  #NP100, Silver, 2" x 0.75"
  (Or take trophy to local shop & ask to reproduce cheaper)
  Check font

  - Deactivate All But 32 DEFs
  - BYE WEEKS!
  - Also, when AutoImporting ranks, start with a Fantasypros one for better player names
*/


