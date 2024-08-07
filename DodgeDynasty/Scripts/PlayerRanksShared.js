﻿var ranksWindow;
var clientCookieOptions = null;
var lastQueueUpdate = null;
var playerRanksRefreshCleanupFn = null;

function loadPlayerRanksShared() {
	callRefreshPageWithPickTimer(prRefreshPartial + getRankIdUrlPath(), replaceElementId,
		restoreHighlighting, restoreHighlighting, suspendHighlighting);
	setPickTimer(true);
	touchScrollDiv = ".rank-container";
}

function pageBroadcastDraftHandler(pickInfo) {
	updateRanksPageWithPick(pickInfo);
}

//For performant ranks page refresh
function updateRanksPageWithPick(pickInfo) {
	suspendHighlighting();
	if (isValidPickInfo(pickInfo)) {
		updateCurrentDraftPickPartial(pickInfo);
		updateNewLastPickEndTime(pickInfo);
		var currUserId = $("*[data-user-id]").attr("data-user-id");
		var yours = currUserId == pickInfo.puid;
		updateDraftPickRows(pickInfo, yours);
		toggleRanksWindows();
		var uturn = currUserId == pickInfo.uturnid;
		togglePlayerLinks(uturn);
		restoreHighlighting();
	}
	else {
		fullRefreshRanksPage();
	}
}

function fullRefreshRanksPage(miscCleanupFn) {
	callRefreshPage(prRefreshPartial + getRankIdUrlPath(), replaceElementId,
		function () {
			if (playerRanksRefreshCleanupFn) { playerRanksRefreshCleanupFn(); }
			restoreHighlighting();
			if (miscCleanupFn) { miscCleanupFn(); }
		}, function () {
			if (playerRanksRefreshCleanupFn) { playerRanksRefreshCleanupFn(); }
			restoreHighlighting();
			if (miscCleanupFn) { miscCleanupFn(); }
		}, suspendHighlighting);
}

function initPlayerRanksShared() {
	siteBroadcastDraftDelay = 50;
	toggleHighlighting();
	bindToggleAllLinks();
	bindExpandLinks();
	toggleRanksWindows();
	bindPlayerLinks();
	bindHideCategoryLinks();
	toggleCategoryTables();
	bindCompareRanks();
}

function bindExpandLinks() {
	var expandLinks = $(".expand-link");
	$.each(expandLinks, function (index, link) {
		bindToggleExpandLink(link);
	});
	var expandHdrLinks = $(".ba-expand-hdr-link");
	$.each(expandHdrLinks, function (index, link) {
		bindToggleExpandHdrLink(link);
	});
}

function bindToggleExpandLink(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var linkId = $(link).attr('id');
		var table = $("table[data-link='" + linkId + "']");

		$(link).attr("data-expand", !toBool($(link).attr("data-expand")));
		var expandRows = flipCookieValue(linkId);

		toggleExpandTableRows(table, expandRows, linkId);
	});
}

function bindToggleExpandHdrLink(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var expandLink = $(".expand-link", $(link).parents("table"));
		if (expandLink) {
			expandLink.click();
		}
	});
}

function bindToggleAllLinks() {
	$(".pr-expand-all").click(function (e) {
		e.preventDefault();
		var collapsedLinks = $(".expand-link[data-expand=false]");
		$.each(collapsedLinks, function (index, link) {
			if ($(link).attr("id") != "ExpandQueue" || $(link).is(":visible")) {
				$(link).click();
			}
		});
	});
	$(".pr-collapse-all").click(function (e) {
		e.preventDefault();
		var expandedLinks = $(".expand-link[data-expand=true]");
		$.each(expandedLinks, function (index, link) {
			$(link).click();
		});
	});
}

function bindPlayerLinks(links) {
	var playerLinks = links || $(".player-link");
	$.each(playerLinks, function (index, link) {
		bindPlayerLink(link);
	});
}

