var clientCookieOptions = null;
var playerIds;
var _playerNameIds = {
	"derek carrier": 1063,
}
var _playerNameSubs = {
	"buck allen": "Javorius Allen",
	"christopher ivory": "Chris Ivory",
	"philly brown": "Corey Brown",
	"stevie johnson": "Steve Johnson"
}
var toggleUnrankedTimeout = null;
var changeUnrankedListTimeout = null;

 $(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/CurrentDraftPickPartial", ".draft-info");
 });

function pageBroadcastDraftHandler() {
	refreshCurrentDraftPickPartial();
}

 function initSetupRank() {
 	displayBestUnrankedPlayers();
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
	bindPlayerSelectChange($("select.player-select"));
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
	var lastPlayerRank = $(".rank-setup-section").find(".player-rank-entry:last");
	$(".rank-move-down", lastPlayerRank).addClass("hide-yo-husbands-too");
}

/* Best Unranked Players */

function displayBestUnrankedPlayers() {
	$(".rank-avail-section").removeClass("hide-yo-husbands-too");
	toggleUnrankedPlayers();
	bindAddUnrankedPlayers();
	bindAddAllUnrankedPlayers();
	bindExpandUnrankedLink();
	bindHideUnrankedList();
	bindChangeUnrankedList();
}

function toggleUnrankedPlayers() {
	//var rankedPlayerIds = $(".bup-player-row").not(".unranked").map(function () { return $(this).attr("data-player-id") });
	//var unrankedPlayerIds = $(".bup-player-row.unranked").map(function () { return $(this).attr("data-player-id") });

	var rankedPlayerIds = $(".player-select").map(function () {
		if ($(this).is(":visible")) {
			return getPlayerSelectId($(this));
		}
	});
	$.each($(".bup-player-row"), function (ix, elem) {
		var bupPlayerId = $(elem).attr("data-player-id");
		var isRanked = ($.inArray(bupPlayerId, rankedPlayerIds) >= 0);
		if (isRanked && $(elem).hasClass("unranked")) {
			toggleUnrankedPlayerRow(elem, false);
		}
		else if (!isRanked && !$(elem).hasClass("unranked")){
			toggleUnrankedPlayerRow(elem, true);
		}
	});

	toggleUnrankedTableEmpty();
	toggleHideUnrankedList(clientCookieOptions.HideBUP);
	toggleExpandUnrankedRows(clientCookieOptions.ExpandBUP);
	toggleUnrankedTimeout = null;
}

function toggleUnrankedPlayer(playerId, show) {
	if (playerId) {
		var unrankedPlayerRow = $(".bup-player-row[data-player-id=" + playerId + "]");
		if (unrankedPlayerRow) {
			toggleUnrankedPlayerRow(unrankedPlayerRow, show);
		}
	}
}

function toggleUnrankedPlayerRow(unrankedPlayerRow, show) {
	toggleDisplay($(unrankedPlayerRow), show);
	if (show) {
		$(unrankedPlayerRow).addClass("unranked");
	}
	else {
		$(unrankedPlayerRow).removeClass("unranked");
	}
}

function toggleUnrankedTableEmpty() {
	if ($(".bup-player-row.unranked").length == 0) {
		$(".bup-full").addClass("hide-yo-wives");
		$(".bup-empty").removeClass("hide-yo-wives");
	}
	else {
		$(".bup-full").removeClass("hide-yo-wives");
		$(".bup-empty").addClass("hide-yo-wives");
	}
}

