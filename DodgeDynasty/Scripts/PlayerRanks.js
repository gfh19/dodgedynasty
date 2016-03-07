var replaceElementId = "#allPlayerRanks";
$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId);
}