function bindPlayerLink(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var playerCol = $(link).closest('.ba-player-name');
		var playerId = $(playerCol).data('player-id');
		location.href = baseURL + "Draft/Pick?playerId=" + playerId;
	});
}

function togglePlayerLinks(showLinks) {
	//.ba-table - scratch that, update hq & cat tbls
	if (showLinks) {
		var playerLinks = $(".ba-table tr:not(.ba-selected) .pr-name-span:not(.player-link)");
		$(playerLinks).addClass("anchor");
		$(playerLinks).addClass("player-link");
		bindPlayerLinks(playerLinks);
	}
	else {
		var playerLinks = $(".ba-table .player-link");
		$(playerLinks).removeClass("anchor");
		$(playerLinks).removeClass("player-link");
		$(playerLinks).unbind("click");
	}
}

function bindHideCategoryLinks() {
	//var hideLinks = $(".ba-header:not(.hq-header)");
	var hideLinks = $(".ba-hide-cat-link");
	$.each(hideLinks, function (index, link) {
		bindHideCategoryLink(link);
	});
}

function bindHideCategoryLink(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var table = $(link).parents("table");
		var linkId = $(table).attr('data-link');
		var hideId = $(link).attr('data-hide-id');
		var hideCategory = flipCookieValue(hideId);
		toggleHideCategory(table, hideCategory);

		$("#"+linkId).attr("data-expand", false);
		clientCookieOptions[linkId] = false;
		toggleExpandTableRows(table, false, linkId);
	});
}

function toggleHideCategory(table, hideCategory) {
	if (hideCategory) {
		$(".ba-hide-cat-link", table).text("Show");
	}
	else {
		$(".ba-hide-cat-link", table).text("Hide");
	}
	$("tbody", table).toggle(!hideCategory);
}

function toggleCategoryTables() {
	$.each($(".pr-table"), function (ix, table) {
		var hideId = $(".ba-hide-cat-link", table).attr("data-hide-id");
		toggleHideCategory(table, clientCookieOptions[hideId]);
	});
}

function getCookieOptions() {
	return clientCookieOptions;
}

function setCookieOptions(options) {
	ajaxPost(options, "Rank/PostPlayerRankOptions");
}

function flipCookieValue(property) {
	var response = false;
	if (property.startsWith("ExpandCR-")) {
		var currentRankId = property.replace("ExpandCR-", "");
		response = setCompRankExpandIds(currentRankId);
	}
	else {
		clientCookieOptions[property] = !clientCookieOptions[property];
		response = clientCookieOptions[property];
	}
	setCookieOptions(clientCookieOptions);
	return response;
}

function setCompRankExpandIds(currentRankId) {
	var response = false;
	var compRankExpLinks = $('.expand-link[id^="ExpandCR-"]');
	var compRankExpIds = "";
	$.each(compRankExpLinks, function (ix, expLink) {
		var expValue = $(expLink).attr("data-expand");
		if (toBool(expValue)) {
			var rankId = $(expLink).attr("id").replace("ExpandCR-", "");
			compRankExpIds += rankId + ",";
			if (currentRankId && rankId == currentRankId) {
				response = true;
			}
		}
	});
	compRankExpIds = compRankExpIds.removeTrailing(",");
	clientCookieOptions["CompRankExpandIds"] = compRankExpIds;
	return response;
}

//On load, opens tables to whatever is stored in cookie
function toggleRanksWindows() {
	$.each($(".ba-table"), function (index, table) {
		toggleRanksTable(table);
	});
}

function toggleRanksTable(table) {
	var linkId = $(table).attr("data-link");
	var expandRows;
	if (linkId.startsWith("ExpandCR-")) {
		var currentRankId = linkId.replace("ExpandCR-", "")
		expandRows = getCompRankExpanded(currentRankId);
	}
	else {
		expandRows = clientCookieOptions[linkId];
	}
	toggleExpandTableRows(table, expandRows, linkId);
}

function getCompRankExpanded(rankId) {
	return (clientCookieOptions["CompRankExpandIds"] &&
		clientCookieOptions["CompRankExpandIds"].split(",").indexOf(rankId) > -1);
}

