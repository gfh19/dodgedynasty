var teamDisplayPartialUrl = "Draft/TeamDisplayPartial";

$(function () {
	setPickTimer(true);

	callRefreshPageWithPickTimer(getTeamDisplayUrl, '#teamDisplay');
	touchScrollDiv = ".team-container";
});

function pageBroadcastDraftHandler(pickInfo) {
	updatePageWithDraftPickInfo(pickInfo, function (pickInfo) {
		var draftPick = $(".team-draft-pick[data-pick-num=" + pickInfo.pnum + "]");
		if (draftPick) {
			$(".player-nflteam", draftPick).text(pickInfo.team + "-");
			$(".player-pos", draftPick).text(pickInfo.pos);
			$(".player-name", draftPick).text(pickInfo.pname);
			$(draftPick).addClass("filled");
			var posSfx = (pickInfo.pos.toLowerCase() === "te") ? "wrte" : pickInfo.pos.toLowerCase();
			var posColorCss = (shouldShowPosColors)
				? "show-pos-colors dp-" + posSfx
				: "dp-" + posSfx;
			$(draftPick).addClass(posColorCss);
		}
	}, function () {
		callRefreshPage(getTeamDisplayUrl, '#teamDisplay');
	});
}

function getTeamDisplayUrl() {
	var teamDisplayUrl = teamDisplayPartialUrl;
	if ($("#team-by-pos").is(':checked')) {
		teamDisplayUrl = addQSValue(teamDisplayUrl, "byPositions=true");
	}
	return teamDisplayUrl;
}

function initTeamDisplay() {
	bindByPositionCheckbox();
}

function bindByPositionCheckbox() {
	$("#team-by-pos").click(function () {
		var teamDisplayUrl = getCurrentDraftPartialUrl(teamDisplayPartialUrl);
		if ($("#team-by-pos").is(':checked')) {
			addWaitCursor();
			callRefreshPage(addQSValue(teamDisplayUrl, "byPositions=true"), '#teamDisplay', function () {
				$("#team-by-pos").prop("checked", true);
				removeWaitCursor();
			});
		}
		else {
			addWaitCursor();
			callRefreshPage(addQSValue(teamDisplayUrl, "byPositions=false"), '#teamDisplay', function () {
				$("#team-by-pos").prop("checked", false);
				removeWaitCursor();
			});
		}
	});
}

function getCurrentDraftPartialUrl(currentPartialUrl) {
	if (!window.location.pathname.replace(/\//g, '').endsWith("TeamDisplay") && window.location.pathname.lastIndexOf("/") > 0) {
		return currentPartialUrl + "/" + window.location.pathname.substr(window.location.pathname.lastIndexOf("/") + 1);
	}
	return currentPartialUrl;
}