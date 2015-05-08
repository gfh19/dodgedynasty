var leagueUserIds = null;

function initAddEditLeague() {
	displayLinks();
	bindAddOwnerLinks();
	bindRemoveOwnerLinks();
	bindSetOwnerSelects();
	bindSubmitLeague();
	bindColorSelects();
	initColorSelects();
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
		lo.CssClass = $(".lo-color-select", ownerUser).val();
		lo.IsActive = $(".lo-active-chkbx", ownerUser).prop('checked');

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
		markInvalidId("");
		isValid = false;
	}
	for (var i = 0; i < (leagueUserIds.length - 1) ; i++) {
		if (leagueUserIds[i] != undefined && leagueUserIds[i] == leagueUserIds[i + 1]) {
			$(".dup-owner-msg").removeClass("hide-yo-wives");
			markInvalidId(leagueUserIds[i]);
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

function bindColorSelects() {
	var selects = $(".lo-color-select");
	$.each(selects, function (index, select) {
		bindColorSelect(select);
	});
}

function bindColorSelect(select) {
	$(select).change(function (e) {
		var loEntry = $(select).closest(".lo-selected-color");
		var color = $(select).val();
		var colorText = $("option:selected", select).text();
		var prevColorClass = $(select).attr("data-prev-color");
		var prevColorText = $(select).attr("data-prev-text");
		$(loEntry).removeClass();
		$(loEntry).addClass("lo-selected-color");
		$(loEntry).addClass(color);
		removeSelectedColor(color);
		addPreviousColor(prevColorClass, prevColorText);
		$(select).attr("data-prev-color", color);
		$(select).attr("data-prev-text", colorText);
	});
}

function removeSelectedColor(cssClass) {
	$.each($(".lo-color-select"), function (ix, select) {
		if ($(select).val() != cssClass && cssClass != "_none") {
			$("option[value='" + cssClass + "']", select).remove();
		}
	});
}

function addPreviousColor(prevColorClass, prevColorText) {
	$.each($(".lo-color-select"), function (ix, select) {
		var added = false;
		if ($("option[value='" + prevColorClass + "']", select).length == 0) {
			$.each($("option", select), function (i, option) {
				if (prevColorClass < $(option).val()) {
					var newOption = $("<option></option>")
						.attr("value", prevColorClass)
						.text(prevColorText)
					$(newOption).insertBefore($(option))
					added = true;
					return false;
				}
			});
			if (!added) {
				$(select).append($("<option></option>")
				.attr("value", prevColorClass)
				.text(prevColorText));
			}
		}
	});
}

function initColorSelects() {
	$.each($(".lo-color-select"), function (ix, select) {
		removeSelectedColor($(select).val());
	});
}

function bindNewOwnerMisc(entry) {
	bindColorSelect($(".lo-color-select", entry));
}