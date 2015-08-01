USE [Home]
GO

/****** Object:  StoredProcedure [dbo].[usp_LoadPastDraftRosters]    Script Date: 7/26/2015 9:34:57 PM ******/
SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[usp_LoadPastDraftRosters]
	@PlayerName varchar(50),
	@Position varchar(10),
	@NFLTeam varchar(50),
	@Owner varchar(10),
	@PickNum int,
	@RoundNum int,
	@LeagueId int,
	@DraftYear int
AS
BEGIN
	SET NOCOUNT ON;
	
	SET XACT_ABORT ON
	BEGIN TRANSACTION;

	DECLARE @ScrubbedNFLTeam varchar(5);
	DECLARE @ScrubbedPlayerName varchar(50);
	DECLARE @ScrubbedPosition varchar(10);
	DECLARE @ScrubbedOwner varchar(10);
	DECLARE @DraftId int;
	DECLARE @UserId int;
	DECLARE @HistoryPlayerId int;
	DECLARE @PlayerId int;
	DECLARE @TruePlayerId int;
	DECLARE @AddDateTime datetime = DATEFROMPARTS(@DraftYear, '7', '31');

	SELECT @ScrubbedNFLTeam = 
		CASE RTRIM(UPPER(REPLACE(@NFLTeam, '.', '')))
		WHEN 'AZ' THEN 'ARI'
		WHEN 'JAC' THEN 'JAX'
		WHEN 'WSH' THEN 'WAS'
		WHEN 'D/ST' THEN ''

		WHEN 'ARIZONA CARDINALS' THEN 'ARI'
		WHEN 'ATLANTA FALCONS' THEN 'ATL'
		WHEN 'BALTIMORE RAVENS' THEN 'BAL'
		WHEN 'BUFFALO BILLS' THEN 'BUF'
		WHEN 'CAROLINA PANTHERS' THEN 'CAR'
		WHEN 'CHICAGO BEARS' THEN 'CHI'
		WHEN 'CINCINNATI BENGALS' THEN 'CIN'
		WHEN 'CLEVELAND BROWNS' THEN 'CLE'
		WHEN 'DALLAS COWBOYS' THEN 'DAL'
		WHEN 'DENVER BRONCOS' THEN 'DEN'
		WHEN 'DETROIT LIONS' THEN 'DET'
		WHEN 'GREEN BAY PACKERS' THEN 'GB'
		WHEN 'HOUSTON TEXANS' THEN 'HOU'
		WHEN 'INDIANAPOLIS COLTS' THEN 'IND'
		WHEN 'JACKSONVILLE JAGUARS' THEN 'JAX'
		WHEN 'KANSAS CITY CHIEFS' THEN 'KC'
		WHEN 'MIAMI DOLPHINS' THEN 'MIA'
		WHEN 'MINNESOTA VIKINGS' THEN 'MIN'
		WHEN 'NEW ENGLAND PATRIOTS' THEN 'NE'
		WHEN 'NEW ORLEANS SAINTS' THEN 'NO'
		WHEN 'NEW YORK GIANTS' THEN 'NYG'
		WHEN 'NEW YORK JETS' THEN 'NYJ'
		WHEN 'OAKLAND RAIDERS' THEN 'OAK'
		WHEN 'PHILADELPHIA EAGLES' THEN 'PHI'
		WHEN 'PITTSBURGH STEELERS' THEN 'PIT'
		WHEN 'SAN DIEGO CHARGERS' THEN 'SD'
		WHEN 'SEATTLE SEAHAWKS' THEN 'SEA'
		WHEN 'SAN FRANCISCO 49ERS' THEN 'SF'
		WHEN 'ST LOUIS RAMS' THEN 'STL'
		WHEN 'TAMPA BAY BUCCANEERS' THEN 'TB'
		WHEN 'TENNESSEE TITANS' THEN 'TEN'
		WHEN 'WASHINGTON REDSKINS' THEN 'WAS'

		ELSE RTRIM(@NFLTeam)
		END

	--TODO:  Replace with lookup
	SELECT @ScrubbedPlayerName = 
		CASE (RTRIM(UPPER(REPLACE(@PlayerName, '.', ''))))
		WHEN 'CARDINALS D/ST' THEN 'Arizona Cardinals'
		WHEN 'FALCONS D/ST' THEN 'Atlanta Falcons'
		WHEN 'RAVENS D/ST' THEN 'Baltimore Ravens'
		WHEN 'BILLS D/ST' THEN 'Buffalo Bills'
		WHEN 'PANTHERS D/ST' THEN 'Carolina Panthers'
		WHEN 'BEARS D/ST' THEN 'Chicago Bears'
		WHEN 'BENGALS D/ST' THEN 'Cincinnati Bengals'
		WHEN 'BROWNS D/ST' THEN 'Cleveland Browns'
		WHEN 'COWBOYS D/ST' THEN 'Dallas Cowboys'
		WHEN 'BRONCOS D/ST' THEN 'Denver Broncos'
		WHEN 'LIONS D/ST' THEN 'Detroit Lions'
		WHEN 'PACKERS D/ST' THEN 'Green Bay Packers'
		WHEN 'TEXANS D/ST' THEN 'Houston Texans'
		WHEN 'COLTS D/ST' THEN 'Indianapolis Colts'
		WHEN 'JAGUARS D/ST' THEN 'Jacksonville Jaguars'
		WHEN 'CHIEFS D/ST' THEN 'Kansas City Chiefs'
		WHEN 'DOLPHINS D/ST' THEN 'Miami Dolphins'
		WHEN 'VIKINGS D/ST' THEN 'Minnesota Vikings'
		WHEN 'PATRIOTS D/ST' THEN 'New England Patriots'
		WHEN 'SAINTS D/ST' THEN 'New Orleans Saints'
		WHEN 'GIANTS D/ST' THEN 'New York Giants'
		WHEN 'JETS D/ST' THEN 'New York Jets'
		WHEN 'RAIDERS D/ST' THEN 'Oakland Raiders'
		WHEN 'EAGLES D/ST' THEN 'Philadelphia Eagles'
		WHEN 'STEELERS D/ST' THEN 'Pittsburgh Steelers'
		WHEN 'CHARGERS D/ST' THEN 'San Diego Chargers'
		WHEN 'SEAHAWKS D/ST' THEN 'Seattle Seahawks'
		WHEN '49ERS D/ST' THEN 'San Francisco 49ers'
		WHEN 'RAMS D/ST' THEN 'St. Louis Rams'
		WHEN 'BUCCANEERS D/ST' THEN 'Tampa Bay Buccaneers'
		WHEN 'TITANS D/ST' THEN 'Tennessee Titans'
		WHEN 'REDSKINS D/ST' THEN 'Washington Redskins'
		
		WHEN 'CARDINALS' THEN 'Arizona Cardinals'
		WHEN 'FALCONS' THEN 'Atlanta Falcons'
		WHEN 'RAVENS' THEN 'Baltimore Ravens'
		WHEN 'BILLS' THEN 'Buffalo Bills'
		WHEN 'PANTHERS' THEN 'Carolina Panthers'
		WHEN 'BEARS' THEN 'Chicago Bears'
		WHEN 'BENGALS' THEN 'Cincinnati Bengals'
		WHEN 'BROWNS' THEN 'Cleveland Browns'
		WHEN 'COWBOYS' THEN 'Dallas Cowboys'
		WHEN 'BRONCOS' THEN 'Denver Broncos'
		WHEN 'LIONS' THEN 'Detroit Lions'
		WHEN 'PACKERS' THEN 'Green Bay Packers'
		WHEN 'TEXANS' THEN 'Houston Texans'
		WHEN 'COLTS' THEN 'Indianapolis Colts'
		WHEN 'JAGUARS' THEN 'Jacksonville Jaguars'
		WHEN 'CHIEFS' THEN 'Kansas City Chiefs'
		WHEN 'DOLPHINS' THEN 'Miami Dolphins'
		WHEN 'VIKINGS' THEN 'Minnesota Vikings'
		WHEN 'PATRIOTS' THEN 'New England Patriots'
		WHEN 'SAINTS' THEN 'New Orleans Saints'
		WHEN 'GIANTS' THEN 'New York Giants'
		WHEN 'JETS' THEN 'New York Jets'
		WHEN 'RAIDERS' THEN 'Oakland Raiders'
		WHEN 'EAGLES' THEN 'Philadelphia Eagles'
		WHEN 'STEELERS' THEN 'Pittsburgh Steelers'
		WHEN 'CHARGERS' THEN 'San Diego Chargers'
		WHEN 'SEAHAWKS' THEN 'Seattle Seahawks'
		WHEN '49ERS' THEN 'San Francisco 49ers'
		WHEN 'RAMS' THEN 'St. Louis Rams'
		WHEN 'BUCCANEERS' THEN 'Tampa Bay Buccaneers'
		WHEN 'TITANS' THEN 'Tennessee Titans'
		WHEN 'REDSKINS' THEN 'Washington Redskins'
		
		WHEN 'ARIZONA CARDINALS' THEN 'Arizona Cardinals'
		WHEN 'ATLANTA FALCONS' THEN 'Atlanta Falcons'
		WHEN 'BALTIMORE RAVENS' THEN 'Baltimore Ravens'
		WHEN 'BUFFALO BILLS' THEN 'Buffalo Bills'
		WHEN 'CAROLINA PANTHERS' THEN 'Carolina Panthers'
		WHEN 'CHICAGO BEARS' THEN 'Chicago Bears'
		WHEN 'CINCINNATI BENGALS' THEN 'Cincinnati Bengals'
		WHEN 'CLEVELAND BROWNS' THEN 'Cleveland Browns'
		WHEN 'DALLAS COWBOYS' THEN 'Dallas Cowboys'
		WHEN 'DENVER BRONCOS' THEN 'Denver Broncos'
		WHEN 'DETROIT LIONS' THEN 'Detroit Lions'
		WHEN 'GREEN BAY PACKERS' THEN 'Green Bay Packers'
		WHEN 'HOUSTON TEXANS' THEN 'Houston Texans'
		WHEN 'INDIANAPOLIS COLTS' THEN 'Indianapolis Colts'
		WHEN 'JACKSONVILLE JAGUARS' THEN 'Jacksonville Jaguars'
		WHEN 'KANSAS CITY CHIEFS' THEN 'Kansas City Chiefs'
		WHEN 'MIAMI DOLPHINS' THEN 'Miami Dolphins'
		WHEN 'MINNESOTA VIKINGS' THEN 'Minnesota Vikings'
		WHEN 'NEW ENGLAND PATRIOTS' THEN 'New England Patriots'
		WHEN 'NEW ORLEANS SAINTS' THEN 'New Orleans Saints'
		WHEN 'NEW YORK GIANTS' THEN 'New York Giants'
		WHEN 'NEW YORK JETS' THEN 'New York Jets'
		WHEN 'OAKLAND RAIDERS' THEN 'Oakland Raiders'
		WHEN 'PHILADELPHIA EAGLES' THEN 'Philadelphia Eagles'
		WHEN 'PITTSBURGH STEELERS' THEN 'Pittsburgh Steelers'
		WHEN 'SAN DIEGO CHARGERS' THEN 'San Diego Chargers'
		WHEN 'SEATTLE SEAHAWKS' THEN 'Seattle Seahawks'
		WHEN 'SAN FRANCISCO 49ERS' THEN 'San Francisco 49ers'
		WHEN 'ST LOUIS RAMS' THEN 'St. Louis Rams'
		WHEN 'TAMPA BAY BUCCANEERS' THEN 'Tampa Bay Buccaneers'
		WHEN 'TENNESSEE TITANS' THEN 'Tennessee Titans'
		WHEN 'WASHINGTON REDSKINS' THEN 'Washington Redskins'

		WHEN 'CHRISTOPHER IVORY' THEN 'Chris Ivory'
		WHEN 'TIMOTHY WRIGHT' THEN 'Tim Wright'

		WHEN 'ROBERT GRIFFIN' THEN 'Robert Griffin III'
		WHEN 'ODELL BECKHAM JR' THEN 'Odell Beckham'

		WHEN 'CHRIS WELLS' THEN 'Beanie Wells'
		WHEN 'CHAD JOHNSON' THEN 'Chad Ochocinco'
		WHEN 'PIERRE GARÇON' THEN 'Pierre Garcon'
		WHEN 'ROY E WILLIAMS' THEN 'Roy Williams'
		WHEN UPPER('Ted Ginn Jr') THEN 'Ted Ginn'

		ELSE LTRIM(RTRIM(@PlayerName))
		END
		
	SELECT @ScrubbedPosition = 
		CASE (RTRIM(UPPER(@Position)))
		WHEN 'D/ST' THEN 'DEF'
		WHEN 'DST' THEN 'DEF'
		ELSE @Position
		END
		
	SELECT @ScrubbedOwner = 
		CASE (RTRIM(UPPER(@Owner)))
		WHEN 'HAWKE' THEN 'George'
		WHEN 'JEFF' THEN 'Pohlmann'
		WHEN 'JOEY' THEN 'Joe'
		WHEN 'MEAT' THEN 'Meat'
		ELSE @Owner
		END

	SELECT @DraftId = DraftId FROM Draft
		WHERE LeagueId = @LeagueId AND DraftYear = @DraftYear

	SELECT @UserId = UserId FROM dbo.[User]
		WHERE NickName = @ScrubbedOwner

	DECLARE @Message varchar(50);
	IF (@UserId IS NULL)
	BEGIN
		SET @Message = 'Invalid Owner: ' + @ScrubbedOwner;
		RAISERROR(@Message, 1, 1)
		ROLLBACK TRANSACTION;
	END
	
	SELECT TOP 1 * FROM NFLTeam WHERE TeamAbbr = @ScrubbedNFLTeam

	IF (@@ROWCOUNT = 0)
	BEGIN
		SET @Message = 'Invalid NFL Team: ' + @ScrubbedNFLTeam;
		RAISERROR(@Message, 1, 1)
		ROLLBACK TRANSACTION;
	END


	SELECT TOP 1 @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId FROM dbo.Player p
	WHERE UPPER(REPLACE(REPLACE(p.PlayerName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedPlayerName, '''', ''), '.', ''))) 
		AND p.Position = RTRIM(@ScrubbedPosition) AND (UPPER(p.NFLTeam) = RTRIM(UPPER(@ScrubbedNFLTeam)) OR @ScrubbedNFLTeam = '')

	--Either add exact match, close match (diff NFL team), or new player
	IF @PlayerId IS NOT NULL
	BEGIN
		INSERT INTO [dbo].[_HistoryPlayer]
				   ([DraftId]
				   ,[Year]
				   ,[PlayerId]
				   ,[TruePlayerId]
				   ,[FirstName]
				   ,[LastName]
				   ,[Position]
				   ,[NFLTeam]
				   ,[Action]
				   ,[AddTimestamp])
			 SELECT @DraftId, @DraftYear, PlayerId, TruePlayerId, FirstName, LastName, Position, NFLTeam, '01. Exact Match Found', @AddDateTime
				FROM Player WHERE PlayerId = @PlayerId

		SET @HistoryPlayerId = SCOPE_IDENTITY()

	END
	ELSE
	--Check close match
	BEGIN
		SELECT TOP 1 @PlayerId = p.PlayerId, @TruePlayerId=TruePlayerId FROM dbo.Player p
		WHERE UPPER(REPLACE(REPLACE(p.PlayerName, '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedPlayerName, '''', ''), '.', ''))) 
			AND p.Position = RTRIM(@ScrubbedPosition)

		IF @PlayerId IS NOT NULL
		BEGIN
			DECLARE @PrevNFLTeam varchar(5);
			
			SELECT TOP 1 @PrevNFLTeam=NFLTeam FROM dbo._HistoryPlayer p
			WHERE UPPER(REPLACE(REPLACE(((p.[FirstName]+' ')+p.[LastName]), '''', ''), '.', '')) = RTRIM(UPPER(REPLACE(REPLACE(@ScrubbedPlayerName, '''', ''), '.', ''))) 
				AND p.Position = RTRIM(@ScrubbedPosition) AND p.[Year] = @DraftYear AND @ScrubbedNFLTeam <> @PrevNFLTeam

			IF @PrevNFLTeam IS NOT NULL
			BEGIN
				SET @Message = 'Error: Two Teams in SAME Year: ' + @ScrubbedPlayerName + ' - ' + @ScrubbedNFLTeam + ' & ' + @PrevNFLTeam;
				RAISERROR(@Message, 1, 1)
				ROLLBACK TRANSACTION;
			END

			INSERT INTO [dbo].[_HistoryPlayer]
					   ([DraftId]
					   ,[Year]
					   ,[PlayerId]
					   ,[TruePlayerId]
					   ,[FirstName]
					   ,[LastName]
					   ,[Position]
					   ,[NFLTeam]
					   ,[Action]
					   ,[AddTimestamp])
				 SELECT @DraftId, @DraftYear, NULL, TruePlayerId, FirstName, LastName, Position, @ScrubbedNFLTeam, '02. Close Match Found, Diff Team', @AddDateTime
					FROM Player WHERE PlayerId = @PlayerId

			SET @HistoryPlayerId = SCOPE_IDENTITY()

			SET @PlayerId = NULL;
		END
		ELSE
		--No match found, add new player
		BEGIN
			DECLARE @FirstName varchar(25);
			DECLARE @LastName varchar(25);

			SET @FirstName = LTRIM(RTRIM(SUBSTRING(@ScrubbedPlayerName, 1, CASE CHARINDEX(' ', @ScrubbedPlayerName) WHEN 0 THEN LEN(@ScrubbedPlayerName) ELSE CHARINDEX(' ', @ScrubbedPlayerName)-1 END)));
			SET @LastName = LTRIM(RTRIM(SUBSTRING(@ScrubbedPlayerName, CASE CHARINDEX(' ', @ScrubbedPlayerName) WHEN 0 THEN LEN(@ScrubbedPlayerName)+1 ELSE CHARINDEX(' ', @ScrubbedPlayerName)+1 END, 50)));

			INSERT INTO [dbo].[_HistoryPlayer]
					   ([DraftId]
					   ,[Year]
					   ,[PlayerId]
					   ,[TruePlayerId]
					   ,[FirstName]
					   ,[LastName]
					   ,[Position]
					   ,[NFLTeam]
					   ,[Action]
					   ,[AddTimestamp])
				 VALUES (@DraftId, @DraftYear, NULL, NULL, @FirstName, @LastName, @ScrubbedPosition, @ScrubbedNFLTeam, '03. No Match Found, New Player', @AddDateTime)

			SET @HistoryPlayerId = SCOPE_IDENTITY()

		END
	END --End of HistoryPlayer add

	INSERT INTO [dbo].[_HistoryDraftPick]
			   ([DraftId]
			   ,[PickNum]
			   ,[RoundNum]
			   ,[UserId]
			   ,[HistoryPlayerId]
			   ,[PlayerId]
			   ,[TruePlayerId]
			   ,[AddTimestamp])
		 VALUES
			   (@DraftId, @PickNum, @RoundNum, @UserId, @HistoryPlayerId, @PlayerId, @TruePlayerId, @AddDateTime);


	COMMIT TRANSACTION;
END

GO