function bindAddUnrankedPlayers() {
	$.each($(".bup-player-add"), function (ix, link) {
		$(link).unbind("click");
		$(link).click(function (e) {
			e.preventDefault();
			var tempScrollTop = $(window).scrollTop();
			var lastAddPlayer = $(".rank-add-player").last();
			$(lastAddPlayer).click();
			var newPlayer = $(lastAddPlayer).parents(".player-rank-entry").next();
			var unrankedPlayerRow = $(link).parents(".bup-player-row");
			$(".player-select", newPlayer).val($(unrankedPlayerRow).attr("data-player-id"));
			toggleUnrankedPlayerRow(unrankedPlayerRow, false);
			toggleExpandUnrankedRows(toBool($(".bup-expand-link").attr("data-expand")));
			toggleUnrankedTableEmpty();
			$(window).scrollTop(tempScrollTop);
		});
	});
}

function bindAddAllUnrankedPlayers() {
	$("#bup-add-all").unbind("click");
	$("#bup-add-all").click(function (e) {
		addWaitCursor();
		showLoadingDialog();
		var unrankedAddLinks = $(".unranked .bup-player-add");
		setTimeout(function () {
			var tempScrollTop = $(window).scrollTop();
			$.each($(unrankedAddLinks), function (ix, link) {
				$(link).click();
				if (ix == unrankedAddLinks.length - 1) {
					removeWaitCursor();
					closeAllDialogs();
				}
			});
			toggleUnrankedTableEmpty();
			$(window).scrollTop(tempScrollTop);
		}, 1);
	});
}

function toggleExpandUnrankedRows(expandRows) {
	var table = $(".bup-table");
	$("tr.unranked", table).removeClass("hide-yo-wives");
	if (expandRows) {
		$("#ExpandBUP").text("Less...");
	}
	else {
		$("tr.unranked:gt(" + (ranksWindow-1) + ")", table).addClass("hide-yo-wives");
		$("#ExpandBUP").text("More...");
	}
}

function bindExpandUnrankedLink() {
	var link = $(".bup-expand-link");
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var table = $(".bup-table");

		var newExpandVal = !toBool($(link).attr("data-expand"))
		$(link).attr("data-expand", newExpandVal);
		toggleExpandUnrankedRows(newExpandVal);
		clientCookieOptions.ExpandBUP = newExpandVal;
		updateBestUnrankedOptions();
	});
}

function bindHideUnrankedList() {
	$(".bup-hide-link").unbind("click");
	$(".bup-hide-link").click(function (e) {
		var hideTable = !toBool($(".bup-hide-link").attr("data-hide-val"));
		toggleHideUnrankedList(hideTable);
		$(".bup-hide-link").attr("data-hide-val", hideTable);
		clientCookieOptions.HideBUP = hideTable;
		$("#ExpandBUP").attr("data-expand", false);
		clientCookieOptions.ExpandBUP = false;
		updateBestUnrankedOptions();
	});
}

function toggleHideUnrankedList(hideTable) {
	if (hideTable) {
		$(".bup-hide-link").text("Show");
	}
	else {
		$(".bup-hide-link").text("Hide");
		toggleExpandUnrankedRows(false);
	}
	$("#bup-section").toggle(!hideTable);
}

function bindChangeUnrankedList() {
	$(".bup-rank-select").change(function (e) {
		if (changeUnrankedListTimeout) {
			clearTimeout(changeUnrankedListTimeout);
		}
		changeUnrankedListTimeout = setTimeout(changeUnrankedList, 60);
	});
}

function changeUnrankedList() {
	var bupId = $(".bup-rank-select").val();
	clientCookieOptions["BUPId"] = bupId;
	updateBestUnrankedOptions(function () {
		ajaxGetReplace("Rank/BupSectionPartial?rankId=" + $("#setupRank").attr("data-rank-id"),
			"#bup-section", function () {
				displayBestUnrankedPlayers();
				$(".bup-rank-select").focus();
			});
	});
}

function updateBestUnrankedOptions(successFn, errorFn) {
	ajaxPost({ options: clientCookieOptions }, "Rank/UpdateBestUnrankedOptions", successFn, errorFn);
}

/* End Best Unranked Players */

