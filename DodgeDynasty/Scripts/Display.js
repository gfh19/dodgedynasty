$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/DisplayPartial", '#draftDisplay');
	touchScrollDiv = ".draft-container";
});

function pageBroadcastDraftHandler() {
	callRefreshPage("Draft/DisplayPartial", '#draftDisplay');
}