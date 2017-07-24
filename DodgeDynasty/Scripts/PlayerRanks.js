var replaceElementId = "#allPlayerRanks";
var prRefreshPartial = "Draft/PlayerRanksPartial";

$(function () {
	loadPlayerRanksShared();
});

function updateDraftPickRows(pickInfo, yours) {
	$.each($(".ba-table tr[data-player-id=" + pickInfo.pid + "]"), function (ix, row) {
		$(row).addClass("ba-selected");
		$(row).addClass(pickInfo.ocss);
		if (yours) {
			$(row).addClass("you");
		}

		var colspan = 1;
		$(".pr-team", row).remove();
		if ($(".pr-pos", row).length > 0) {
			colspan = 2;
			$(".pr-pos", row).remove();
		}
		$(".ba-player-name", row).after('<td colspan="' + colspan + '">(' + pickInfo.oname + ', #' + pickInfo.pnum + ')</td>');
	});
}
