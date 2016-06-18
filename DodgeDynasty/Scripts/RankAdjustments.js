
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
}

function bindAutoImportLinks() {
	var links = $(".ra-auto-import");
	$.each(links, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			if ($(link).parents("tr[data-rank-id]").length > 0) {
				var rankId = $(link).parents("tr[data-rank-id]").attr("data-rank-id");
				autoImportRank(rankId, false);
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
	$("#edit-plyr-id").val("");
	$("#edit-plyr-tpid").val("");
	$("#edit-plyr-fname").val("");
	$("#edit-plyr-lname").val("");
	$("#edit-plyr-pos").val("");
	$("#edit-plyr-nfl").val("");
	$("#edit-plyr-active").prop("checked", true);
}

function getNewRank() {
	var rank = {};
	rank.RankName = $("#raf-rank-rname").val();
	rank.Year = $("#raf-rank-year").val();
	rank.Url = $("#raf-rank-url").val();
	rank.DraftId = $("#raf-rank-draft-id").val();
	rank.PrimaryDraftRanking = $("#raf-rank-primary").prop('checked');
	rank.AutoImport = $("#raf-rank-import").prop('checked');
	return rank;
}

function getEditRank() {
	var rank = {};
	rank.rankId = $("#edit-plyr-id").val();
	rank.TruerankId = $("#edit-plyr-tpid").val();
	rank.FirstName = $("#edit-plyr-fname").val();
	rank.LastName = $("#edit-plyr-lname").val();
	rank.Position = $("#edit-plyr-pos").val();
	rank.NFLTeam = $("#edit-plyr-nfl").val();
	rank.IsActive = $("#edit-plyr-active").prop('checked');
	return rank;
}

function addNewRank(rank) {
	ajaxPost(rank, "Admin/AddNewRank", function () {
		window.location.reload();
	});
}

function editRank(rank) {
	ajaxPost(rank, "Admin/EditPlayer", function () {
		window.location.reload();
	});
}

function autoImportRank(rankId, confirmed) {
	ajaxPost({RankId: rankId, Confirmed: true}, "Admin/AutoImportRank", function () {
		window.location.reload();
	});
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
