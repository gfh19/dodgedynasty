$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
});

function broadcastDraft() {
	callRefreshPage("Draft/BestAvailablePartial" + getRankIdUrlPath(), "#bestAvailable");
}