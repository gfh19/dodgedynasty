var replaceElementId = "#bestAvailable";
var prRefreshPartial = "Draft/BestAvailablePartial";

$(function () {
	loadPlayerRanksShared();
});

function updateDraftPickRows(pickInfo) {
	var hqRows = $(".queue-table tr[data-player-id=" + pickInfo.pid + "]");
	if (hqRows.length > 0) {
		var hqRankNum = $("td[data-rank-num]", hqRows[0]).attr("data-rank-num");
		var hqRowsAfter = $(".queue-table td[data-rank-num]").filter(function () {
			return (parseInt($(this).attr("data-rank-num")) > 1)
				&& (parseInt($(this).attr("data-rank-num")) > parseInt(hqRankNum));
		});
		$.each($(hqRowsAfter), function (ix, rowAfter) {
			var newRankNum = parseInt($(rowAfter).attr("data-rank-num") - 1);
			$(rowAfter).attr("data-rank-num", newRankNum);
			$(rowAfter).text(newRankNum);
		});
		$(hqRows[0]).remove();
	}
	$.each($(".pr-table tr[data-player-id=" + pickInfo.pid + "]"), function (ix, row) {
		$(row).remove();
	});
}