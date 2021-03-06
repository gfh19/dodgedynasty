﻿var adminMode = null;
var draftUserIds = null;

function initAddEditDraft() {
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
		var addEditDraftModel = getAddEditDraftModel();
		var draftFormValid = $('#addEditDraftForm').valid();
		var ownersValid = validateAddEditDraftModel(addEditDraftModel);
		if (draftFormValid && ownersValid) {
			updateAddEditDraftModel(addEditDraftModel);
		}
	});
}

function getAddEditDraftModel() {
	var addEditDraftModel = $("#addEditDraftForm").serializeObject();
	var draftOwnerUsers = new Array();
	draftUserIds = new Array();
	var ix = 0;
	$.each($(".league-owner-entry"), function (index, ownerUser) {
		var lo = {};
		lo.UserId = $("select option:selected", ownerUser).val();

		draftOwnerUsers.push(lo);
		draftUserIds[ix++] = lo.UserId;
	});

	addEditDraftModel.DraftOwnerUsers = draftOwnerUsers;
	return addEditDraftModel;
}

function validateAddEditDraftModel() {
	var isValid = true;
	draftUserIds.sort();
	var blankOwner = $.inArray("", draftUserIds);
	if (blankOwner > -1) {
		$(".blank-owner-msg").removeClass("hide-yo-wives");
		markInvalidId("");
		isValid = false;
	}
	for (var i = 0; i < (draftUserIds.length - 1) ; i++) {
		if (draftUserIds[i] != undefined && draftUserIds[i] == draftUserIds[i + 1]) {
			$(".dup-owner-msg").removeClass("hide-yo-wives");
			markInvalidId(draftUserIds[i]);
			isValid = false;
		}
	}
	return isValid;
}

function resetValidations() {
	$(".blank-owner-msg, .dup-owner-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}
