$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/DisplayPartial", '#draftDisplay');
	touchScrollDiv = ".draft-container";
});

function pageBroadcastDraftHandler(pickInfo) {
	updatePageWithDraftPickInfo(pickInfo, function (pickInfo) {
		var draftPick = $(".draft-pick[data-pick-num=" + pickInfo.pnum + "]");
		if (draftPick) {
			$(".player-nflteam", draftPick).text(pickInfo.team + "-");
			$(".player-pos", draftPick).text(pickInfo.pos);
			$(".player-name", draftPick).text(pickInfo.pname);
			$(draftPick).addClass("filled");
		}
	}, function () {
		callRefreshPage("Draft/DisplayPartial", '#draftDisplay');
	});
}