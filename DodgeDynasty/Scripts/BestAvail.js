﻿var replaceElementId = "#bestAvailable";
$(function () {
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId,
		restoreHighlighting, restoreHighlighting, suspendHighlighting);
	setPickTimer(true);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId,
		function () {
			if (playerRanksBroadcastFn) { playerRanksBroadcastFn(); }
			restoreHighlighting();
		}, function () {
			if (playerRanksBroadcastFn) { playerRanksBroadcastFn(); }
			restoreHighlighting();
		}, suspendHighlighting);
}