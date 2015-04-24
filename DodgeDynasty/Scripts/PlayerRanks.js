$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
});

function broadcastDraft() {
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), "#allPlayerRanks");
}