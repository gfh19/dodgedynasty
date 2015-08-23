$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
}