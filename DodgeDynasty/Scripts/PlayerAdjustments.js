var playerAdjWindow = 8;

$(function () {
	displayAdjustmentWindows();
	bindToggleWindowLinks();
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