function getPlayerSelectId(elem) {
	var playerId = null;
	if (hasAttr($(elem), "data-player-id")) {
		playerId = $(elem).attr("data-player-id");
	}
	else if ($(elem).is("select")) {
		playerId = $(elem).val();
	}
	return playerId;
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
		var playerSelect = $("select", newPlayerRankEntry);
		bindPlayerSelectChange(playerSelect);
		$(playerSelect).focus();
	});
}

function bindPlayerSelectChange(playerSelect) {
	$(playerSelect).unbind("change");
	$(playerSelect).change(function (e) {
		if (toggleUnrankedTimeout) {
			clearTimeout(toggleUnrankedTimeout);
		}
		toggleUnrankedTimeout = setTimeout(toggleUnrankedPlayers, 35);
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
		if ($(".rank-remove-player:visible").length > 1) {
			var playerRankEntry = $(link).closest('.player-rank-entry');
			var playerRankNum = parseInt($(".player-rank-num", playerRankEntry).text());
			changeAllPlayerRankNums(playerRankNum, -1);
			$(playerRankEntry).remove();
			var playerId = getPlayerSelectId($(".player-select", playerRankEntry));
			if (playerId) {
				var unrankedPlayer = $(".bup-player-row[data-player-id=" + playerId + "]");
				$(unrankedPlayer).show();
				$(unrankedPlayer).addClass("unranked");
			}
			toggleUnrankedTableEmpty();
			toggleExpandUnrankedRows(toBool($(".bup-expand-link").attr("data-expand")));
			displayLinks();
		}
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
		var playerRankEntry = moveUpPlayer(link);
		$(playerRankEntry).fadeIn(180);
		$(playerRankEntry).focus();
	});
}

function moveUpPlayer(link) {
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
	displayLinks();
	return playerRankEntry;
};

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
	$(document).on("paste", function (e) {
		if (isBrowserIE()) {
			pastePlayerHandler(window.clipboardData, e, false);
		}
		else {
			pastePlayerHandler(e.originalEvent.clipboardData, e, false);
		}
	});

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
					pastePlayerHandler(window.clipboardData, e, true);
				}
			}
		});
	}

	$(".rank-paste-new").unbind("click");
	$(".rank-paste-new").click(function (e) {
		e.preventDefault();
		$(".rank-paste-new").addClass("hide-yo-wives");
		$(".rank-paste-ctrls").removeClass("hide-yo-wives");
	});

	$(".rank-paste-cancel").unbind("click");
	$(".rank-paste-cancel").click(function (e) {
		e.preventDefault();
		$(".rank-paste-ctrls").addClass("hide-yo-wives");
		$(".rank-paste-new").removeClass("hide-yo-wives");
	});
}

function pastePlayerHandler(clipboardData, e, skipPasteTextbox) {
	var pastedText = (clipboardData) ? clipboardData.getData('Text') : null;
	if (pastedText) {
		var onPlayerSelect = $(document.activeElement).is("select") && $(document.activeElement).hasClass("player-select");
		var onPasteTextbox = $(document.activeElement).is("input") && $(document.activeElement).hasClass("rank-paste-txt") && !skipPasteTextbox;
		if (onPlayerSelect || onPasteTextbox) {
			var destSelect = document.activeElement;
			addWaitCursor();
			showLoadingDialog();
			setTimeout(function () {
				if (onPasteTextbox) {
					$(".rank-add-player").eq(1).click();
					var newMoveUpLink = $(".rank-move-up").eq(2);
					var playerRankEntry = moveUpPlayer(newMoveUpLink);
					$(playerRankEntry).show();
					destSelect = $(".player-select", playerRankEntry);
				}
				var pastedArray;
				if (pastedText.indexOf("\r\n") > 0) {
					pastedArray = pastedText.split("\r\n");
				}
				else if (pastedText.indexOf("\r") > 0) {
					pastedArray = pastedText.split("\r");
				}
				else {
					pastedArray = pastedText.split("\n");
				}
				if (pastedArray.length > 0 && pastedArray[pastedArray.length - 1] == "") {
					pastedArray = pastedArray.splice(0, pastedArray.length - 1);
				}
				if (pastedArray.length > 0) {
					$.each(pastedArray, function (ix, txt) {
						selectPastedPlayer(txt, destSelect);
						//var newDestSelect = $(".player-select", $(destSelect).parent().parent().next());
						if (ix < (pastedArray.length - 1)) {
							$(".rank-add-player", $(destSelect).parents(".player-rank-entry")).click();
							destSelect = $(".player-select", $(destSelect).parents(".player-rank-entry").next());
						}
					});
					if (onPasteTextbox) {
						if (e) {
							e.preventDefault();
						}
						setTimeout(function () {
							$(".rank-paste-txt").val("");
							$(':focus').blur();
						}, 0);
					}
					setTimeout(function () {
						toggleUnrankedPlayers();
					}, 0);
				}
				removeWaitCursor();
				closeAllDialogs();
			}, 0);
		}
	}
}

