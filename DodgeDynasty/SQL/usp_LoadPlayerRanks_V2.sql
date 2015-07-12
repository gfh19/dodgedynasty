/****** Object:  StoredProcedure [dbo].[usp_LoadPlayerRanks_V2]    Script Date: 7/12/2015 4:17:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[usp_LoadPlayerRanks_V2]
	@FirstName varchar(25),
	@LastName varchar(25),
	@Position varchar(10),
	@NFLTeam varchar(5),
	@DateOfBirth date,
	@RankId int,
	@RankNum int,
	@PosRankNum int,
	@AuctionValue decimal,
	@DraftYear smallint
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @PlayerId int;
	DECLARE @OldPlayerId int;
	DECLARE @TruePlayerId int;
	DECLARE @SeasonId int;
	DECLARE @ScrubbedNFLTeam varchar(5);
	DECLARE @ScrubbedFirstName varchar(25);
	DECLARE @ScrubbedLastName varchar(25);
	DECLARE @ScrubbedPosition varchar(10);

	--Get current season entry (or create if not found)
	SELECT @SeasonId=SeasonId FROM Season
	WHERE SeasonYear = @DraftYear

		--Create Season if none found for year (shouldn't be needed)
		IF @SeasonId IS NULL
		BEGIN
			INSERT INTO [dbo].[Season]
			   ([SeasonYear]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
			VALUES
			   (@DraftYear,getdate(),getdate())

			SET @SeasonId = SCOPE_IDENTITY();
		END


	SELECT @ScrubbedNFLTeam = 
		CASE RTRIM(UPPER(@NFLTeam))
		WHEN 'AZ' THEN 'ARI'
		WHEN 'JAC' THEN 'JAX'
		ELSE RTRIM(@NFLTeam)
		END

	--TODO:  Replace with lookup
	SELECT @ScrubbedFirstName = 
		CASE (RTRIM(UPPER(@FirstName)) + ' ' + RTRIM(UPPER(@LastName)))
		WHEN 'CHRISTOPHER IVORY' THEN 'Chris'
		WHEN 'TIMOTHY WRIGHT' THEN 'Tim'
		ELSE @FirstName
		END

	SELECT @ScrubbedLastName = 
		CASE (RTRIM(UPPER(@FirstName)) + ' ' + RTRIM(UPPER(@LastName)))
		WHEN 'ROBERT GRIFFIN' THEN 'Griffin III'
		WHEN 'ODELL BECKHAM JR' THEN 'ODELL BECKHAM'
		WHEN 'ODELL BECKHAM JR.' THEN 'ODELL BECKHAM'
		ELSE @LastName
		END
		
	SELECT @ScrubbedPosition = 
		CASE (RTRIM(UPPER(@Position)))
		WHEN 'D/ST' THEN 'DEF'
		WHEN 'DST' THEN 'DEF'
		ELSE @Position
		END

	SELECT @PlayerId = p.PlayerId FROM dbo.Player p
	WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', ''))) 
		AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', ''))) 
		AND p.Position = RTRIM(@ScrubbedPosition) AND (UPPER(p.NFLTeam) = RTRIM(UPPER(@ScrubbedNFLTeam)) OR @ScrubbedNFLTeam = 'D/ST' OR @ScrubbedNFLTeam = '')
	
	IF @PlayerId IS NOT NULL
	BEGIN
		--If Exact Player Match Found, just set update time
		UPDATE Player
			SET IsActive = 1,
				LastUpdateTimestamp = getdate()
			WHERE PlayerId = @PlayerId
	END		
	ELSE
	BEGIN
		SELECT @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId FROM dbo.Player p
		WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', '')))
			AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', '')))
			AND p.Position = RTRIM(@ScrubbedPosition)

		--If Player on old NFL team found, add new player and port its TruePlayerId over
		IF @PlayerId IS NOT NULL
		BEGIN
			SET @OldPlayerId = @PlayerId;
			INSERT INTO [dbo].[Player]
				([TruePlayerId]
				,[FirstName]
				,[LastName]
				,[Position]
				,[NFLTeam]
				,[DateOfBirth]
				,[IsActive]
				,[AddTimestamp]
				,[LastUpdateTimestamp])
			VALUES (
				@TruePlayerId,
				RTRIM(@ScrubbedFirstName),
				RTRIM(@ScrubbedLastName),
				RTRIM(@ScrubbedPosition),
				@ScrubbedNFLTeam,
				@DateOfBirth,
				1,
				getdate(),
				getdate()
			)
			SET @PlayerId = SCOPE_IDENTITY()


			INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
			   ,[NewPlayerId]
			   ,[TruePlayerId]
			   ,[NewFirstName]
			   ,[NewLastName]
			   ,[NewPosition]
			   ,[NewNFLTeam]
			   ,[SeasonId]
			   ,[Action]
			   ,[UserId]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
			SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam, @SeasonId,
				'Add Player New Team', NULL, getdate(), getdate()
			FROM dbo.Player
			WHERE PlayerId = @PlayerId
		END
		ELSE
		BEGIN
			--Add brand new player
			INSERT INTO [dbo].[Player]
				([FirstName]
				,[LastName]
				,[Position]
				,[NFLTeam]
				,[DateOfBirth]
				,[IsActive]
				,[AddTimestamp]
				,[LastUpdateTimestamp])
			VALUES (
				RTRIM(@ScrubbedFirstName),
				RTRIM(@ScrubbedLastName),
				RTRIM(@ScrubbedPosition),
				@ScrubbedNFLTeam,
				@DateOfBirth,
				1,
				getdate(),
				getdate()
			)
			SET @PlayerId = SCOPE_IDENTITY()

			--Update player's TruePlayerId
			UPDATE [dbo].[Player]
			SET [TruePlayerId] = [PlayerId]
			WHERE [PlayerId] = @PlayerId
			

			INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
			   ,[NewPlayerId]
			   ,[TruePlayerId]
			   ,[NewFirstName]
			   ,[NewLastName]
			   ,[NewPosition]
			   ,[NewNFLTeam]
			   ,[SeasonId]
			   ,[Action]
			   ,[UserId]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
			SELECT NULL, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam, @SeasonId,
				'Add Player', NULL, getdate(), getdate()
			FROM dbo.Player
			WHERE PlayerId = @PlayerId
		END
	END

	--Add PlayerSeason entry if not found for this season/NULL season.
	IF NOT EXISTS(SELECT 1 FROM [dbo].[PlayerSeason]
					WHERE [PlayerId] = @PlayerId AND ([SeasonId] IS NULL OR [SeasonId] = @SeasonId))
	BEGIN
		INSERT INTO [dbo].[PlayerSeason]
			   ([PlayerId]
			   ,[SeasonId]
			   ,[AddTimestamp]
			   ,[LastUpdateTimestamp])
		 VALUES
			   (@PlayerId
			   ,@SeasonId
			   ,getdate()
			   ,getdate())
	END

	--Add PlayerRank entry
	INSERT INTO [dbo].[PlayerRank]
		([RankId]
		,[PlayerId]
		,[RankNum]
		,[PosRankNum]
		,[AuctionValue]
		,[AddTimestamp]
		,[LastUpdateTimestamp])
	VALUES
		(@RankId
		,@PlayerId
		,@RankNum
		,@PosRankNum
		,@AuctionValue
		,getdate(),
		getdate())

END
