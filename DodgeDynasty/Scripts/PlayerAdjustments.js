var playerAdjWindow = 6;

function initPlayerAdjustments() {
	displayAdjustmentWindows();
	bindToggleWindowLinks();
	bindAddNewPlayerLink();
	bindEditPlayerLink();
	bindActivatePlayerLinks();
	bindInactivateAllPlayerButtons();
	bindDraftsRanksMoreLinks();
}

function displayAdjustmentWindows() {
	$("#pa-added-players tr:gt(" + playerAdjWindow + ")").hide();
	$("#pa-other-players tr:gt(" + playerAdjWindow + ")").hide();
}

function bindToggleWindowLinks() {
	$(".pa-toggle").unbind("click");
	$(".pa-toggle").click(function(e) {
		$("#pa-added-players tr:gt(" + playerAdjWindow + ")").toggle();
		$("#pa-other-players tr:gt(" + playerAdjWindow + ")").toggle();
	});
}

function bindAddNewPlayerLink() {
	$(".pa-add-player").click(function (e) {
		e.preventDefault();
		showAddNewPlayerDialog();
		setTruePlayerAutoComplete(null, "#add-plyr-tpid", "#add-plyr-fname", "#add-plyr-lname",
			"#add-plyr-pos", "#add-plyr-nfl", "#add-plyr-active", "#add-plyr-drafted");
		$("#add-plyr-fname").focus();
	});
	$(".pa-clear-add").click(clearAddPlayer);
}

function bindEditPlayerLink() {
	var links = $(".pa-edit-player");
	$.each(links, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			if ($(link).parents("tr[data-player-id]").length > 0) {
				var playerId = $(link).parents("tr[data-player-id]").attr("data-player-id");
				var playerHint = $(playerHints).where("id", playerId);
				if (playerHint.length > 0) {
					setSelectedPlayer("#edit-plyr-id", "#edit-plyr-tpid", "#edit-plyr-fname", "#edit-plyr-lname",
						"#edit-plyr-pos", "#edit-plyr-nfl", "#edit-plyr-active", "#edit-plyr-drafted", playerHint[0]);
				}
				else {
					clearEditPlayer();
				}
			}
			else {
				clearEditPlayer();
			}
			showEditPlayerDialog();
			setTruePlayerAutoComplete("#edit-plyr-id", "#edit-plyr-tpid", "#edit-plyr-fname", "#edit-plyr-lname",
				"#edit-plyr-pos", "#edit-plyr-nfl", "#edit-plyr-active", "#edit-plyr-drafted");
			$("#edit-plyr-fname").focus();
		});
	});
}

function bindActivatePlayerLinks() {
	var links = $(".pa-activate-player, .pa-deactivate-player");
	$.each(links, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			if ($(link).parents("tr[data-player-id]").length > 0) {
				var playerId = $(link).parents("tr[data-player-id]").attr("data-player-id");
				setPlayerStatus(playerId, $(link).hasClass("pa-activate-player"));
			}
		});
	});
}

function bindInactivateAllPlayerButtons() {
	$(".activate-defs").click(function (e) {
		e.preventDefault();
		showConfirmDialog("Are you sure you want to Inactivate all but 32 DEFs? <br/><br/>(This is irreversible)", null, function () {
			ajaxPost({ playerGroup: "def" }, "Admin/InactivatePlayers", function () {
				window.location.reload();
			});
		});
	});
	$(".inactivate-all").click(function (e) {
		e.preventDefault();
		showConfirmDialog("Are you sure you want to Inactivate ALL PLAYERS? <br/><br/>(This is irreversible)", null, function () {
			ajaxPost({ playerGroup: "all" }, "Admin/InactivatePlayers", function () {
				window.location.reload();
			});
		});
	});
}

function setPlayerStatus(id, activate) {
	ajaxPost({ PlayerId: id, IsActive: activate }, "Admin/SetPlayerStatus", function () {
		window.location.reload();
	});
}

function clearAddPlayer() {
	$("#add-plyr-tpid").val("");
	$("#add-plyr-fname").val("");
	$("#add-plyr-lname").val("");
	$("#add-plyr-pos").val("");
	$("#add-plyr-nfl").val("");
	$("#add-plyr-active").prop("checked", true);
	$("#add-plyr-drafted").prop("checked", true);
}