function toggleExpandTableRows(table, expandRows, linkId) {
	$("tr", table).show();
	var expandHdrLink = $(".ba-expand-hdr-link", $("#" + linkId).parents("table"));
	if (expandRows) {
		$("#" + linkId).text("Less...");
		$(expandHdrLink).text("Collapse");
	}
	else {
		$("tr:gt(" + ranksWindow + ")", table).hide();
		$("tr:last", table).show();
		$("#" + linkId).text("More...");
		$(expandHdrLink).text("Expand");
	}
	if (linkId == "ExpandQueue") {
		toggleDeleteHighlightDisplay();
	}
}

function getRankIdUrlPath(urlString) {
	var rankIdAndCompQS = urlString || "";
	var rankId = $(".rank-name").attr("data-rank-id");
	if (rankId != undefined && rankId.length > 0) {
		rankIdAndCompQS = addQSValue(rankIdAndCompQS, "rankId=" + rankId);
	}
	var compRank = $(".rank-name").attr("data-compare-val");
	if (compRank != undefined && compRank.length > 0) {
		rankIdAndCompQS = addQSValue(rankIdAndCompQS, "compare=" + compRank);
	}
	rankIdAndCompQS = getCompRankPosQS(rankIdAndCompQS);
	return rankIdAndCompQS;
}

function getCompRankPosQS(rankIdAndCompQS) {
	var qsText = rankIdAndCompQS || "";
	var compRankPos = $(".rank-name").attr("data-compare-pos");
	if (compRankPos != undefined && compRankPos.length > 0) {
		qsText = addQSValue(qsText, "position=" + compRankPos);
	}
	return qsText;
}

function isBestAvailablePage() {
	return replaceElementId == "#bestAvailable";
}



//Highlighting

//On load, hide/show highlighting based on what's in cookie
function toggleHighlighting(isQueueRefresh) {
	if (!isHistoryMode()) {
		toggleEnableHighlighting();
		if (isQueueRefresh) {
			bindExpandLinks();
			//Toggle Queue window must come after highlight section shown, or could pop to top of screen.  Go figure.
			toggleQueueWindow();
			bindQueuePlayerLinks();
		}

		var shouldPostCookie = false;
		shouldPostCookie = changeHighlightColor(false);

		bindShowHighlightingLink();
		bindHighlightColorSelect();
		bindDeleteAllHighlightingDialog();
		bindCopyLastDraftHighlights();
		bindEditHighlighting(".pr-empty-edit-link");
		bindHighlightQueueOptions();
		bindViewCurrentOtherDraftHighlights();
		bindDeleteHighlightQueueDialog();
		shouldPostCookie = syncHighlightQueueCookie(false) || shouldPostCookie;
		if (shouldPostCookie) {
			setCookieOptions(clientCookieOptions);
		}
	}
}

function toggleEnableHighlighting() {
	if (clientCookieOptions["ShowHighlighting"]) {
		enableHighlighting();
	} else {
		disableHighlighting();
	}
}

function toggleQueueWindow() {
	toggleRanksTable($(".pr-highlight-section .ba-table"));
}

function bindQueuePlayerLinks() {
	var playerLinks = $(".pr-highlight-section .player-link");
	$.each(playerLinks, function (index, link) {
		bindPlayerLink(link);
	});
}

function enableHighlighting() {
	$(".pr-highlight-section").removeClass("hide-yo-wives");
	toggleQueueDisplay();
	$(".ba-table tr[data-player-id]").addClass("on");
	$(".pr-toggle-highlight").text("Hide Highlighting");
	$(".pr-highlight-options").toggle(true);
	toggleEditHighlighting();
	bindEditHighlightingLink();
}

function disableHighlighting() {
	$(".pr-highlight-section").addClass("hide-yo-wives");
	$(".ba-table tr[data-player-id]").removeClass("on");
	$(".pr-toggle-highlight").text("Show Highlighting");
	$(".pr-highlight-options").toggle(false);
	disableEditHighlighting();
}

