$(function () {
	setPickTimer(true);
	callRefreshPageWithPickTimer("Draft/RankingsListPartial", "#rankingsList");
});

function broadcastDraft() {
	callRefreshPage("Draft/RankingsListPartial", "#rankingsList");
}

function initRankingsList() {
	bindRankLinks();
	bindAddRankDialog();
}

function bindRankLinks() {
	$.each($(".rank-link"), function (ix, link) {
		$(link).click(function () {
			var options = getCookieOptions();
			options.RankId = $(link).attr("data-rank-id");
			setCookieOptions(options);
		});
	});
}

function bindAddRankDialog() {
	$(".add-rank").click(function (e) {
		e.preventDefault();
		showAddRankDialog();
	});
}

function showAddRankDialog() {
	$("#addRankDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '275px',
		modal: true,
		buttons: [
					{
						id: "btnCopyFromRank",
						text: "Copy From Ranking", click: function () {
							var copyFromRankId = $("select option:selected", $("#addRankDialog")).val();
							var copyCount = $("#copyRowCount", $("#addRankDialog")).val();
							$("body").css("cursor", "progress");
							location.href = baseURL + "Rank/AddRank?rankId=" + copyFromRankId + "&copyCount=" + copyCount;
						}
					},
					{
						id: "btnCreateNew",
						text: "Create New", click: function () {
							$("body").css("cursor", "progress");
							location.href = baseURL + "Rank/AddRank";
						}
					},
				],
		open: function () {
			$("#btnCopyFromRank").focus();
		}
	});
}
