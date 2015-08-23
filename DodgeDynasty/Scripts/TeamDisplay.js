$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/TeamDisplayPartial", '#teamDisplay');
	touchScrollDiv = ".team-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/TeamDisplayPartial", '#teamDisplay');
}