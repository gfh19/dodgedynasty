﻿
function displayLinks() {
	var firstOwnerUser = $(".league-owners").find(".league-owner-entry:first");
	$(".league-remove-owner", firstOwnerUser).addClass("hide-yo-husbands-too");
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