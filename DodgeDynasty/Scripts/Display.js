$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/DisplayPartial", '#draftDisplay');
});

function broadcastDraft() {
	callRefreshPage("Draft/DisplayPartial", '#draftDisplay');
}