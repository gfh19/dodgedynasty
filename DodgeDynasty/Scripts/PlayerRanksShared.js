﻿var ranksWindow;
var clientCookieOptions = null;

function initPlayerRanksShared() {
	syncCookies();
	toggleHighlighting();
	bindToggleAllLinks();
	bindExpandLinks();
	toggleRanksWindows();
	bindPlayerLinks();
	bindHideCategoryLinks();
	toggleCategoryTables();
	bindCompareRanksSelects();
}

function bindExpandLinks() {
	var expandLinks = $(".expand-link");
	$.each(expandLinks, function (index, link) {
		bindToggleLink(link);
	});
}

function bindToggleLink(link) {
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

function bindPlayerLinks() {
	var playerLinks = $(".player-link");
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
	$.each($(".ba-table:not(.queue-table)"), function (ix, table) {
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
	clientCookieOptions[property] = !clientCookieOptions[property];
	setCookieOptions(clientCookieOptions);
	return clientCookieOptions[property];
}

//On load, opens tables to whatever is stored in cookie
function toggleRanksWindows() {
	$.each($(".ba-table"), function (index, table) {
		toggleRanksTable(table);
	});
}

function toggleRanksTable(table) {
	var linkId = $(table).attr("data-link");
	var expandRows = clientCookieOptions[linkId];
	toggleExpandTableRows(table, expandRows, linkId);
}

function toggleExpandTableRows(table, expandRows, linkId) {
	if (expandRows) {
		$("tr", table).show();
		$("#" + linkId).text("Less...");
	}
	else {
		$("tr:gt(" + ranksWindow + ")", table).hide();
		$("tr:last", table).show();
		$("#" + linkId).text("More...");
	}
	if (linkId == "ExpandQueue") {
		toggleDeleteHighlightDisplay();
	}
}

function syncCookies() {
	if (clientCookieOptions == null) {
		var cookieOptions = getCookieOptions();
		clientCookieOptions = cookieOptions;
	}
}

function getRankIdUrlPath() {
	var rankId = $(".rank-name").attr("data-rank-id");
	if (rankId != undefined && rankId.length > 0) {
		return "?rankId=" + rankId;
	}
	return "";
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

		changeHighlightColor();

		bindShowHighlightingLink();
		bindHighlightColorSelect();
		bindDeleteAllHighlightingDialog();
		bindCopyLastDraftHighlights();
		bindEditHighlighting(".pr-empty-edit-link");
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
	$("tr[data-player-id]").addClass("on");
	$(".pr-toggle-highlight").text("Hide Highlighting");
	$(".pr-highlight-options").toggle(true);
	toggleEditHighlighting();
	bindEditHighlightingLink();
}

function disableHighlighting() {
	$(".pr-highlight-section").addClass("hide-yo-wives");
	$("tr[data-player-id]").removeClass("on");
	$(".pr-toggle-highlight").text("Show Highlighting *NEW!*");
	$(".pr-highlight-options").toggle(false);
	disableEditHighlighting();
}

function bindShowHighlightingLink() {
	$(".pr-toggle-highlight").unbind("click");
	$(".pr-toggle-highlight").click(function (e) {
		e.preventDefault();
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
	$(".pr-highlight-section").toggleClass("highlight-locked", lockHighlighting);
	$(".pr-empty-edit-msg").toggle(lockHighlighting);
	if (lockHighlighting) {
		$(".pr-edit-highlight").text("Turn Edit ON");
		disableEditHighlighting();
	}
	else {
		$(".pr-edit-highlight").text("Turn Edit Off");
		enableEditHighlighting();
	}
	toggleDeleteHighlightDisplay();
}

function enableEditHighlighting() {
	$(".ba-category").css("cursor", "pointer");
	$("tr[data-player-id]").unbind("click");
	$("tr[data-player-id]").click(function (e) {
		e.preventDefault();
		handlePlayerHighlightClick();
	});
	bindSortableQueue();
}

function disableEditHighlighting() {
	$(".ba-category").css("cursor", "auto");
	$("tr[data-player-id]").unbind("click");
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
		PlayerId: $(playerRow).attr("data-player-id"),
		HighlightClass: $("#highlight-color").val()
	};
}

function addPlayerHighlighting(playerRow) {
	var playerRankModel = getPlayerHighlightModel(playerRow);
	ajaxPost(playerRankModel, "Rank/AddPlayerHighlight", function () {
		refreshHighlightQueue();
		clientAddPlayerHighlight(playerRankModel);
	}, pageBroadcastDraftHandler);	//Refresh whole ranks partial only on error
}

function removePlayerHighlighting(playerRow) {
	var playerRankModel = getPlayerHighlightModel(playerRow);
	ajaxPost(playerRankModel, "Rank/DeletePlayerHighlight", function () {
		refreshHighlightQueue();
		clientRemovePlayerHighlight(playerRankModel);
	}, pageBroadcastDraftHandler);	//Refresh whole ranks partial only on error
}

function refreshHighlightQueue() {
	var isBestAvailable = isBestAvailablePage();
	ajaxGetReplace("Draft/HighlightQueuePartial?isBestAvailable=" + isBestAvailable, "#highlightQueuePartial", function () {
		toggleHighlighting(true);
	}, pageBroadcastDraftHandler);	//Refresh whole ranks partial only on error
}

function clientAddPlayerHighlight(playerRankModel) {
	var playerRows = $("tr[data-player-id=" + playerRankModel.PlayerId + "]");
	$.each(playerRows, function (ix, playerRow) {
		wipeAllHighlightColors(playerRow);
		$(playerRow).addClass("highlighted " + playerRankModel.HighlightClass);
	});
}

function clientRemovePlayerHighlight(playerRankModel) {
	var playerRows = $("tr[data-player-id=" + playerRankModel.PlayerId + "]");
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

function changeHighlightColor() {
	var colorSpan = $(".hq-color-span");
	var newColor = $("#highlight-color").val();
	$(colorSpan).removeClass();
	$(colorSpan).addClass("hq-color-span");
	$(colorSpan).addClass(newColor);
	clientCookieOptions["HighlightColor"] = newColor;
	setCookieOptions(clientCookieOptions);
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
							addWaitCursor();
							ajaxPost({}, "Rank/DeleteAllHighlights", function () {
								$("#ExpandQueue[data-expand=true]").click();	//Collapse if expanded
								pageBroadcastDraftHandler();
								removeWaitCursor();
							}, removeWaitCursor);
							$(this).dialog("close");
						}
					},
					{ text: "Cancel", click: function () { $(this).dialog("close"); } },
			]
		});
		return false;
	});
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
}

