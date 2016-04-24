/****** Object:  StoredProcedure [dbo].[usp_LoadPlayerRanks_V2]    Script Date: 7/12/2015 4:17:09 PM ******/
USE [Home]

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
	DECLARE @TruePlayerId int;	--Identifies True Unique Player even if multiple table entries
	DECLARE @IsDrafted bit;
	DECLARE @ExistingActivePlayerId int;
	DECLARE @ExistingActiveTruePlayerId int;
	DECLARE @ExistingActiveIsDrafted bit;
	DECLARE @ScrubbedNFLTeam varchar(5);
	DECLARE @ScrubbedFirstName varchar(25);
	DECLARE @ScrubbedLastName varchar(25);
	DECLARE @ScrubbedPosition varchar(10);

	/*	Steps: */
	/*	- Scrubs for known NFLTeam/FirstName/LastName/Pos
		- First, check for EXACT match in ACTIVE players
			-If Exact Player Match Found, just set update time
		- Else Next, check for EXACT match from remaining inactive players
			-If found EXACT match from Inactive players, need to investigate	*
				-If found Matching Active TruePlayerId already exists, investigate	*
						- Set @PlayerId to Matching true Active player id (or new) to be used in rank
					- If Player IsDrafted, Add new player and link to TruePlayerId
						-- * Update all current year's rankings of trueplyr to New Player Id and Deactivate Old!
					- Else if player is not drafted, update team name
				- else EXACT Inactive Match & NO Active Match
					-No other active found, so update inactive player to active & set update time	
		- else no EXACT Inactive match
			-No Exact Match Found, check close match in ACTIVE players
				-If Close Player Match in Active Players, investigate *
					- If Player IsDrafted, Add new player and link to TruePlayerId
						-- * Update all current year's rankings of trueplyr to New Player Id and Deactivate Old!
					- Else if player is not drafted, update team name
		- else no Close ACTIVE match
			-No close match active player, check remaining inactive players
			-If close match inactive player found, investigate	*
				- If Player IsDrafted, Add new player and link to TruePlayerId
				- Else if player is not drafted, update team name
					- Mark inactive player Active and update team name & time
		- If here and @PlayerId still null, no matching player found at all
			- Add brand new player
			- Update brand new player's TruePlayerId
		- Before Add PlayerRank entry, *McCluster check
			- If same Player Name/NFL Team but different position is Active:
				- Update IsActive to False, TruePlayerId to ExistingActiveTruePlayerId
				- Set @PlayerId = @ExistingActivePlayerId
		-Add PlayerRank entry
		*/

	--TODO:  Replace with lookup
	SELECT @ScrubbedFirstName = 
		CASE (LTRIM(RTRIM(UPPER(REPLACE(@FirstName, '.', '')))) + ' ' + LTRIM(RTRIM(UPPER(REPLACE(@LastName, '.', '')))))
		WHEN 'CHRISTOPHER IVORY' THEN 'Chris'
		WHEN 'TIMOTHY WRIGHT' THEN 'Tim'
		WHEN 'STEVIE JOHNSON' THEN 'Steve'
		ELSE LTRIM(RTRIM(@FirstName))
		END

	SELECT @ScrubbedLastName = 
		CASE (LTRIM(RTRIM(UPPER(REPLACE(@FirstName, '.', '')))) + ' ' + LTRIM(RTRIM(UPPER(REPLACE(@LastName, '.', '')))))
		WHEN 'ROBERT GRIFFIN' THEN 'Griffin III'
		WHEN 'ODELL BECKHAM JR' THEN 'Beckham'
		WHEN 'STEVE SMITH SR' THEN 'Smith'
		ELSE LTRIM(RTRIM(@LastName))
		END

	SELECT TOP 1 @ScrubbedNFLTeam = TeamAbbr FROM NFLTeam
		WHERE (LTRIM(RTRIM(UPPER(REPLACE(LocationName, '.', '')))) = UPPER(REPLACE(@ScrubbedFirstName, '.', ''))
			   AND LTRIM(RTRIM(UPPER(REPLACE(TeamName, '.', '')))) = UPPER(REPLACE(@ScrubbedLastName, '.', '')))
		OR (LTRIM(RTRIM(UPPER(REPLACE(LocationName, '.', '')))) + ' ' + LTRIM(RTRIM(UPPER(REPLACE(TeamName, '.', ''))))
			= UPPER(REPLACE(@ScrubbedFirstName, '.', '')) + ' ' + UPPER(REPLACE(@ScrubbedLastName, '.', '')))

	--If First/Last Name = NFLTeam (location & team name), set NFLTeam and Position
	IF @ScrubbedNFLTeam IS NOT NULL
	BEGIN
		SET @ScrubbedPosition = 'DEF';
	END
	ELSE
	BEGIN
		SELECT @ScrubbedNFLTeam = 
			CASE RTRIM(UPPER(@NFLTeam))
			WHEN 'AZ' THEN 'ARI'
			WHEN 'GBP' THEN 'GB'
			WHEN 'GNB' THEN 'GB'
			WHEN 'JAC' THEN 'JAX'
			WHEN 'KAN' THEN 'KC'
			WHEN 'KCC' THEN 'KC'
			WHEN 'NEP' THEN 'NE'
			WHEN 'NWE' THEN 'NE'
			WHEN 'NOR' THEN 'NO'
			WHEN 'NOS' THEN 'NO'
			WHEN 'SDC' THEN 'SD'
			WHEN 'SDG' THEN 'SD'
			WHEN 'SFO' THEN 'SF'
			WHEN 'TAM' THEN 'TB'
			WHEN 'TBB' THEN 'TB'
			WHEN 'WSH' THEN 'WAS'
			WHEN 'D/ST' THEN ''
			ELSE LTRIM(RTRIM(UPPER(@NFLTeam)))
			END
		
		SELECT @ScrubbedPosition = 
			CASE (RTRIM(UPPER(@Position)))
			WHEN 'D/ST' THEN 'DEF'
			WHEN 'DST' THEN 'DEF'
			ELSE LTRIM(RTRIM(UPPER(@Position)))
			END
	END

	--First, check for EXACT match in ACTIVE players
	SELECT @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId, @IsDrafted=IsDrafted FROM dbo.Player p
	WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', ''))) 
		AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', ''))) 
		AND p.Position = RTRIM(@ScrubbedPosition) AND (UPPER(p.NFLTeam) = RTRIM(UPPER(@ScrubbedNFLTeam)) OR @ScrubbedNFLTeam = '')
		AND p.IsActive = 1
	
	IF @PlayerId IS NOT NULL
	BEGIN
		--If Exact Player Match Found, just set update time
		UPDATE Player
		SET IsActive = 1,
			LastUpdateTimestamp = getdate()
		WHERE PlayerId = @PlayerId
	END		
	ELSE --else no EXACT ACTIVE match
	BEGIN
		--Else Next, check for EXACT match from remaining inactive players
		SELECT TOP 1 @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId, @IsDrafted=IsDrafted FROM dbo.Player p
		WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', ''))) 
			AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', ''))) 
			AND p.Position = RTRIM(@ScrubbedPosition) AND (UPPER(p.NFLTeam) = RTRIM(UPPER(@ScrubbedNFLTeam)) OR @ScrubbedNFLTeam = '')
			ORDER BY LastUpdateTimestamp DESC
			
		--If found EXACT match from Inactive players, need to investigate
		IF @PlayerId IS NOT NULL
		BEGIN
			SELECT @ExistingActivePlayerId = p.PlayerId, 
				@ExistingActiveTruePlayerId=p.TruePlayerId, @ExistingActiveIsDrafted=IsDrafted FROM dbo.Player p
			WHERE p.TruePlayerId = @TruePlayerId AND p.IsActive = 1
			
			--If found Matching Active TruePlayerId already exists; investigate
			IF @ExistingActivePlayerId IS NOT NULL
			BEGIN
				SET @OldPlayerId = @PlayerId
				--Set @PlayerId to Matching True Active player id (to be used in ranking)
				SET @PlayerId = @ExistingActivePlayerId

				IF @ExistingActiveIsDrafted = 1
				BEGIN
					-- If Player IsDrafted, Add new player and link to TruePlayerId
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

					--Update player's TruePlayerId to existing active TruePlayerId
					UPDATE [dbo].[Player]
					SET [TruePlayerId] = @ExistingActiveTruePlayerId
					WHERE [PlayerId] = @PlayerId
					
					-- Deactivate old Active drafted player in favor of new team Newly Added Player!
					UPDATE Player
					SET IsActive = 0
					WHERE PlayerId = @ExistingActivePlayerId

					INSERT INTO dbo.PlayerAdjustment
						([OldPlayerId],[NewPlayerId],[TruePlayerId],[NewFirstName],[NewLastName],[NewPosition],[NewNFLTeam],[Action],[UserId],[AddTimestamp],[LastUpdateTimestamp])
						SELECT @ExistingActivePlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
							'Deactivate Drafted Player - New Team', NULL, getdate(), getdate()
						FROM dbo.Player
						WHERE PlayerId = @ExistingActivePlayerId

					DECLARE @AddPlayerMatchBoth VARCHAR(50) = 'Add Player, Match Both, IsDrafted';

					IF EXISTS (SELECT TOP 1 * FROM [dbo].[PlayerRank] pr
								JOIN [dbo].[Rank] r ON pr.RankId = r.RankId
								WHERE pr.PlayerId = @ExistingActivePlayerId
									AND r.[Year] = @DraftYear)
					BEGIN
						-- * Update all current year's rankings of trueplyr to New Player Id and Deactivate Old!
						UPDATE pr
						SET pr.[PlayerId] = @PlayerId
						FROM [dbo].[PlayerRank] pr
						JOIN [dbo].[Rank] r ON pr.RankId = r.RankId
						WHERE pr.PlayerId = @ExistingActivePlayerId
							AND r.[Year] = @DraftYear

						PRINT 'Updated Rankings Rowcount: ' + CAST(@@ROWCOUNT AS VARCHAR);

						SET @AddPlayerMatchBoth = @AddPlayerMatchBoth + ' (& Ranks)';
					END

					INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
						,[NewPlayerId]
						,[TruePlayerId]
						,[NewFirstName]
						,[NewLastName]
						,[NewPosition]
						,[NewNFLTeam]
						,[Action]
						,[UserId]
						,[AddTimestamp]
						,[LastUpdateTimestamp])
					SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
						@AddPlayerMatchBoth, NULL, getdate(), getdate()
					FROM dbo.Player
					WHERE PlayerId = @PlayerId
				END
				ELSE
				BEGIN
					-- Else if player is not drafted, update team name
					INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
					   ,[NewPlayerId]
						,[TruePlayerId]
					   ,[NewFirstName]
					   ,[NewLastName]
					   ,[NewPosition]
					   ,[NewNFLTeam]
					   ,[Action]
					   ,[UserId]
					   ,[AddTimestamp]
					   ,[LastUpdateTimestamp])
					SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam, 
						'Update NFL Team, Active Not Drafted', NULL, getdate(), getdate()
					FROM dbo.Player
					WHERE PlayerId = @PlayerId
				
					--Update active player's team name & time
					UPDATE Player
					SET NFLTeam = @ScrubbedNFLTeam,
						IsActive = 1,
						LastUpdateTimestamp = getdate()
					WHERE PlayerId = @PlayerId
				END
			END
			ELSE --else EXACT Inactive Match & NO Active Match
			BEGIN
				--No other active found, so update inactive player to active & set update time			
				UPDATE Player
				SET IsActive = 1,
					LastUpdateTimestamp = getdate()
				WHERE PlayerId = @PlayerId
			END
		END
	END

	--else no EXACT Inactive match
	IF @PlayerId IS NULL  
	BEGIN
		--No Exact Match Found, check close match in ACTIVE players
		SELECT TOP 1 @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId, @IsDrafted=IsDrafted FROM dbo.Player p
		WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', '')))
			AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', '')))
			AND p.Position = RTRIM(@ScrubbedPosition)
			AND p.IsActive = 1
			ORDER BY LastUpdateTimestamp DESC

		IF @PlayerId IS NOT NULL
		BEGIN
			SET @OldPlayerId = @PlayerId
			--If Close Player Match in Active Players, investigate
			IF @IsDrafted = 1
			BEGIN
				-- If Player IsDrafted, Add new player and link to TruePlayerId, DEACTIVATE EXISTING!
				INSERT INTO [dbo].[Player]	--Add new player
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

				--Update player's TruePlayerId to close matching active TruePlayerId
				UPDATE [dbo].[Player]
				SET [TruePlayerId] = @TruePlayerId
				WHERE [PlayerId] = @PlayerId
								
				-- Deactivate old Active drafted player in favor of new team Newly Added Player!
				UPDATE Player
				SET IsActive = 0
				WHERE PlayerId = @OldPlayerId

				INSERT INTO dbo.PlayerAdjustment
					([OldPlayerId],[NewPlayerId],[TruePlayerId],[NewFirstName],[NewLastName],[NewPosition],[NewNFLTeam],[Action],[UserId],[AddTimestamp],[LastUpdateTimestamp])
					SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
						'Deactivate Drafted Player - New Team (2)', NULL, getdate(), getdate()
					FROM dbo.Player
					WHERE PlayerId = @OldPlayerId

				DECLARE @AddPlayerMatchActive VARCHAR(50) = 'Add Player, Match Active, IsDrafted';
				IF EXISTS (SELECT TOP 1 * FROM [dbo].[PlayerRank] pr
							JOIN [dbo].[Rank] r ON pr.RankId = r.RankId
							WHERE pr.PlayerId = @OldPlayerId
								AND r.[Year] = @DraftYear)
				BEGIN
					-- * Update all current year's rankings of trueplyr to New Player Id
					UPDATE pr
					SET pr.[PlayerId] = @PlayerId
					FROM [dbo].[PlayerRank] pr
					JOIN [dbo].[Rank] r ON pr.RankId = r.RankId
					WHERE pr.PlayerId = @OldPlayerId
						AND r.[Year] = @DraftYear

					PRINT 'Updated Rankings Rowcount: ' + CAST(@@ROWCOUNT AS VARCHAR);
					
					INSERT INTO dbo.PlayerAdjustment
						([OldPlayerId],[NewPlayerId],[TruePlayerId],[NewFirstName],[NewLastName],[NewPosition],[NewNFLTeam],[Action],[UserId],[AddTimestamp],[LastUpdateTimestamp])
						SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
							'Deactivate Drafted Player - New Team', NULL, getdate(), getdate()
						FROM dbo.Player
						WHERE PlayerId = @OldPlayerId

					SET @AddPlayerMatchActive = @AddPlayerMatchActive + ' (& Ranks)';
				END

				--And log record of Add New Active Player (& possibly Ranks update)
				INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
					,[NewPlayerId]
					,[TruePlayerId]
					,[NewFirstName]
					,[NewLastName]
					,[NewPosition]
					,[NewNFLTeam]
					,[Action]
					,[UserId]
					,[AddTimestamp]
					,[LastUpdateTimestamp])
				SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
					@AddPlayerMatchActive, NULL, getdate(), getdate()
				FROM dbo.Player
				WHERE PlayerId = @PlayerId
			END
			ELSE
			BEGIN
				-- Else if player is not drafted, update team name
				INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
					,[NewPlayerId]
					,[TruePlayerId]
					,[NewFirstName]
					,[NewLastName]
					,[NewPosition]
					,[NewNFLTeam]
					,[Action]
					,[UserId]
					,[AddTimestamp]
					,[LastUpdateTimestamp])
				SELECT @PlayerId, NULL, TruePlayerId, FirstName, LastName, Position, NFLTeam, 
					'Update NFL Team, (Re)activate', NULL, getdate(), getdate()
				FROM dbo.Player
				WHERE PlayerId = @PlayerId
				
				--Update active player's team name & time
				UPDATE Player
				SET NFLTeam = @ScrubbedNFLTeam,
					IsActive = 1,
					LastUpdateTimestamp = getdate()
				WHERE PlayerId = @PlayerId
			END
		END
	END

	-- else no Close ACTIVE match
	IF @PlayerId IS NULL  
	BEGIN
		--Else No close match active player, check remaining inactive players
		SELECT TOP 1 @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId, @IsDrafted=IsDrafted FROM dbo.Player p
		WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', '')))
			AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', '')))
			AND p.Position = RTRIM(@ScrubbedPosition)
			ORDER BY LastUpdateTimestamp DESC

		IF @PlayerId IS NOT NULL
		BEGIN
			SET @OldPlayerId = @PlayerId
			--If close match inactive player found, investigate
			IF @IsDrafted = 1
			BEGIN
				-- If Player IsDrafted, Add new player and link to TruePlayerId
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

				--Update player's TruePlayerId to close matching inactive TruePlayerId
				UPDATE [dbo].[Player]
				SET [TruePlayerId] = @TruePlayerId
				WHERE [PlayerId] = @PlayerId

				INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
					,[NewPlayerId]
					,[TruePlayerId]
					,[NewFirstName]
					,[NewLastName]
					,[NewPosition]
					,[NewNFLTeam]
					,[Action]
					,[UserId]
					,[AddTimestamp]
					,[LastUpdateTimestamp])
				SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
					'Add Player, Match Inactive, IsDrafted', NULL, getdate(), getdate()
				FROM dbo.Player
				WHERE PlayerId = @PlayerId
			END
			ELSE --Else if inactive player is not drafted
			BEGIN
				--Else if player is not drafted, update team name
				INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
					,[NewPlayerId]
					,[TruePlayerId]
					,[NewFirstName]
					,[NewLastName]
					,[NewPosition]
					,[NewNFLTeam]
					,[Action]
					,[UserId]
					,[AddTimestamp]
					,[LastUpdateTimestamp])
				SELECT @PlayerId, NULL, TruePlayerId, FirstName, LastName, Position, NFLTeam, 
					'Update NFL Team, Activate', NULL, getdate(), getdate()
				FROM dbo.Player
				WHERE PlayerId = @PlayerId

				--Mark inactive player Active and update team name & time
				UPDATE Player
				SET NFLTeam = @ScrubbedNFLTeam,
					IsActive = 1,
					LastUpdateTimestamp = getdate()
				WHERE PlayerId = @PlayerId
			END
		END
	END

	--If here and @PlayerId still null, no matching player found at all
	IF @PlayerId IS NULL
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

		--Update brand new player's TruePlayerId
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
			,[Action]
			,[UserId]
			,[AddTimestamp]
			,[LastUpdateTimestamp])
		SELECT NULL, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
			'Add Player', NULL, getdate(), getdate()
		FROM dbo.Player
		WHERE PlayerId = @PlayerId
	END
	
	--MUST Clear @ExistingActivePlayerId before trying to use it again!
	SET @ExistingActivePlayerId = NULL;

	-- Before Add PlayerRank entry, *McCluster check
		-- Position does NOT match; Different, Active player
	SELECT TOP 1 @ExistingActivePlayerId = p.PlayerId, 
		@ExistingActiveTruePlayerId=p.TruePlayerId FROM dbo.Player p
	WHERE UPPER(REPLACE(REPLACE(p.FirstName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedFirstName, '''', ''), '.', ''))) 
		AND UPPER(REPLACE(REPLACE(p.LastName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedLastName, '''', ''), '.', ''))) 
		AND p.Position <> RTRIM(@ScrubbedPosition) 
		AND (UPPER(p.NFLTeam) = RTRIM(UPPER(@ScrubbedNFLTeam)))
	AND p.IsActive = 1
	AND p.PlayerId <> @PlayerId
	ORDER BY LastUpdateTimestamp DESC

	-- If same Player Name/NFL Team but different position is Active:
	IF @ExistingActivePlayerId IS NOT NULL AND @ExistingActivePlayerId <> @PlayerId
	BEGIN
		SET @OldPlayerId = @PlayerId
		-- Update IsActive to False, TruePlayerId to ExistingActiveTruePlayerId
		UPDATE [dbo].[Player]
		SET [TruePlayerId] = @ExistingActiveTruePlayerId,
			[IsActive] = 0
		WHERE [PlayerId] = @PlayerId

		-- Set @PlayerId = @ExistingActivePlayerId to use in ranking
		SET @PlayerId = @ExistingActivePlayerId
		
		INSERT INTO dbo.PlayerAdjustment ([OldPlayerId]
			,[NewPlayerId]
			,[TruePlayerId]
			,[NewFirstName]
			,[NewLastName]
			,[NewPosition]
			,[NewNFLTeam]
			,[Action]
			,[UserId]
			,[AddTimestamp]
			,[LastUpdateTimestamp])
		SELECT @OldPlayerId, @PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam,
			'Merge Player, Active Diff Pos', NULL, getdate(), getdate()
		FROM dbo.Player
		WHERE PlayerId = @PlayerId
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
