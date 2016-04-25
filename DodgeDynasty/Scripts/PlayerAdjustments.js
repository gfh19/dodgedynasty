var playerAdjWindow = 8;

$(function () {
	displayAdjustmentWindows();
	bindToggleWindowLinks();
	bindAddNewPlayerLink();
});

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
		//setPlayerAutoComplete("#add-plyr-fname", "#add-plyr-lname", "#add-plyr-pos", "#add-plyr-nfl");
	});
}

function showAddNewPlayerDialog() {
	$("#addNewPlayerDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '280px',
		modal: true,
		buttons: [
					{
						text: "Add New Player", click: function () {
							//resetValidations();
							//var rankSetupModel = getRankSetupModel();
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