function bindShowHighlightingLink() {
	$(".pr-toggle-highlight").unbind("click");
	$(".pr-toggle-highlight").click(function (e) {
		e.preventDefault();
		clientCookieOptions["DraftHighlightId"] = getDraftHQId();
		flipCookieValue("ShowHighlighting");
		toggleEnableHighlighting();
	});
}

function bindEditHighlightingLink() {
	bindEditHighlighting(".pr-edit-highlight");
}

function bindEditHighlighting(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		flipCookieValue("LockHighlighting");
		toggleEditHighlighting();
	});
}

function toggleEditHighlighting() {
	var lockHighlighting = clientCookieOptions["LockHighlighting"];
	$(".hq-color-span").toggle(!lockHighlighting);
	$("#btnHQOptions").toggle(!lockHighlighting);
	$(".pr-highlight-section").toggleClass("highlight-locked", lockHighlighting);
	$(".pr-empty-edit-msg").toggle(lockHighlighting);
	if (lockHighlighting) {
		$(".pr-edit-highlight").text("Unlock Highlighter");
		disableEditHighlighting();
	}
	else {
		$(".pr-edit-highlight").text("Lock Highlighter");
		enableEditHighlighting();
	}
	toggleDeleteHighlightDisplay();
}

function enableEditHighlighting() {
	$(".ba-category").css("cursor", "pointer");
	$(".ba-table tr[data-player-id]").unbind("click");
	$(".ba-table tr[data-player-id]").click(function (e) {
		e.preventDefault();
		handlePlayerHighlightClick();
	});
	bindSortableQueue();
}

function disableEditHighlighting() {
	$(".ba-category").css("cursor", "auto");
	$(".ba-table tr[data-player-id]").unbind("click");
	unbindSortableQueue();
}

function toggleQueueDisplay() {
	var numQueueRows = $(".pr-highlight-section tr[data-player-id]");
	if (numQueueRows.length > 0) {
		$(".hq-empty-msg").addClass("hide-yo-wives");
		$(".queue-table").removeClass("hide-yo-wives");
		if (numQueueRows.length > ranksWindow) {
			$(".queue-table .expand").parent("tr").removeClass("hide-yo-wives");
		}
		else {
			$(".queue-table .expand").parent("tr").addClass("hide-yo-wives");
		}
	}
	else {
		$(".queue-table").addClass("hide-yo-wives");
		$(".hq-empty-msg").removeClass("hide-yo-wives");
	}
	toggleDeleteHighlightDisplay();
}

function handlePlayerHighlightClick() {
	if ($(event.target).hasClass("player-link")) {
		return;
	}
	var playerRow = $(event.target).closest("tr");
	if ($(playerRow).hasClass("highlighted")) {
		if ($(playerRow).parents(".queue-table").length > 0) {
			if ($("#highlight-color").val() == "<remove>") {
				removePlayerHighlighting(playerRow);
			}
			else {
				addPlayerHighlighting(playerRow);
			}
		}
		else {
			if ($("#highlight-color").val() != "<remove>" && !$(playerRow).hasClass($("#highlight-color").val())) {
				addPlayerHighlighting(playerRow);
			}
			else {
				removePlayerHighlighting(playerRow);
			}
		}
	}
	else {
		if ($("#highlight-color").val() != "<remove>") {
			addPlayerHighlighting(playerRow);
		}
	}
}

function getPlayerHighlightModel(playerRow) {
	return {
		DraftHighlightId: getDraftHQId(),
		PlayerId: $(playerRow).attr("data-player-id"),
		HighlightClass: $("#highlight-color").val()
	};
}

function getDraftHQId() {
	return $(".hq-display").attr("data-draft-hq-id");
}

function setDraftHQId(hqId) {
	return $(".hq-display").attr("data-draft-hq-id", hqId);
}

