var playerIds;
var _playerNameSubs = {
	"christopher ivory": "Chris Ivory",
	"buck allen": "Javorius Allen",
	"stevie johnson": "Steve Johnson"
}

 $(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/CurrentDraftPickPartial", ".draft-info");
 });

 function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/CurrentDraftPickPartial", ".draft-info");
}

function initSetupRank() {
	displaySavedMessage();
	displayLinks();
	bindAddPlayerLinks();
	bindRemovePlayerLinks();
	bindMoveUpPlayerLinks();
	bindMoveDownPlayerLinks();
	bindPastePlayerHandlers();
	bindSubmitRankings();
	bindAddNewPlayerLink();
	bindClearRankingsLink();
	saveCookieRankId();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function displaySavedMessage() {
	if ($(".rank-saved").is(":visible")) {
		setTimeout(function () {
			$(".rank-saved").fadeOut("slow");
		}, 5000);
	}
}

function displayLinks() {
	$(".rank-move-up").removeClass("hide-yo-husbands-too");
	$(".rank-move-down").removeClass("hide-yo-husbands-too");
	$(".rank-remove-player").removeClass("hide-yo-husbands-too");
	var firstPlayerRank = $(".rank-setup-section").find(".player-rank-entry:first");
	$(".rank-move-up", firstPlayerRank).addClass("hide-yo-husbands-too");
	$(".rank-remove-player", firstPlayerRank).addClass("hide-yo-husbands-too");
	var lastPlayerRank = $(".rank-setup-section").find(".player-rank-entry:last");
	$(".rank-move-down", lastPlayerRank).addClass("hide-yo-husbands-too");
}

function bindAddPlayerLinks() {
	var links = $(".rank-add-player");
	$.each(links, function (index, link) {
		bindAddPlayerLink(link);
	});
}

function bindAddPlayerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var playerRankEntry = $(link).closest('.player-rank-entry');
		var playerRankNum = parseInt($(".player-rank-num", playerRankEntry).text());
		var newPlayerRankEntry = copyPlayerRankEntry();
		$(newPlayerRankEntry).insertAfter(playerRankEntry);
		changeAllPlayerRankNums(playerRankNum, 1);
		setNewPlayerRankNum(newPlayerRankEntry, playerRankNum, 1);
		clearNewPlayerRankAttributes(newPlayerRankEntry);

		bindNewEntryLinks(newPlayerRankEntry);
		displayLinks();
		$("select", newPlayerRankEntry).focus();
	});
}

function bindRemovePlayerLinks() {
	var links = $(".rank-remove-player");
	$.each(links, function (index, link) {
		bindRemovePlayerLink(link);
	});
}

function bindRemovePlayerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var playerRankEntry = $(link).closest('.player-rank-entry');
		var playerRankNum = parseInt($(".player-rank-num", playerRankEntry).text());
		changeAllPlayerRankNums(playerRankNum, -1);
		$(playerRankEntry).remove();
		displayLinks();
	});
}

function bindMoveUpPlayerLinks() {
	var links = $(".rank-move-up");
	$.each(links, function (index, link) {
		bindMoveUpPlayerLink(link);
	});
}

function bindMoveUpPlayerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var playerRankEntry = $(link).closest('.player-rank-entry');
		var playerRankNum = parseInt($(".player-rank-num", playerRankEntry).text());
		var prevPlayerRankEntry = $(playerRankEntry).prev();
		var prevPlayerRankNum = parseInt($(".player-rank-num", prevPlayerRankEntry).text());
		setNewPlayerRankNum(prevPlayerRankEntry, prevPlayerRankNum, 1);
		setNewPlayerRankNum(playerRankEntry, playerRankNum, -1);
		var oldPos = $($(".player-select", playerRankEntry)).offset().top;
		$(playerRankEntry).insertBefore($(prevPlayerRankEntry));
		setElementTopPosition($(".player-select", playerRankEntry), oldPos, (-1 * $(playerRankEntry).height()));
		$(playerRankEntry).css("display", "none");
		$(playerRankEntry).fadeIn(180);
		$(playerRankEntry).focus();
		displayLinks();
	});
}

function bindMoveDownPlayerLinks() {
	var links = $(".rank-move-down");
	$.each(links, function (index, link) {
		bindMoveDownPlayerLink(link);
	});
}

function bindMoveDownPlayerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var playerRankEntry = $(link).closest('.player-rank-entry');
		var playerRankNum = parseInt($(".player-rank-num", playerRankEntry).text());
		var nextPlayerRankEntry = $(playerRankEntry).next();
		var nextPlayerRankNum = parseInt($(".player-rank-num", nextPlayerRankEntry).text());
		setNewPlayerRankNum(nextPlayerRankEntry, nextPlayerRankNum, -1);
		setNewPlayerRankNum(playerRankEntry, playerRankNum, 1);
		var oldPos = $($(".player-select", playerRankEntry)).offset().top;
		$(playerRankEntry).insertAfter($(nextPlayerRankEntry));
		setElementTopPosition($(".player-select", playerRankEntry), oldPos, $(playerRankEntry).height());
		$(playerRankEntry).css("display", "none");
		$(playerRankEntry).fadeIn(180);
		$(playerRankEntry).focus();
		displayLinks();
	});
}

function bindPastePlayerHandlers() {
	if (!isBrowserIE()) {
		$(document).on("paste", function (e) {
			pastePlayerHandler(e.originalEvent.clipboardData);
		});
	}

	if (isBrowserIE()) {
		var pasteKeysPressed = false;
		var map = {
			17: false,	//Ctrl
			86: false	//V
		};
		$(document).keydown(function (e) {
			if (e.keyCode in map) {
				map[e.keyCode] = true;
				if (map[17] && map[86]) {
					pasteKeysPressed = true;
				}
			}
		}).keyup(function (e) {
			if (e.keyCode in map) {
				map[e.keyCode] = false;
				if (pasteKeysPressed) {
					pasteKeysPressed = false;
					pastePlayerHandler(window.clipboardData);
				}
			}
		});
	}
}

function pastePlayerHandler(clipboardData) {
	$("body").css("cursor", "wait");
	var pastedText = (clipboardData) ? clipboardData.getData('Text') : null;
	if (pastedText && $(document.activeElement).is("select") && $(document.activeElement).hasClass("player-select")) {
		var pastedArray = pastedText.split("\r\n");
		if (pastedArray.length > 0 && pastedArray[pastedArray.length - 1] == "") {
			pastedArray = pastedArray.splice(0, pastedArray.length - 1);
		}
		var destSelect = document.activeElement;
		if (pastedArray.length > 0) {
			$.each(pastedArray, function (ix, txt) {
				selectPastedPlayer(txt, destSelect);
				var newDestSelect = $(".player-select", $(destSelect).parent().parent().next());
				if (ix < (pastedArray.length - 1) && ($(newDestSelect).length == 0 || $(newDestSelect).is("span"))) {
					$(".rank-add-player", $(destSelect).parent().parent()).click();
					newDestSelect = $(".player-select", $(destSelect).parent().parent().next());
				}
				destSelect = newDestSelect;
			});
		}
	}
	$("body").css("cursor", "default");
}

function selectPastedPlayer(txt, destSelect) {
	if (txt && txt.length > 0) {
		var matchedVal = null;
		var pastedVal = $("option", destSelect).filter(function () {
			if (matchedVal == null) {
				var playerNameLength = $(this).html().indexOf(" (");
				if (playerNameLength < 0) { playerNameLength = $(this).html().length; }
				var playerName = $(this).html().substr(0, playerNameLength);
				var defenseName = null;
				if (isDEF($(this).html())) {
					var teamNameIx = playerName.lastIndexOf(" ");
					if (teamNameIx > 0) {
						defenseName = playerName.substr(teamNameIx+1, playerNameLength);
					}
				}

				var pastedPlayerLength = txt.indexOf("(");
				if (pastedPlayerLength < 0) { pastedPlayerLength = txt.length; }
				var pastedPlayer = txt.substr(0, pastedPlayerLength);
				pastedPlayerLength = pastedPlayer.indexOf(",");
				if (pastedPlayerLength > 0) {
					pastedPlayer = txt.substr(0, pastedPlayerLength);
				}
				var scrubbedPlayer = scrubPlayerName(pastedPlayer);

				if (playerNameLength > 0
					&& (formatName($(this).html()).startsWith(scrubbedPlayer)
					|| scrubbedPlayer.startsWith(formatName(playerName)))
					|| (defenseName != null
						&& (formatName(defenseName).startsWith(scrubbedPlayer)
						|| scrubbedPlayer.startsWith(formatName(defenseName)+' d')))
					) {
					matchedVal = $(this).val();
					return true;
				}
			}
			return false;
		}).val();
		if (matchedVal && matchedVal >= 0) {
			(isBrowserIE()) ? $('option[value=' + matchedVal + ']', destSelect).prop('selected', true)
							: $(destSelect).val(matchedVal);
		}
		else {
			(isBrowserIE()) ? $("option:first", destSelect).prop('selected', true)
							: $(destSelect).val($("option:first", destSelect).val());
		}
		matchedVal = null;
	}
};

