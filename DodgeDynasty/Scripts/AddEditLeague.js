var leagueUserIds = null;

function initAddEditLeague() {
	displayLinks();
	bindAddOwnerLinks();
	bindRemoveOwnerLinks();
	bindSetOwnerSelects();
	bindSubmitLeague();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function bindSubmitLeague() {
	$(".submit-league").click(function (e) {
		e.preventDefault();
		$(".submit-league").first().focus();
		resetValidations();
		var addEditLeagueModel = getAddEditLeagueModel();
		var leagueFormValid = $('#leagueForm').valid()
		var ownersValid = validateAddEditLeagueModel(addEditLeagueModel);
		if (leagueFormValid && ownersValid) {
			updateAddEditLeagueModel(addEditLeagueModel);
		}
	});
}

function getAddEditLeagueModel() {
	var addEditLeagueModel = {};
	addEditLeagueModel.LeagueId = $(".league-id").val();
	addEditLeagueModel.LeagueName = $(".league-name").val();
	var leagueOwnerUsers = new Array();
	leagueUserIds = new Array();
	var ix = 0;
	$.each($(".league-owner-entry"), function (index, ownerUser) {
		var lo = {};
		lo.UserId = $("select option:selected", ownerUser).val();
		lo.TeamName = $(".lo-team-input", ownerUser).val();
		lo.IsActive = $(".lo-active-chkbx", ownerUser).val();

		leagueOwnerUsers.push(lo);
		leagueUserIds[ix++] = lo.UserId;
	});

	addEditLeagueModel.LeagueOwnerUsers = leagueOwnerUsers;
	return addEditLeagueModel;
}

function validateAddEditLeagueModel() {
	var isValid = true;
	leagueUserIds.sort();
	var blankOwner = $.inArray("", leagueUserIds);
	if (blankOwner > -1) {
		$(".blank-owner-msg").removeClass("hide-yo-wives");
		markInvalidOwnerId("");
		isValid = false;
	}
	for (var i = 0; i < (leagueUserIds.length - 1) ; i++) {
		if (leagueUserIds[i] != undefined && leagueUserIds[i] == leagueUserIds[i + 1]) {
			$(".dup-owner-msg").removeClass("hide-yo-wives");
			markInvalidOwnerId(leagueUserIds[i]);
			isValid = false;
		}
	}
	$.each($(".league-owner-entry .lo-team-input"), function (index, team) {
		if ($(team).val().trim() === "") {
			$(".blank-team-msg").removeClass("hide-yo-wives");
			$(team).addClass("invalid-border");
			isValid = false;
		}
	});
	return isValid;
}

function resetValidations() {
	$(".blank-owner-msg, .blank-team-msg, .dup-owner-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}