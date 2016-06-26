var autoImportHints = [];

function initRankAdjustments() {
	bindAddNewRankLink();
	bindAutoImportLinks();
}

function bindAddNewRankLink() {
	$(".ra-add-rank").click(function (e) {
		e.preventDefault();
		showAddNewRankDialog();
		$("#add-rank-name").focus();
	});
	$(".ra-clear-add").click(clearAddRank);
	$("#raf-rank-import").change(function (e) {
		var autoImportId = $("#raf-rank-import").val();
		var autoImportRanks = $(autoImportHints).filter(function (el) {
			return $(this)[0].id == autoImportId;
		});
		if (autoImportRanks.length > 0) {
			var rank = autoImportRanks[0];
			$("#raf-rank-rname").val(rank.rankName);
			$("#raf-rank-url").val(rank.defaultUrl);
		}
	});
}

function bindAutoImportLinks() {
	var links = $(".ra-auto-import");
	$.each(links, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			if ($(link).parents("tr[data-rank-id]").length > 0) {
				var rankId = $(link).parents("tr[data-rank-id]").attr("data-rank-id");
				confirmAutoImportRank(rankId, $("td", $(link).parents("tr[data-rank-id]")).first().text());
			}
		});
	});
}

function clearAddRank() {
	$("#add-rank-tpid").val("");
	$("#add-rank-fname").val("");
	$("#add-rank-lname").val("");
	$("#add-rank-pos").val("");
	$("#add-rank-nfl").val("");
	$("#add-rank-active").prop("checked", true);
}

function clearEditRank() {
	//$("#edit-plyr-id").val("");
	//$("#edit-plyr-tpid").val("");
	//$("#edit-plyr-fname").val("");
	//$("#edit-plyr-lname").val("");
	//$("#edit-plyr-pos").val("");
	//$("#edit-plyr-nfl").val("");
	//$("#edit-plyr-active").prop("checked", true);
}

function getNewRank() {
	var rank = {};
	rank.RankName = $("#raf-rank-rname").val();
	rank.Year = $("#raf-rank-year").val();
	rank.Url = $("#raf-rank-url").val();
	rank.DraftId = $("#raf-rank-draft-id").val();
	rank.PrimaryDraftRanking = $("#raf-rank-primary").prop('checked');
	rank.AutoImportId = $("#raf-rank-import").val();
	return rank;
}

function getEditRank() {
	var rank = {};
	return rank;
}

function addNewRank(rank) {
	addWaitCursor();
	ajaxPost(rank, "Admin/AddNewRank", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function editRank(rank) {
	addWaitCursor();
	ajaxPost(rank, "Admin/EditPlayer", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function confirmAutoImportRank(rankId, rankName) {
	addWaitCursor();
	ajaxPost({ RankId: rankId, Confirmed: false }, "Admin/AutoImportRank", function (data) {
		removeWaitCursor();
		var response = JSON.parse(data);
		if (response.error) {
			showAlertDialog("Error:  " + response.error);
		}
		else {
			var text = "First Player:<br/>" + response.first + "<br/><br/>"
					+ "Last Player:<br/>" + response.last + "<br/><br/>"
					+ "Player Count: " + response.count + "<br/><br/>"
					+ "Do you want to import <br/>'" + rankName + "' now?";
			showConfirmDialog(text, function () { autoImportRank(rankId); });
		}
	}, removeWaitCursor);
}

function autoImportRank(rankId) {
	addWaitCursor();
	ajaxPost({ RankId: rankId, Confirmed: true }, "Admin/AutoImportRank", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function showAddNewRankDialog() {
	showRankDialog("#raRankDialog", "#raRankForm", "Add New Rank", getNewRank, addNewRank);
}

function showEditRankDialog() {
	showRankDialog("#raRankDialog", "#raRankForm", "Edit Rank", getEditRank, editRank);
}

function showRankDialog(dialogId, formId, header, getRankFn, rankFn) {
	$(dialogId).dialog({
		resizable: false,
		height: 'auto',
		width: '452px',
		modal: true,
		dialogClass: "visible-dialog",
		buttons: [
					{
						text: header, click: function () {
							var player = getRankFn();
							if ($(formId).valid()) {
								$(this).dialog("close");
								rankFn(player);
							}
						}
					},
					{
						text: "Cancel", click: function () {
							$(this).dialog("close");
						}
					},
		]
	});
}
