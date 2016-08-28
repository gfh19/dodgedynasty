var autoImportHints = [];

function initRankAdjustments() {
	removeHeaderFreeze();
	changeViewport(0.25, 3.0);
	bindAddEditRankLinks();
	bindAutoImportLinks();
	bindDraftsRanksMoreLinks();
}

function bindAddEditRankLinks() {
	$(".ra-add-rank").click(function (e) {
		e.preventDefault();
		showAddNewRankDialog();
		$("#raf-rank-rname").focus();
	});
	var editLinks = $(".ra-edit-details");
	$.each(editLinks, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			var rankRow = $(link).parents("tr");
			var rankId = $(rankRow).attr("data-rank-id");
			setEditRankValues(rankRow);
			showEditRankDialog(rankId);
			$("#raf-rank-rname").focus();
		});
	});
	$("#raf-rank-import").change(function (e) {
		var autoImportId = $("#raf-rank-import").val();
		var autoImportRanks = $(autoImportHints).filter(function (el) {
			return $(this)[0].id == autoImportId;
		});
		if (autoImportRanks.length > 0) {
			var rank = autoImportRanks[0];
			$("#raf-rank-rname").val(rank.rankName);
			$("#raf-rank-url").val(rank.importUrl);
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

function getEditRank(rankId) {
	var rank = getNewRank();
	rank.RankId = rankId;
	return rank;
}

function setEditRankValues(rankRow) {
	$("#raf-rank-rname").val($(".ra-edit-rname", rankRow).val());
	$("#raf-rank-year").val($(".ra-edit-year", rankRow).val());
	$("#raf-rank-url").val($(".ra-edit-url", rankRow).val());
	$("#raf-rank-draft-id").val($(".ra-edit-draft-id", rankRow).val());
	$("#raf-rank-primary").prop("checked", toBool($(".ra-edit-primary", rankRow).val()));
	$("#raf-rank-import").val($(".ra-edit-import", rankRow).val());
}

function addNewRank(rank) {
	addWaitCursor();
	ajaxPost(rank, "Admin/AddNewRank", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function editRank(rank) {
	addWaitCursor();
	ajaxPost(rank, "Admin/EditRank", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function confirmAutoImportRank(rankId, rankName) {
	addWaitCursor();
	ajaxPost({ RankId: rankId, Confirmed: false }, "Admin/AutoImportRank", function (data) {
		removeWaitCursor();
		var response = JSON.parse(data);
		if (response.error) {
			var text = response.error + "<br/><br/>" + response.stack;
			showAlertDialog(text, "Error");
		}
		else {
			var text = "First Player:<br/>" + response.first + "<br/><br/>"
					+ "Last Player:<br/>" + response.last + "<br/><br/>"
					+ "Player Count: " + response.count + "<br/><br/>"
					+ getMaxPlayerCountExceededText(response)
					+ "Do you want to import <br/>'" + rankName + "' now?";
			showConfirmDialog(text, null, function () { autoImportRank(rankId); });
		}
	}, removeWaitCursor);
}

function getMaxPlayerCountExceededText(response) {
	var text = "";
	if (response.count > response.max) {
		text = "(NOTE: Exceeded " + response.max + " Player Max)<br/>"
			+ 'Import top:  <input type="text" id="importMaxRowCount" name="importMaxRowCount" value="'+response.max+'" maxlength="4" />'
			+ "<br/><br/>"
	}
	return text;
}

function autoImportRank(rankId) {
	var maxCount = null;
	if ($("#importMaxRowCount").length > 0) {
		maxCount = $("#importMaxRowCount").val();
	}
	addWaitCursor();
	ajaxPost({ RankId: rankId, Confirmed: true, MaxCount: maxCount }, "Admin/AutoImportRank", function () {
		window.location.reload();
	}, removeWaitCursor);
}

function showAddNewRankDialog() {
	showRankDialog("#raRankDialog", "#raRankForm", "Add New Rank", getNewRank, addNewRank);
}

function showEditRankDialog(rankId) {
	showRankDialog("#raRankDialog", "#raRankForm", "Edit Rank", function () { return getEditRank(rankId); }, editRank);
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