function addPlayerHighlighting(playerRow) {
	var updateId = $(playerRow).hasClass("highlighted") ? getRandomInt() : null;
	lastQueueUpdate = updateId;
	var playerRankModel = getPlayerHighlightModel(playerRow);
	ajaxPost(playerRankModel, "Rank/AddPlayerHighlight", function () {
		refreshHighlightQueue(updateId);
		clientAddPlayerHighlight(playerRankModel);
	}, fullRefreshRanksPage);	//Refresh whole ranks partial only on error
}

function removePlayerHighlighting(playerRow) {
	var playerRankModel = getPlayerHighlightModel(playerRow);
	ajaxPost(playerRankModel, "Rank/DeletePlayerHighlight", function () {
		refreshHighlightQueue(null);
		clientRemovePlayerHighlight(playerRankModel);
	}, fullRefreshRanksPage);	//Refresh whole ranks partial only on error
}

function refreshHighlightQueue(updateId) {
	var isBestAvailable = isBestAvailablePage();
	var refreshUrl = "Draft/HighlightQueueInnerPartial?isBestAvailable=" + isBestAvailable;
	ajaxGet(getRankIdUrlPath(refreshUrl), function (response) {
		//updateId - For throttling multiple highlight clicks down to only last one
		if (lastQueueUpdate == updateId || lastQueueUpdate == null) {
			replaceWith("#highlightQueueInnerPartial", response);
			toggleHighlighting(true);
			lastQueueUpdate = null;
		}
	}, function () {
		fullRefreshRanksPage();
		lastQueueUpdate = null;
	});
}

function clientAddPlayerHighlight(playerRankModel) {
	var playerRows = $(".ba-table tr[data-player-id=" + playerRankModel.PlayerId + "]");
	$.each(playerRows, function (ix, playerRow) {
		wipeAllHighlightColors(playerRow);
		$(playerRow).addClass("highlighted " + playerRankModel.HighlightClass);
	});
}

function clientRemovePlayerHighlight(playerRankModel) {
	var playerRows = $(".ba-table tr[data-player-id=" + playerRankModel.PlayerId + "]");
	$.each(playerRows, function (ix, playerRow) {
		wipeAllHighlightColors(playerRow);
	});
}

function wipeAllHighlightColors(playerRow) {
	$.each(_highlightColors, function (ix, color) {
		$(playerRow).removeClass(color);
	});
}


function bindHighlightColorSelect() {
	$("#highlight-color").change(changeHighlightColor);
}

function changeHighlightColor(shouldPostCookie = true) {
	var response = false;
	var colorSpan = $(".hq-color-span");
	var newColor = $("#highlight-color").val();
	$(colorSpan).removeClass();
	$(colorSpan).addClass("hq-color-span");
	$(colorSpan).addClass(newColor);
	if (clientCookieOptions["HighlightColor"] != newColor) {
		clientCookieOptions["HighlightColor"] = newColor;
		if (shouldPostCookie) {
			setCookieOptions(clientCookieOptions);
		}
		response = true;
	}
	return response;
}

function bindDeleteAllHighlightingDialog() {
	$(".hq-delete-all").click(function (e) {
		e.preventDefault();
		$("#hqConfirmDelete").dialog({
			resizable: false,
			height: 'auto',
			width: '240px',
			modal: true,
			buttons: [
					{
						text: "OK",
						click: function () {
							showPleaseWait();
							ajaxPost({ DraftHighlightId: getDraftHQId() }, "Rank/DeleteAllHighlights", function () {
								$("#ExpandQueue[data-expand=true]").click();	//Collapse if expanded
								fullRefreshRanksPage(closePleaseWait);
							}, closePleaseWait);
							$(this).dialog("close");
						}
					},
					{ text: "Cancel", click: function () { $(this).dialog("close"); } },
			]
		});
		return false;
	});
}

