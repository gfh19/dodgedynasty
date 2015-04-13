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


/* 4/6/2015 */

ALTER TABLE dbo.[User]
ADD [FullName]  AS (([FirstName]+' ')+[LastName]) PERSISTED NOT NULL
GO

ALTER TABLE dbo.[LeagueOwner]
ADD [IsActive] [bit] NOT NULL DEFAULT(1)
GO


/* 4/8/15 */

UPDATE lo
SET lo.CssClass = u.UserName
FROM dbo.LeagueOwner lo
INNER JOIN dbo.[User] u ON lo.UserId = u.UserId


/* 4/12/15 */

ALTER TABLE dbo.[Draft]
ADD [AddTimestamp] [datetime] NOT NULL
GO

UPDATE Draft
SET AddTimestamp = LastUpdateTimestamp
GO

/*** ! ADD ForeignKey Constraint to the LeagueOwner table for LeagueId!!! ***/