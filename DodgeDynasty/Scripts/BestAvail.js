$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
}