function bindCopyLastDraftHighlights() {
	$(".hq-copy-last-draft").click(function (e) {
		e.preventDefault();
		copyLastDraftHighlights();
	});
}

function copyLastDraftHighlights() {
	ajaxPost({}, "Rank/CopyLastDraftHighlights", pageBroadcastDraftHandler);
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
			var prevPlayerId = "";
			if (prevRow) {
				prevPlayerId = $(prevRow).attr("data-player-id");
			}
			var playerQueueOrderModel = {
				UpdatedPlayerId: $(ui.item).attr("data-player-id"),
				PreviousPlayerId: prevPlayerId
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
	}, pageBroadcastDraftHandler);
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



//Compare Ranks

function bindCompareRanksSelects() {
	var selects = $(".cr-rank-select");
	$.each(selects, function (index, select) {
		bindCompareRanksSelect(select);
	});
}

function bindCompareRanksSelect(select) {
	$(select).unbind("change");
	$(select).change(function (e) {
		e.preventDefault();
		var compareRankdIds = "";
		$.each($(".cr-rank-select"), function (ix, select) {
			compareRankdIds += $(select).val() + ",";
		});
		compareRankdIds = compareRankdIds.removeTrailing(",")
		ajaxPostReplace({ compRankIds: compareRankdIds, isBestAvailable: isBestAvailablePage() },
			"Draft/UpdateCompareRankSelects", replaceElementId);
	});
}