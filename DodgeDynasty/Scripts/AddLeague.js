var leagueUserIds = null;

function initAddLeague() {
	displayLinks();
	bindAddOwnerLinks();
	bindRemoveOwnerLinks();
	bindSelectOwner();
	bindSubmitLeague();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function bindSelectOwner() {
	var loSelects = $(".lo-select");
	$.each(loSelects, function (index, loSelect) {
		$(loSelect).change(function (e) {
			var loEntry = $(loSelect).closest(".league-owner-entry");
			$(".lo-team-input", loEntry).val("Team " + $("option:selected", loSelect).text());
		});
	})
}

function bindSubmitLeague() {
	$(".submit-league").click(function (e) {
		e.preventDefault();
		$(".submit-league").first().focus();
		resetValidations();
		var addLeagueModel = getAddLeagueModel();
		var leagueFormValid = $('#leagueForm').valid()
		var ownersValid = validateAddLeagueModel(addLeagueModel);
		if (leagueFormValid && ownersValid) {
			updateAddLeagueModel(addLeagueModel);
		}
	});
}

function getAddLeagueModel() {
	var addLeagueModel = {};
	addLeagueModel.LeagueName = $(".league-name").val();
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

	addLeagueModel.LeagueOwnerUsers = leagueOwnerUsers;
	return addLeagueModel;
}

function validateAddLeagueModel() {
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

function markInvalidOwnerId(ownerId) {
	if (ownerId === "") {
		//Find any blank spans
		$.each($("select"), function (index, owner) {
			if ($(owner).val() === "") {
				$(owner).addClass("invalid-border");
			}
		});
	}
	else {
		//Find any matching selected options
		var invalidEntries = $("select option:selected[value=" + ownerId + "]").closest("select");
		$(invalidEntries).addClass("invalid-border");
	}
}

function resetValidations() {
	$(".blank-owner-msg, .blank-team-msg, .dup-owner-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}

function updateAddLeagueModel(addLeagueModel) {
	ajaxPost(addLeagueModel, "Admin/AddLeague", function (data) {
		var response = JSON.parse(data);
		location.href = baseURL + "Admin/AddDraft/" + response.leagueId;
	}, null, null, true);
}