function bindDeleteHighlightQueueDialog() {
	$(".hq-delete-queue-lnk").click(function (e) {
		e.preventDefault();
		$("#hqConfirmQueueDelete").dialog({
			resizable: false,
			height: 'auto',
			width: '240px',
			modal: true,
			buttons: [
				{
					text: "OK",
					click: function () {
						showPleaseWait();
						ajaxPost({ DraftHighlightId: getDraftHQId() }, "Rank/DeleteHighlightQueue", function () {
							$("#ExpandQueue[data-expand=true]").click();	//Collapse if expanded
							fullRefreshRanksPage(closePleaseWait);
						}, closePleaseWait);
						$(this).dialog("close");
					}
				},
				{ text: "Cancel", click: function () { $(this).dialog("close"); } },
			]
		});
		return false;
	});
}

function bindHighlightQueueOptions() {
	$("#btnHQOptions").unbind("click");
	$("#btnHQOptions").click(function (e) {
		e.preventDefault();
		setDefaultQueueName();
		setDefaultNewQueueName();

		$("#hqOptionsDialog").dialog({
			resizable: false,
			height: 'auto',
			width: '240px',
			modal: true,
			autoOpen: true,
			buttons: [
				{ text: "Cancel", click: function () { $(this).dialog("close"); } },
			]
		});
		return false;
	});

	$("#btnChangeQueueName").unbind("click");
	$("#btnChangeQueueName").click(function (e) {
		e.preventDefault();
		var model = {
			DraftHighlightId: getDraftHQId(),
			QueueName: $("#txtChangeQueueName").val()
		};
		addEditHighlightQueue(model, null);
	});

	$("#btnAddNewQueue").unbind("click");
	$("#btnAddNewQueue").click(function (e) {
		e.preventDefault();
		var oldModel = {
			DraftHighlightId: getDraftHQId(),
			QueueName: $("#txtChangeQueueName").val()
		};
		var newModel = {
			QueueName: $("#txtNewQueueName").val()
		};
		addEditHighlightQueue(oldModel, newModel);
	});
}

function isHQOptionsDialogOpen() {
	return $("#hqOptionsDialog").dialog({ autoOpen: false }).dialog("isOpen");
}

function syncHighlightQueueCookie(shouldPostCookie=true) {
	var response = false;
	if (getDraftHQId() && getDraftHQId() != clientCookieOptions["DraftHighlightId"]) {
		clientCookieOptions["DraftHighlightId"] = getDraftHQId();
		if (shouldPostCookie) {
			setCookieOptions(clientCookieOptions);
		}
	}
	return response;
}

function addEditHighlightQueue(oldModel, newModel) {
	if (isHQOptionsDialogOpen()) {
		$("#hqOptionsDialog").dialog("close");
	}
	showPleaseWait(null, 300);
	ajaxPost({ oldModel: oldModel, newModel: newModel }, "Rank/AddEditHighlightQueue", function (data) {
		var response = JSON.parse(data);
		if (response.queueId && response.queueId != clientCookieOptions["DraftHighlightId"]) {
			clientCookieOptions["DraftHighlightId"] = response.queueId;
			ajaxPost(clientCookieOptions, "Rank/PostPlayerRankOptions", function () {
				fullRefreshRanksPage(closePleaseWait);
			});
		}
		else {
			refreshHighlightQueue();
			closePleaseWait();
		}
	}, function () {
		closePleaseWait();
	});
}

function setDefaultQueueName() {
	var defaultQueueName = $(".hq-header").text();
	if (!defaultQueueName || defaultQueueName == "Highlighted Queue") {
		defaultQueueName = $(".hq-league-name").val();
	}
	$("#txtChangeQueueName").val(defaultQueueName);
}

function setDefaultNewQueueName() {
	var queueName = $("#txtChangeQueueName").val().trim();
	var newQueueName = queueName + " 2";
	if (queueName.length > 0) {
		if (queueName.lastIndexOf(" ") > 0) {
			var queueSfx = queueName.substring(queueName.lastIndexOf(" ") + 1, queueName.length);
			if ($.isNumeric(queueSfx)) {
				var coreQueueName = queueName.substring(0, queueName.lastIndexOf(" "));
				newQueueName = coreQueueName + " " + (parseInt(queueSfx) + 1).toString();
			}
		}
		newQueueName = newQueueName.substring(0, 25);
	}
	else {
		newQueueName = "Highlighted Queue";
	}

	$("#txtNewQueueName").val(newQueueName);
}

