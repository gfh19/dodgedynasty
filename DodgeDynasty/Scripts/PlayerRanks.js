var replaceElementId = "#allPlayerRanks";
$(function () {
	callRefreshPageWithPickTimer("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId,
		restoreHighlighting, restoreHighlighting, suspendHighlighting);
	setPickTimer(true);
	touchScrollDiv = ".rank-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/PlayerRanksPartial" + getRankIdUrlPath(), replaceElementId,
		restoreHighlighting, restoreHighlighting, suspendHighlighting);
}