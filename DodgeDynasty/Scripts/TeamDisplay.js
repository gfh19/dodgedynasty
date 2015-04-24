$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/TeamDisplayPartial", '#teamDisplay');
});

function broadcastDraft() {
	callRefreshPage("Draft/TeamDisplayPartial", '#teamDisplay');
}