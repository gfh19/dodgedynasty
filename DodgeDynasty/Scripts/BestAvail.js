var replaceElementId = "#bestAvailable";
var prRefreshPartial = "Draft/BestAvailablePartial";

$(function () {
	loadPlayerRanksShared();
});

function updateDraftPickRows(pickInfo) {
	$.each($(".pr-table tr[data-player-id=" + pickInfo.pid + "]"), function (ix, row) {
		$(row).remove();
	});
}