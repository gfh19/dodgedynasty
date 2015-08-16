$(function () {
	bindViewWinnersCheckbox();
});

function bindViewWinnersCheckbox() {
	$("#dh-view-chkbox").click(function () {
		if ($("#dh-view-chkbox").is(':checked')) {
			$(".dh-header, .dh-winner, .dh-runner-up").addClass("dh-view-winners");
			$("#draftHistory .dh-league-section").addClass("dh-mobile-view");
		}
		else {
			$(".dh-header, .dh-winner, .dh-runner-up").removeClass("dh-view-winners");
			$("#draftHistory .dh-league-section").removeClass("dh-mobile-view");
		}
	});
}