function isDEF(playerText) {
	return playerText.indexOf("-DEF)") > 0;
}

function scrubPlayerName(playerName) {
	var scrubbedName = formatName(playerName);
	return (_playerNameSubs[scrubbedName]) ? formatName(_playerNameSubs[scrubbedName]) : scrubbedName;
}

function copyPlayerRankEntry() {
	var copyPREntry = $('.copy-pr-entry');
	var newPlayerRankEntry = $(copyPREntry).clone();
	$(newPlayerRankEntry).addClass("player-rank-entry");
	$(newPlayerRankEntry).removeClass("copy-pr-entry");
	$(newPlayerRankEntry).removeClass("hide-yo-kids");
	return newPlayerRankEntry;
}

function setElementTopPosition(element, oldPos, heightOffset) {
	var elemTop = oldPos - (oldPos - $(window).scrollTop()) + heightOffset;
	$(window).scrollTop(elemTop);
}

function bindNewEntryLinks(entry) {
	bindAddPlayerLink($(".rank-add-player", $(entry)));
	bindRemovePlayerLink($(".rank-remove-player", $(entry)));
	bindMoveUpPlayerLink($(".rank-move-up", $(entry)));
	bindMoveDownPlayerLink($(".rank-move-down", $(entry)));
}

function changeAllPlayerRankNums(playerRankNum, offset) {
	var playerRankEntries = $(".player-rank-entry");
	$.each(playerRankEntries, function (ix, nextPlayerRankEntry) {
		var nextPlayerRankNum = parseInt($(".player-rank-num", nextPlayerRankEntry).text());
		if (nextPlayerRankNum > playerRankNum) {
			setNewPlayerRankNum(nextPlayerRankEntry, nextPlayerRankNum, offset);
		}
	});
}

function setNewPlayerRankNum(playerRankEntry, playerRankNum, offset) {
	$(".player-rank-num", playerRankEntry).text(playerRankNum + offset);
}

function clearNewPlayerRankAttributes(newPlayerRankEntry) {
	var playerSelect = $(".player-select", newPlayerRankEntry);
	playerSelect.val("");
	$("option", playerSelect).removeAttr("selected");
	$(playerSelect).prop('selectedIndex', 0);
}

function bindSubmitRankings() {
	$(".submit-rankings").click(function (e) {
		e.preventDefault();
		$(".submit-rankings").first().focus();
		resetValidations();
		var rankSetupModel = getRankSetupModel();
		if (validateRankSetupModel(rankSetupModel)) {
			updatePlayerRanks(rankSetupModel);
		}
	});
}

function getRankSetupModel() {
	var rankSetupModel = {};
	rankSetupModel.RankId = $("#setupRank").attr("data-rank-id");
	rankSetupModel.RankName = $(".personal-rank-name input").val();
	var rankedPlayers = new Array();
	playerIds = new Array();
	var ix = 0;
	$.each($(".player-rank-entry"), function (index, playerRank) {
		var player = {};
		player.PlayerRankId = 0;
		player.RankId = rankSetupModel.RankId;
		if ($("select", playerRank).length > 0) {
			player.PlayerId = $("select option:selected", playerRank).val();
		}
		else {
			player.PlayerId = $(".player-select", playerRank).attr("data-player-id");
		}

		var playerEntryText = $("select option:selected", playerRank).text();
		var firstSpaceIx = playerEntryText.indexOf(' ');
		var leftParenIx = playerEntryText.indexOf('(');
		var dashIx = playerEntryText.indexOf('-');
		var rightParenIx = playerEntryText.indexOf(')');
		player.RankNum = parseInt($(".player-rank-num", playerRank).text());
		player.PosRankNum = 0;
		player.AuctionValue = null;
		rankedPlayers.push(player);
		playerIds[ix++] = player.PlayerId;
	});

	rankSetupModel.RankedPlayers = rankedPlayers;
	return rankSetupModel;
}

