/* Below run in Production on 6/10/17 */

SET XACT_ABORT ON
BEGIN TRANSACTION;



ALTER TABLE [dbo].[League]
ADD [NumRounds] smallint NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET NumRounds = 15
WHERE LeagueId IN (1, 2, 7)

UPDATE [dbo].[League]
SET NumRounds = 17
WHERE LeagueId IN (8)

UPDATE [dbo].[League]
SET NumRounds = 4
WHERE LeagueId IN (3, 6, 9)

UPDATE [dbo].[League]
SET NumRounds = 6
WHERE LeagueId IN (4)

UPDATE [dbo].[League]
SET NumRounds = 3
WHERE LeagueId IN (5)

UPDATE [dbo].[League]
SET NumRounds = 8
WHERE LeagueId IN (10)

ALTER TABLE [dbo].[League]
ADD [NumKeepers] smallint NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET NumKeepers = 0

UPDATE [dbo].[League]
SET NumKeepers = 6
WHERE LeagueId IN (1)

UPDATE [dbo].[League]
SET NumKeepers = 1
WHERE LeagueId IN (2)

ALTER TABLE [dbo].[League]
ADD [Format] varchar(10) NOT NULL DEFAULT('snake')
GO

UPDATE [dbo].[League]
SET [Format] = 'snake'

UPDATE [dbo].[League]
SET [Format] = 'repeat'
WHERE LeagueId IN (1)

ALTER TABLE [dbo].[League]
ADD [CombineWRTE] bit NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[League]
SET [CombineWRTE] = 0

UPDATE [dbo].[League]
SET [CombineWRTE] = 1
WHERE LeagueId IN (1, 2, 4, 8)

ALTER TABLE [dbo].[League]
ADD [PickTimeSeconds] smallint NOT NULL DEFAULT(120)
GO



ALTER TABLE [dbo].[Draft]
ADD [CombineWRTE] bit NOT NULL DEFAULT(0);
GO

UPDATE [dbo].[Draft]
SET [CombineWRTE] = 0

UPDATE [dbo].[Draft]
SET [CombineWRTE] = 1
WHERE LeagueId IN (1, 2, 4, 8)

ALTER TABLE [dbo].[Draft]
DROP COLUMN [NumOwners];
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [NumRounds] smallint NOT NULL;
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [NumKeepers] smallint NOT NULL;
GO

ALTER TABLE [dbo].[Draft]
ALTER COLUMN [Format] varchar(10) NOT NULL
GO


/* DON'T FORGET to set defaults for NumRounds, NumKeepers, Format, PickTimeSeconds, and CombineWRTE for all existing leagues !!! */


ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandWR] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideWR] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [ExpandTE] bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[PlayerRankOption]
ADD [HideTE] bit NOT NULL DEFAULT(0)
GO





COMMIT TRANSACTION;