function toggleDeleteHighlightDisplay() {
	var anyHighlightedPlayers = $(".pr-highlight-section tr[data-player-id]").length > 0;
	var isExpandQueueVisible = $("#ExpandQueue").is(":visible");
	var isQueueExpanded = $("#ExpandQueue").attr("data-expand") == "true";
	var isHighlightLocked = clientCookieOptions["LockHighlighting"];
	var shouldDisplay = false;
	if (anyHighlightedPlayers) {
		shouldDisplay = (!isExpandQueueVisible || isQueueExpanded) && !isHighlightLocked;
	}
	else {
		shouldDisplay = false;
	}
	toggleDisplay($(".hq-delete-span"), shouldDisplay);
	toggleDisplay($(".hq-delete-queue-span"), !anyHighlightedPlayers);
}

function bindViewCurrentOtherDraftHighlights() {
	$(".hq-view-current-dh").click(function (e) {
		e.preventDefault();
		showPleaseWait();
		setDraftHQId($(this).attr("data-hq-dh-id"));
		clientCookieOptions["DraftHighlightId"] = getDraftHQId();
		ajaxPost(clientCookieOptions, "Rank/PostPlayerRankOptions", function () {
			fullRefreshRanksPage(closePleaseWait);
		}, closePleaseWait);
	});
}

function bindCopyLastDraftHighlights() {
	$(".hq-copy-last-draft").click(function (e) {
		e.preventDefault();
		copyLastDraftHighlights($(this).attr("data-hq-dh-id"));
	});
}

function copyLastDraftHighlights(hqDHId) {
	showPleaseWait();
	ajaxPost({ LastDraftHighlightId: hqDHId, NewDraftHighlightId: getDraftHQId() }, "Rank/CopyLastDraftHighlights", function () {
		fullRefreshRanksPage(closePleaseWait);
	}, closePleaseWait);
}

//jQuery UI Sortable for Drag & drop
function bindSortableQueue() {
	$(".queue-table tbody").sortable({
		disabled: false,
		items: "tr:not(.unsortable)",
		delay: 175,
		scrollSensitivity: 25,
		update: function (event, ui) {
			var prevRow = $(ui.item).prev("tr[data-player-id]");
			var nextRow = $(ui.item).next("tr[data-player-id]");
			var prevPlayerId = "", nextPlayerId = "";
			if (prevRow) {
				prevPlayerId = $(prevRow).attr("data-player-id");
			}
			if (nextRow) {
				nextPlayerId = $(nextRow).attr("data-player-id");
			}
			var playerQueueOrderModel = {
				DraftHighlightId: getDraftHQId(),
				UpdatedPlayerId: $(ui.item).attr("data-player-id"),
				PreviousPlayerId: prevPlayerId,
				NextPlayerId: nextPlayerId
			}
			updatePlayerQueueOrder(playerQueueOrderModel);
		}
	})
}

function unbindSortableQueue() {
	$(".queue-table tbody").sortable({
		disabled: true
	});
};

function updatePlayerQueueOrder(playerQueueOrderModel) {
	addWaitCursor();
	ajaxPost(playerQueueOrderModel, "Rank/UpdatePlayerQueueOrder", function () {
		updateQueueRankNums(playerQueueOrderModel);
		removeWaitCursor();
	}, fullRefreshRanksPage);
}

function updateQueueRankNums(playerQueueOrderModel) {
	var newRankNum = 1;
	var playerRows = $("td[data-rank-num]");
	$.each(playerRows, function (index, player) {
		$(player).attr("data-rank-num", newRankNum);
		$(player).text(newRankNum);
		newRankNum++;
	});
}