function resetValidations() {
	$(".blank-player-msg, .dup-player-msg").addClass("hide-yo-wives");
	$(".player-rank-entry").removeClass("invalid-border");
}

function validateRankSetupModel(rankSetupModel) {
	var isValid = true;
	playerIds.sort();
	var blankPlayer = $.inArray("", playerIds);
	if (blankPlayer > -1) {
		$(".blank-player-msg").removeClass("hide-yo-wives");
		markInvalidPlayerId("");
		isValid = false;
	}
	for (var i = 0; i < (playerIds.length - 1); i++) {
		if (playerIds[i] != undefined && playerIds[i] == playerIds[i + 1]) {
			$(".dup-player-msg").removeClass("hide-yo-wives");
			markInvalidPlayerId(playerIds[i]);
			isValid = false;
		}
	}
	return isValid;
}

function markInvalidPlayerId(playerId) {
	if (playerId == "") {
		//Find any matching selected options
		$.each($("select.player-select"), function (index, select) {
			if ($(select).val() == "") {
				var invalidEntries = $(select).closest(".player-rank-entry");
				$(invalidEntries).addClass("invalid-border");
			}
		});
	}
	else {
		//Find any matching selected options
		var invalidEntries = $("select option:selected[value=" + playerId + "]").closest(".player-rank-entry");
		$(invalidEntries).addClass("invalid-border");
		//Find any matching spans
		invalidEntries = $(".player-select[data-player-id=" + playerId + "]").closest(".player-rank-entry");
		$(invalidEntries).addClass("invalid-border");
	}
}

function showSavingDialog() {
	$("body").css("cursor", "progress");
	$("#saveRanksDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '200px',
		modal: true
	});
}

function updatePlayerRanks(rankSetupModel) {
	showSavingDialog();
	setTimeout(function () {
		ajaxPost(rankSetupModel, "Rank/UpdateRankSetup", function (rankStatus) {
			window.location.reload();
		}, null, null, true);
	}, 0);
}

function saveCookieRankId() {
	var options = getCookieOptions();
	options.RankId = $("#setupRank").attr("data-rank-id");
	setCookieOptions(options);
}

function bindAddNewPlayerLink() {
	$(".add-new-player").click(function (e) {
		e.preventDefault();
		showAddNewPlayerDialog();
		setPlayerAutoComplete("#add-plyr-fname", "#add-plyr-lname", "#add-plyr-pos", "#add-plyr-nfl");
	});
}

function bindClearRankingsLink() {
	$(".clear-ranks").click(function (e) {
		e.preventDefault();
		showClearRanksDialog();
	});
}

function getNewPlayer() {
	var player = {};
	player.FirstName = $("#add-plyr-fname").val();
	player.LastName = $("#add-plyr-lname").val();
	player.Position = $("#add-plyr-pos").val();
	player.NFLTeam = $("#add-plyr-nfl").val();
	return player;
}

function showAddNewPlayerDialog() {
	$("#addNewPlayerDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '280px',
		modal: true,
		buttons: [
					{
						text: "Save Changes & Add Player", click: function () {
							resetValidations();
							var rankSetupModel = getRankSetupModel();
							rankSetupModel.Player = getNewPlayer();
							if (!validateRankSetupModel(rankSetupModel)) {
								$(this).dialog("close");
							}
							else if ($("#addNewPlayerForm").valid()) {
								$(this).dialog("close");
								updatePlayerRanks(rankSetupModel);
							}
						}
					},
					{
						text: "Cancel", click: function () {
							$(this).dialog("close");
						}
					},
				]
	});
}

function showClearRanksDialog() {
	$("#clearRanksDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '230px',
		modal: true,
		buttons: [
					{
						text: "OK", click: function () {
							$(".rank-add-player").last().click()
							$(".rank-remove-player").not(":last").click();
							$(this).dialog("close");
						}
					},
					{
						text: "Cancel", click: function () {
							$(this).dialog("close");
						}
					},
		]
	});
}