$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/DisplayPartial", '#draftDisplay');
	touchScrollDiv = ".draft-container";
});

function pageBroadcastDraftHandler(pickInfo) {
	updatePageWithDraftPickInfo(pickInfo, function (pickInfo) {
		var draftPick = $(".draft-pick[data-pick-num=" + pickInfo.pnum + "]");
		if (draftPick) {
			populateWithDraftPickInfo(draftPick, pickInfo, draftShowPosCol);
		}
	}, function () {
		callRefreshPage("Draft/DisplayPartial", '#draftDisplay');
	});
}