function selectPastedPlayer(txt, destSelect) {
	if (txt && txt.length > 0) {
		var matchedVal = null;
		var pastedPlayerLength = txt.indexOf("(");
		if (pastedPlayerLength < 0) { pastedPlayerLength = txt.length; }
		var pastedPlayer = txt.substr(0, pastedPlayerLength);
		pastedPlayerLength = pastedPlayer.indexOf(",");
		if (pastedPlayerLength > 0) {
			pastedPlayer = txt.substr(0, pastedPlayerLength);
		}
		var scrubbedPlayer = scrubPlayerName(pastedPlayer);
		var formattedUnsubbedPlayer = formatName(pastedPlayer);

		$.each(_playerNameIds, function (playerKey, playerValue) {
			if (scrubbedPlayer.startsWith(playerKey)) {
				matchedVal = _playerNameIds[playerKey];
				return false;
			}
		});
		if (matchedVal == null) {
			$.each($("option", destSelect), function (ix, option) {
				if (matchedVal == null) {
					var optText = $(option).html();
					var playerNameLength = optText.indexOf(" (");
					if (playerNameLength < 0) { playerNameLength = optText.length; }
					var playerName = optText.substr(0, playerNameLength);
					var defenseName = null;
					if (isDEF(optText)) {
						var teamNameIx = playerName.lastIndexOf(" ");
						if (teamNameIx > 0) {
							defenseName = playerName.substr(teamNameIx + 1, playerNameLength);
						}
					}

					if (matchedVal != null) {
						return matchedVal;
					}

					if (playerNameLength > 0) {
						if ((formatName(optText).startsWith(scrubbedPlayer)
							|| scrubbedPlayer.startsWith(formatName(playerName)))
							|| (defenseName != null
								&& (formatName(defenseName).startsWith(scrubbedPlayer)
								|| scrubbedPlayer.startsWith(formatName(defenseName) + ' d')))
							) {
							matchedVal = $(option).val();
							return false;
						}
						else if (scrubbedPlayer != formattedUnsubbedPlayer
							&& (formatName(optText).startsWith(formattedUnsubbedPlayer)
							|| formattedUnsubbedPlayer.startsWith(formatName(playerName)))) {
							//If non-scrubbed name matches, set but allow to keep looking for scrubbed name
							matchedVal = $(option).val();
						}
					}
				}
			});
		}
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
	$.each(_playerNameSubs, function (playerKey, playerValue) {
		if (scrubbedName.startsWith(playerKey) || playerKey.startsWith(scrubbedName)) {
			scrubbedName = formatName(playerValue);
			return false;
		}
	});
	return scrubbedName;
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
		dialogClass: "visible-dialog",
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
							$(this).dialog("close");
							addWaitCursor();
							showLoadingDialog();
							setTimeout(function() {
								$(".rank-add-player").last().click();
								$(".rank-remove-player").not(":last").click();
								closePleaseWait();
							}, 1);
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