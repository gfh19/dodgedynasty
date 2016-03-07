var replaceElementId = "#bestAvailable";
$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), replaceElementId);
}