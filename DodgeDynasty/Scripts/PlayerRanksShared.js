﻿var ranksWindow;
var clientCookieOptions = null;
var highlightedPlayers = {};

function initPlayerRanksShared() {
	bindExpandLinks();
	bindToggleAllLinks();
	syncCookies();
	toggleRanksWindows();
	bindPlayerLinks();
}

function bindExpandLinks() {
	var expandLinks = $(".expand-link");
	$.each(expandLinks, function (index, link) {
		bindToggleLink(link);
	});
}

function bindToggleLink(link) {
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
			$(link).click();
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
		$(link).click(function (e) {
			e.preventDefault();
			var playerCol = $(link).closest('.ba-player-name');
			var playerId = $(playerCol).data('player-id');
			location.href = baseURL + "Draft/Pick?playerId=" + playerId;
		});
	});
}

function getCookieOptions() {
	return jQuery.parseJSON($.cookie("playerRankOptions"));
}

function setCookieOptions(options) {
	$.cookie("playerRankOptions", JSON.stringify(options), { path: baseURL });
}

function flipCookieValue(expandLinkId) {
	clientCookieOptions[expandLinkId] = !clientCookieOptions[expandLinkId];
	setCookieOptions(clientCookieOptions);
	return clientCookieOptions[expandLinkId];
}

function toggleRanksWindows() {
	$.each($(".ba-table"), function (index, table) {
		var linkId = $(table).attr("data-link");
		var expandRows = clientCookieOptions[linkId];
		toggleExpandTableRows(table, expandRows, linkId);
	});
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


//Highlighting

function handlePlayerHighlightClick() {
	var playerRow = $(event.target).closest("tr");
	if ($(playerRow).hasClass("highlighted")) {
		removePlayerHighlighting(playerRow);
	}
	else {
		addPlayerHighlighting(playerRow);
	}
}

function getPlayerHighlightModel(playerRow) {
	return { PlayerId: $(playerRow).attr("data-player-id"), HighlightId: "1" };
}

function addPlayerHighlighting(playerRow) {
	ajaxPost(getPlayerHighlightModel(playerRow), "Rank/AddPlayerHighlight", function (data) {
		$(playerRow).addClass("highlighted");
		$(playerRow).css("background-color", "yellow");
	});
}

function removePlayerHighlighting(playerRow) {
	ajaxPost(getPlayerHighlightModel(playerRow), "Rank/DeletePlayerHighlight", function (data) {
		$(playerRow).removeClass("highlighted");
		$(playerRow).closest("tr").css("background-color", "");
	});
}

$("tr[data-player-id]").click(function (e) {
	handlePlayerHighlightClick();
});

$(".ba-category").css("cursor", "pointer");