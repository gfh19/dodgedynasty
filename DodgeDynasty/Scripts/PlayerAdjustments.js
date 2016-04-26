var playerAdjWindow = 8;

function initPlayerAdjustments() {
	displayAdjustmentWindows();
	bindToggleWindowLinks();
	bindAddNewPlayerLink();
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
		setTruePlayerAutoComplete("#add-plyr-tpid", "#add-plyr-fname", "#add-plyr-lname", "#add-plyr-pos", "#add-plyr-nfl");
		$("#add-plyr-fname").focus();
	});
}

function getNewPlayer() {
	function getNewPlayer() {
		var player = {};
		player.TruePlayerId = $("#add-plyr-tpid").val();
		player.FirstName = $("#add-plyr-fname").val();
		player.LastName = $("#add-plyr-lname").val();
		player.Position = $("#add-plyr-pos").val();
		player.NFLTeam = $("#add-plyr-nfl").val();
		player.IsActive = $("#add-plyr-active").prop('checked');
		return player;
	}
}

function showAddNewPlayerDialog() {
	$("#addNewPlayerDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '295px',
		modal: true,
		dialogClass: "pa-player-dialog",
		buttons: [
					{
						text: "Add New Player", click: function () {
							var player = getNewPlayer();
							if ($("#addNewPlayerForm").valid()) {
								$(this).dialog("close");
								addNewPlayer(player);
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

function setTruePlayerAutoComplete(tpid, fname, lname, pos, nfl) {
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
			$(tpid).val(ui.item.tpid);
			$(nfl).val(ui.item.nflTeamDisplay);
			$(pos).val(ui.item.pos);
			$(lname).val(ui.item.lastName);
			$(fname).val(ui.item.firstName);
			setTimeout(function () { $("#inputSubmit").focus(); }, 0);
			return false;
		}
	});
};
