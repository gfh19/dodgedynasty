var playerIds;

 $(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/CurrentDraftPickPartial", ".draft-info");
 });

function broadcastDraft() {
	callRefreshPage("Draft/CurrentDraftPickPartial", ".draft-info");
}

function initSetupRank() {
	displaySavedMessage();
	displayLinks();
	bindAddPlayerLinks();
	bindRemovePlayerLinks();
	bindMoveUpPlayerLinks();
	bindMoveDownPlayerLinks();
	bindSubmitRankings();
	bindAddNewPlayerLink();
	saveCookieRankId();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function displaySavedMessage() {
	if ($(".rank-saved").is(":visible")) {
		setTimeout(function () {
			$(".rank-saved").fadeOut("slow");
		}, 3000);
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
	//TODO:  Remove when draft id sql loaded
	rankSetupModel.DraftId = $("#setupRank").attr("data-draft-id");
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
		//TODO:  player name parts (not used)
		var playerEntryText = $("select option:selected", playerRank).text();
		var firstSpaceIx = playerEntryText.indexOf(' ');
		var leftParenIx = playerEntryText.indexOf('(');
		var dashIx = playerEntryText.indexOf('-');
		var rightParenIx = playerEntryText.indexOf(')');
		player.FirstName = playerEntryText.substr(0, firstSpaceIx);
		player.LastName = playerEntryText.substr(firstSpaceIx + 1, leftParenIx - (firstSpaceIx + 2));
		player.NFLTeam = playerEntryText.substr(leftParenIx + 1, dashIx - (leftParenIx + 1));
		player.Position = playerEntryText.substr(dashIx + 1, rightParenIx - (dashIx + 1));
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
	//Find any matching selected options
	var invalidEntries = $("select option:selected[value=" + playerId + "]").closest(".player-rank-entry");
	$(invalidEntries).addClass("invalid-border");
	//Find any matching spans
	invalidEntries = $(".player-select[data-player-id=" + playerId + "]").closest(".player-rank-entry");
	$(invalidEntries).addClass("invalid-border");
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
			$("#rankStatus").val(rankStatus);
			$("#setupRankForm").submit();
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