function suspendHighlighting() {
	if (clientCookieOptions["ShowHighlighting"] && !clientCookieOptions["LockHighlighting"]) {
		disableEditHighlighting();
	}
}

function restoreHighlighting() {
	if (clientCookieOptions["ShowHighlighting"] && !clientCookieOptions["LockHighlighting"]) {
		enableEditHighlighting();
	}
}



//Compare Ranks

function bindCompareRanks() {
	var selects = $(".cr-rank-select");
	$.each(selects, function (index, select) {
		bindCompareRanksSelect(select);
	});

	var addLinks = $(".cr-add-rank-text, .cr-add-rank");
	$.each(addLinks, function (index, link) {
		bindAddCompareRank(link);
	});

	var removeLinks = $(".cr-remove-link");
	$.each(removeLinks, function (index, link) {
		bindRemoveCompareRankLink(link);
	});

	bindShowAvgCompRanks($(".cr-chk-show-avg"));

	bindCompRankPosition($("#crPosition"));
}

function updateCompareRankIds(removeRankId, removeRankIndex) {
	var compareRankIds = "";
	$.each($(".cr-rank-select"), function (ix, select) {
		if (!removeRankId || removeRankId != $(select).val() || removeRankIndex < 0 || ix != removeRankIndex) {
			compareRankIds += $(select).val() + ",";
		}
	});
	compareRankIds = compareRankIds.removeTrailing(",");
	showPleaseWait();
	ajaxPostReplace({ compRankIds: compareRankIds, isBestAvailable: isBestAvailablePage() },
		"Draft/UpdateCompareRankIds" + getCompRankPosQS(), replaceElementId, function () {
			closePleaseWait();
			setCompRankExpandIds();
			setCookieOptions(clientCookieOptions);
		}, closePleaseWait);
}

function updatePlayerRankOptions(options) {
	showPleaseWait();
	ajaxPostReplace({ options: options, isBestAvailable: isBestAvailablePage() },
		"Draft/UpdatePlayerRankOptions", replaceElementId, closePleaseWait, closePleaseWait);
}

function bindCompareRanksSelect(select) {
	$(select).unbind("change");
	$(select).change(function (e) {
		e.preventDefault();
		updateCompareRankIds();
	});
}

function bindAddCompareRank(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		showPleaseWait();
		ajaxPostReplace({ isBestAvailable: isBestAvailablePage() }, "Draft/AddCompareRank" + getCompRankPosQS(), replaceElementId,
			closePleaseWait, closePleaseWait);
	});
}

function bindRemoveCompareRankLink(link) {
	$(link).unbind("click");
	$(link).click(function (e) {
		e.preventDefault();
		var removeRankId = $(link).attr('data-rank-id');
		var removeRankGuid = $(link).attr('data-rank-guid');
		var guids = $("a[data-rank-guid]").map(function() {
			return $(this).attr("data-rank-guid");
		});
		updateCompareRankIds(removeRankId, $.inArray(removeRankGuid, guids));
	});
}

function bindShowAvgCompRanks(checkbox) {
	$(checkbox).unbind("change");
	$(checkbox).change(function (e) {
		var showAvg = $(checkbox).prop('checked');
		clientCookieOptions["ShowAvgCompRanks"] = showAvg;
		updatePlayerRankOptions(clientCookieOptions);
	});
}

function bindCompRankPosition(select) {
	$(select).unbind("change");
	$(select).change(function (e) {
		var crPosition = $(select).val();
		$(".rank-name").attr("data-compare-pos", crPosition);
		playerRanksRefreshCleanupFn = tempPlayerRanksRefreshCleanupFn;
		showPleaseWait();
		fullRefreshRanksPage();
	});
}

function tempPlayerRanksRefreshCleanupFn() {
	closePleaseWait();
	clearPlayerRanksRefreshCleanupFn();
}

function clearPlayerRanksRefreshCleanupFn() {
	playerRanksRefreshCleanupFn = null;
}