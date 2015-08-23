$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
}