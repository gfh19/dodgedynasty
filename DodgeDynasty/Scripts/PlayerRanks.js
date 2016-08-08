var replaceElementId = "#allPlayerRanks";
$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	var suspended = suspendHighlighting();
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId, function () {
		restoreHighlighting(suspended);
	}, function () {
		restoreHighlighting(suspended);
	});
}