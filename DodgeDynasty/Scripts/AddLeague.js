var leagueUserIds;

function initAddLeague() {
	displayLinks();
	bindAddOwnerLinks();
	bindRemoveOwnerLinks();
	bindSubmitLeague();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}

function displayLinks() {
	var firstOwnerUser = $(".league-owners").find(".league-owner-entry:first");
	$(".league-remove-owner", firstOwnerUser).addClass("hide-yo-husbands-too");
	//$("input[type=text]").first().focus();
}

function bindAddOwnerLinks() {
	var links = $(".league-add-owner");
	$.each(links, function (index, link) {
		bindAddOwnerLink(link);
	});
}

function bindAddOwnerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var leagueOwnerEntry = $(link).closest('.league-owner-entry');
		var newLeagueOwnerEntry = copyLeagueOwnerEntry();
		$(newLeagueOwnerEntry).insertAfter(leagueOwnerEntry);

		bindNewEntryLinks(newLeagueOwnerEntry);
		$("select", newLeagueOwnerEntry).focus();
	});
}

function bindRemoveOwnerLinks() {
	var links = $(".league-remove-owner");
	$.each(links, function (index, link) {
		bindRemoveOwnerLink(link);
	});
}

function bindRemoveOwnerLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var leagueOwnerEntry = $(link).closest('.league-owner-entry');
		$(leagueOwnerEntry).remove();
	});
}

function bindNewEntryLinks(entry) {
	bindAddOwnerLink($(".league-add-owner", $(entry)));
	bindRemoveOwnerLink($(".league-remove-owner", $(entry)));
}

function copyLeagueOwnerEntry() {
	var copyLOEntry = $('.copy-lo-entry');
	var newLeagueOwnerEntry = $(copyLOEntry).clone();
	$(newLeagueOwnerEntry).addClass("league-owner-entry");
	$(newLeagueOwnerEntry).removeClass("copy-lo-entry");
	$(newLeagueOwnerEntry).removeClass("hide-yo-wives");
	return newLeagueOwnerEntry;
}

function bindSubmitLeague() {
	$(".submit-league").click(function (e) {
		e.preventDefault();
		$(".submit-league").first().focus();
		resetValidations();
		var addLeagueModel = getAddLeagueModel();
		if (validateAddLeagueModel(addLeagueModel)) {
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

function validateAddLeagueModel(addLeagueModel) {
	//TODO:  Validate inputs
	return true;
}

function resetValidations() {
	//TODO:  Reset validations
};

function updateAddLeagueModel(addLeagueModel) {
	ajaxPost(addLeagueModel, "Admin/AddLeague", function () {
		alert("League created");
	}, null, null, true);
}
