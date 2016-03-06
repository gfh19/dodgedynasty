var ranksWindow;
var clientCookieOptions = null;

function initPlayerRanksShared() {
	syncCookies();
	toggleHighlighting();
	bindExpandLinks();
	bindToggleAllLinks();
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
	//Re: "path: baseURL", appears that server Response.Cookies always uses path="/", both local and prod....
	//Doesn't work now locally, may be future enhancement to just set here (& userTurn) to "/"
}

function flipCookieValue(expandLinkId) {
	clientCookieOptions[expandLinkId] = !clientCookieOptions[expandLinkId];
	setCookieOptions(clientCookieOptions);
	return clientCookieOptions[expandLinkId];
}

//On load, opens tables to whatever is stored in cookie
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
	if (linkId == "ExpandQueue" && $("#ExpandQueue").is(":visible")) {
		toggleDeleteHighlightDisplay(expandRows);
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

//On load, hide/show highlighting based on what's in cookie
function toggleHighlighting() {
	if(!isHistoryMode()) {
		bindShowHighlightingLink();
		var showHighlighting = clientCookieOptions["ShowHighlighting"];
		if (showHighlighting) {
			enableHighlighting();
		} else {
			disableHighlighting();
		}
		changeHighlightColor();
		bindHighlightColorSelect();
		bindDeleteAllHighlighting();
	}
}

function enableHighlighting() {
	$(".pr-highlight-section").removeClass("hide-yo-wives");
	toggleQueueDisplay();
	$(".ba-category").css("cursor", "pointer");
	$(".pr-toggle-highlight").addClass("enabled");
	$("tr[data-player-id]").addClass("on");
	$(".pr-toggle-highlight").text("Turn Off Highlighting (also new)");
	$("tr[data-player-id]").click(function (e) {
		handlePlayerHighlightClick();
	});
}

function disableHighlighting() {
	$(".pr-highlight-section").addClass("hide-yo-wives");
	$("tr[data-player-id]").unbind("click");
	$(".ba-category").css("cursor", "auto");
	$(".pr-toggle-highlight").removeClass("enabled");
	$("tr[data-player-id]").removeClass("on");
	$(".pr-toggle-highlight").text("Show Highlighting *NEW!*");
}

function bindShowHighlightingLink() {
	$(".pr-toggle-highlight").click(function (e) {
		e.preventDefault();
		if ($(".pr-toggle-highlight").hasClass("enabled")) {
			disableHighlighting();
		}
		else {
			enableHighlighting();
		}
		flipCookieValue("ShowHighlighting");
	});
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
		toggleDeleteHighlightDisplay(!$("#ExpandQueue").is(":visible") || $("#ExpandQueue").data("expand"));
		//$(".hq-delete-span").removeClass("hide-yo-wives");
	}
	else {
		$(".queue-table").addClass("hide-yo-wives");
		$(".hq-empty-msg").removeClass("hide-yo-wives");
		//$(".hq-delete-span").addClass("hide-yo-wives");
		toggleDeleteHighlightDisplay(false);
	}
}

function handlePlayerHighlightClick() {
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
		pageBroadcastDraftHandler();
	}
	else {
		if ($("#highlight-color").val() != "<remove>") {
			addPlayerHighlighting(playerRow);
			pageBroadcastDraftHandler();
		}
	}
}

function getPlayerHighlightModel(playerRow) {
	return { PlayerId: $(playerRow).attr("data-player-id"), HighlightClass: $("#highlight-color").val() };
}

function addPlayerHighlighting(playerRow) {
	ajaxPost(getPlayerHighlightModel(playerRow), "Rank/AddPlayerHighlight", function (data) {
		$(playerRow).addClass("highlighted");
		$(playerRow).addClass($("#highlight-color").val());
	});
}

function removePlayerHighlighting(playerRow) {
	ajaxPost(getPlayerHighlightModel(playerRow), "Rank/DeletePlayerHighlight", function (data) {
		$(playerRow).removeClass("highlighted");
		$.each(highlightColors, function (ix, color) {
			$(playerRow).removeClass(color);
		});
	});
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

function bindHighlightColorSelect() {
	$("#highlight-color").change(changeHighlightColor);
}

function bindDeleteAllHighlighting() {
	$(".hq-delete-all").click(function (e) {
		e.preventDefault();
		$("#hqConfirmDelete").dialog({
			resizable: false,
			height: 'auto',
			width: '260px',
			modal: true,
			buttons: [
					{
						text: "OK",
						click: function () {
							addWaitCursor();
							ajaxPost({}, "Rank/DeleteAllHighlights", function () {
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

function toggleDeleteHighlightDisplay(shouldDisplay) {
	toggleDisplay($(".hq-delete-span"), shouldDisplay);
}