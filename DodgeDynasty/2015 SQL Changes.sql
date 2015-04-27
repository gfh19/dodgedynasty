BEGIN TRANSACTION;

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
ADD [AddTimestamp] [datetime] NULL
GO

UPDATE Draft
SET AddTimestamp = LastUpdateTimestamp
GO

ALTER TABLE dbo.[Draft]
ALTER COLUMN [AddTimestamp] [datetime] NOT NULL
GO

/* 4/19/15 */

ALTER TABLE dbo.[Draft]
ADD [WinnerId] [int] NULL,
	[RunnerUpId] [int] NULL,
	[HasCoWinners] [bit] NULL
GO

/*** ! ADD ForeignKey Constraint to:
	LeagueOwner table for LeagueId, 
	DraftOwner table for DraftId, 
	!!!
***/


/* 4/25/15 */

ALTER TABLE dbo.[User]
ADD [IsActive] [bit] NOT NULL DEFAULT(1),
	[LastUpdateTimestamp] [datetime] NULL
GO

EXEC sp_RENAME '[User].AddDateTime', 'AddTimestamp', 'COLUMN'
GO

UPDATE dbo.[User]
SET LastUpdateTimestamp = AddTimestamp
GO

ALTER TABLE dbo.[User]
ALTER COLUMN [LastUpdateTimestamp] [datetime] NOT NULL
ALTER TABLE dbo.[User]
ALTER COLUMN [AddTimestamp] [datetime] NOT NULL
GO

ALTER TABLE dbo.[User]
ADD [NickName] varchar(10) NULL
GO

UPDATE u
SET u.NickName = o.NickName
FROM dbo.[User] u, dbo.[Owner] o
WHERE u.UserId = o.UserId
GO

DROP TABLE [dbo].[Owner]
GO

DROP TABLE [dbo].[DraftRound]
GO

DROP TABLE [dbo].[DraftOrder]
GO

DROP TABLE [dbo].[BadTeamsDraft]
GO

EXEC sp_RENAME '[DraftRank].OwnerId', 'UserId', 'COLUMN'
GO

ALTER TABLE dbo.[LeagueOwner]
DROP COLUMN [OwnerId]
GO

EXEC sp_RENAME '[DraftOwner].OwnerId', 'UserId', 'COLUMN'
GO

EXEC sp_RENAME '[DraftPick].OwnerId', 'UserId', 'COLUMN'
GO

EXEC sp_RENAME '[DraftPickHistory].OwnerId', 'UserId', 'COLUMN'
GO




COMMIT TRANSACTION;