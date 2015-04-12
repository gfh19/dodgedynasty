var draftUserIds = null;

function initAddDraft() {
	displayLinks();
	bindAddOwnerLinks();
	bindRemoveOwnerLinks();
	bindSubmitDraft();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function bindSubmitDraft() {
	$(".submit-draft").click(function (e) {
		e.preventDefault();
		$(".submit-draft").first().focus();
		resetValidations();
		var addDraftModel = getAddDraftModel();
		var draftFormValid = $('#draftForm').valid()
		var ownersValid = validateAddDraftModel(addDraftModel);
		if (draftFormValid && ownersValid) {
			updateAddDraftModel(addDraftModel);
		}
	});
}

function getAddDraftModel() {
	var addDraftModel = $("#draftForm").serializeObject();
	var leagueOwnerUsers = new Array();
	draftUserIds = new Array();
	var ix = 0;
	$.each($(".league-owner-entry"), function (index, ownerUser) {
		var lo = {};
		lo.UserId = $("select option:selected", ownerUser).val();

		leagueOwnerUsers.push(lo);
		draftUserIds[ix++] = lo.UserId;
	});

	addDraftModel.DraftOwnerUsers = leagueOwnerUsers;
	return addDraftModel;
}

function validateAddDraftModel() {
	var isValid = true;
	draftUserIds.sort();
	var blankOwner = $.inArray("", draftUserIds);
	if (blankOwner > -1) {
		$(".blank-owner-msg").removeClass("hide-yo-wives");
		markInvalidOwnerId("");
		isValid = false;
	}
	for (var i = 0; i < (draftUserIds.length - 1) ; i++) {
		if (draftUserIds[i] != undefined && draftUserIds[i] == draftUserIds[i + 1]) {
			$(".dup-owner-msg").removeClass("hide-yo-wives");
			markInvalidOwnerId(draftUserIds[i]);
			isValid = false;
		}
	}
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
	$(".blank-owner-msg, .dup-owner-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}

function updateAddDraftModel(addDraftModel) {
	ajaxPost(addDraftModel, "Admin/AddDraft", function () {
		alert("League created");
	}, null, null, true);
}
