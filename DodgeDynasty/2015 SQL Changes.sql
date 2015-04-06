/* 4/4/2015 */

ALTER TABLE [dbo].[League]
ADD [AddTimestamp] datetime NULL
GO

UPDATE [dbo].[League]
SET [AddTimestamp] = [LastUpdateTimestamp]
WHERE [LastUpdateTimestamp] IS NOT NULL
GO

CREATE FUNCTION dbo.GetLeagueName (@LeagueId INT)
RETURNS VARCHAR(50) 
AS BEGIN
    DECLARE @LeagueName VARCHAR(50)

    SELECT @LeagueName = LeagueName FROM dbo.League WHERE LeagueId = @LeagueId

    RETURN @LeagueName
END
GO

ALTER TABLE dbo.Draft
ADD LeagueName AS dbo.GetLeagueName(LeagueId)
GO