function clearEditPlayer() {
	$("#edit-plyr-id").val("");
	$("#edit-plyr-tpid").val("");
	$("#edit-plyr-fname").val("");
	$("#edit-plyr-lname").val("");
	$("#edit-plyr-pos").val("");
	$("#edit-plyr-nfl").val("");
	$("#edit-plyr-active").prop("checked", true);
	$("#edit-plyr-drafted").prop("checked", true);
}

function getNewPlayer() {
	var player = {};
	player.TruePlayerId = $("#add-plyr-tpid").val();
	player.FirstName = $("#add-plyr-fname").val();
	player.LastName = $("#add-plyr-lname").val();
	player.Position = $("#add-plyr-pos").val();
	player.NFLTeam = $("#add-plyr-nfl").val();
	player.IsActive = $("#add-plyr-active").prop('checked');
	player.IsDrafted = $("#add-plyr-drafted").prop('checked');
	return player;
}

function getEditPlayer() {
	var player = {};
	player.PlayerId = $("#edit-plyr-id").val();
	player.TruePlayerId = $("#edit-plyr-tpid").val();
	player.FirstName = $("#edit-plyr-fname").val();
	player.LastName = $("#edit-plyr-lname").val();
	player.Position = $("#edit-plyr-pos").val();
	player.NFLTeam = $("#edit-plyr-nfl").val();
	player.IsActive = $("#edit-plyr-active").prop('checked');
	player.IsDrafted = $("#edit-plyr-drafted").prop('checked');
	return player;
}

function addNewPlayer(player) {
	ajaxPost(player, "Admin/AddNewPlayer", function () {
		window.location.reload();
	});
}

function editPlayer(player) {
	ajaxPost(player, "Admin/EditPlayer", function () {
		window.location.reload();
	});
}

function showAddNewPlayerDialog() {
	showPlayerDialog("#addNewPlayerDialog", "#addNewPlayerForm", "Add New Player", getNewPlayer, addNewPlayer);
}

function showEditPlayerDialog() {
	showPlayerDialog("#editPlayerDialog", "#editPlayerForm", "Edit Player", getEditPlayer, editPlayer);
}

function showPlayerDialog(dialogId, formId, header, getPlayerFn, playerFn) {
	$(dialogId).dialog({
		resizable: false,
		height: 'auto',
		width: '295px',
		modal: true,
		dialogClass: "visible-dialog",
		buttons: [
					{
						text: header, click: function () {
							var player = getPlayerFn();
							if ($(formId).valid()) {
								$(this).dialog("close");
								playerFn(player);
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

function setTruePlayerAutoComplete(pid, tpid, fname, lname, pos, nfl, active, drafted) {
	pid = pid || "";
	$(fname).autocomplete({
		source: function (request, response) {
			var filteredArray = $.map(playerHints, function (item) {
				var response = null;
				var nameParts = [item.firstName, item.lastName, item.firstName + ' ' + item.lastName];
				$.each(nameParts, function (index, elem) {
					if (formatAutoCompName(elem).match("^" + formatAutoCompName(request.term))) {
						response = item;
					}
				});
				return response;
			});
			response(filteredArray);
		},
		select: function (event, ui) {
			setSelectedPlayer(pid, tpid, fname, lname, pos, nfl, active, drafted, ui.item);
			return false;
		}
	});
};

function setSelectedPlayer(pid, tpid, fname, lname, pos, nfl, active, drafted, plyrHint) {
	$(pid).val(plyrHint.id);
	$(tpid).val(plyrHint.tpid);
	$(fname).val(plyrHint.firstName);
	$(lname).val(plyrHint.lastName);
	$(pos).val(plyrHint.pos);
	$(nfl).val(plyrHint.nflTeamDisplay);
	$(active).prop("checked", toBool(plyrHint.active));
	$(drafted).prop("checked", toBool(plyrHint.drafted));
	setTimeout(function () { $("#inputSubmit").focus(); }, 0);
}
