$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
}