$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/DisplayPartial", '#draftDisplay');
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/DisplayPartial", '#draftDisplay');
}