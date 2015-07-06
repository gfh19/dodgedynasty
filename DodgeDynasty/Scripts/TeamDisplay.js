$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/TeamDisplayPartial", '#teamDisplay');
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/TeamDisplayPartial", '#teamDisplay');
}