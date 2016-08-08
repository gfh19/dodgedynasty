var replaceElementId = "#bestAvailable";
$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	var suspended = suspendHighlighting();
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId, function () {
		restoreHighlighting(suspended);
	}, function () {
		restoreHighlighting(suspended);
	});
}