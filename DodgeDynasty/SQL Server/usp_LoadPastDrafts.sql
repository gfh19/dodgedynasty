USE [Home]
GO
 
/****** Object:  StoredProcedure [dbo].[usp_LoadPastDrafts]    Script Date: 8/1/2015 12:06:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[usp_LoadPastDrafts]
	@DraftId int
AS
BEGIN
	SET NOCOUNT ON;
	
	SET XACT_ABORT ON
	BEGIN TRANSACTION;

	DECLARE @HistoryPlayerId INT = 0
	DECLARE @PlayerId INT;

	WHILE (1 = 1) 
	BEGIN 
		SELECT TOP 1 @HistoryPlayerId = hp.HistoryPlayerId
			FROM _HistoryPlayer hp
			WHERE hp.DraftId = @DraftId
			AND hp.HistoryPlayerId > @HistoryPlayerId
			ORDER BY HistoryPlayerId

		IF @@ROWCOUNT = 0 BREAK;

		SELECT TOP 1 @PlayerId = PlayerId FROM _HistoryPlayer WHERE HistoryPlayerId = @HistoryPlayerId;

		--If no player entry already exists, add one (IsActive = 0, IsDrafted = 1)
		IF @PlayerId IS NULL
			BEGIN
			INSERT INTO [dbo].[Player]
				   ([TruePlayerId]
				   ,[FirstName]
				   ,[LastName]
				   ,[Position]
				   ,[NFLTeam]
				   ,[DateOfBirth]
				   ,[IsActive]
				   ,[IsDrafted]
				   ,[AddTimestamp]
				   ,[LastUpdateTimestamp])
			SELECT TOP 1 hp.TruePlayerId, hp.FirstName, hp.LastName, hp.Position, hp.NFLTeam, NULL, 0, 1, hp.AddTimestamp, hp.AddTimestamp
				FROM _HistoryPlayer hp
				WHERE hp.HistoryPlayerId = @HistoryPlayerId
				ORDER BY HistoryPlayerId

			SET @PlayerId = SCOPE_IDENTITY()
		END
		ELSE --Else If Player Entry already exists, mark IsDrafted = 1!
		BEGIN
			UPDATE [dbo].[Player]
			SET IsDrafted = 1
			WHERE PlayerId = @PlayerId
		END

		UPDATE hdp
		SET hdp.PlayerId = @PlayerId
		FROM [dbo].[_HistoryDraftPick] hdp
		WHERE hdp.HistoryPlayerId = @HistoryPlayerId
		
		UPDATE hp
		SET hp.PlayerId = @PlayerId
		FROM [dbo].[_HistoryPlayer] hp
		WHERE hp.HistoryPlayerId = @HistoryPlayerId
	END

	UPDATE Player
	SET TruePlayerId = PlayerId
	WHERE TruePlayerId IS NULL

	INSERT INTO [dbo].[DraftPick]
			   ([DraftId]
			   ,[PickNum]
			   ,[RoundNum]
			   ,[UserId]
			   ,[PlayerId]
			   ,[PickStartDateTime]
			   ,[PickEndDateTime]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
	SELECT hdp.DraftId, hdp.PickNum, hdp.RoundNum, hdp.UserId, hdp.PlayerId, NULL, NULL, hdp.AddTimestamp, hdp.AddTimestamp
		FROM _HistoryDraftPick hdp WHERE DraftId = @DraftId;


	INSERT INTO [dbo].[DraftOwner]
			   ([DraftId]
			   ,[UserId]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
	SELECT DISTINCT DraftId, UserId, AddTimestamp, AddTimestamp
		FROM dbo._HistoryDraftPick
		WHERE DraftId = @DraftId


	COMMIT TRANSACTION;
END

GO
