var ownerHints = [];

 $(function () {
	bindActionLinks();
});

function bindActionLinks() {
	bindAddRoundLinks();
	bindDeleteRoundLinks();
	bindReverseRoundLinks();
	bindAddPickLinks();
	bindDeletePickLinks();
	bindSubmitDraftPicks();
	bindOwnerSelects($("select"));
	bindDeletePlayers();
};

function bindAddRoundLinks() {
	var links = $(".add-round-right");
	$.each(links, function (index, link) {
		bindAddRoundLink(link);
	});
};

function bindAddRoundLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var roundPicks = $(link).closest('.round-picks');
		var round = $(link).closest('.round');
		var roundNum = parseInt(round.attr("data-round-num"));
		var newRoundPicks = $(roundPicks).clone();
		changeRoundNums(roundNum, 1);
		var maxPickNum = getMaxRoundPickNum(roundPicks);
		var roundPickCount = $('.pick', roundPicks).length;
		changeAllPickNums(maxPickNum, roundPickCount);

		addNewRoundPicksAfter(newRoundPicks, roundPicks, roundPickCount, roundNum);
	});
};

function addNewRoundPicksAfter(newRoundPicks, roundPicks, roundPickCount, roundNum) {
	$(newRoundPicks).insertAfter(roundPicks);
	changePickNums($(".pick", newRoundPicks), 0, roundPickCount);
	setNewRoundNum($(".round", newRoundPicks), roundNum + 1);
	clearNewRoundPickAttributes(newRoundPicks);
	var newAddRoundLink = $(".add-round-right", newRoundPicks);
	var newDeleteRoundLink = $(".delete-round-right", newRoundPicks);
	var newReverseRoundLink = $(".reverse-round", newRoundPicks);
	var newAddPickLinks = $(".add-pick-right", newRoundPicks);
	var newDeletePickLinks = $(".delete-pick-right", newRoundPicks);
	bindAddRoundLink(newAddRoundLink);
	bindDeleteRoundLink(newDeleteRoundLink);
	bindReverseRoundLink(newReverseRoundLink);
	$.each(newAddPickLinks, function (index, link) {
		bindAddPickLink(link);
	});
	$.each(newDeletePickLinks, function (index, link) {
		bindDeletePickLink(link);
	});
	bindOwnerSelects($("select", newRoundPicks))
}

function bindDeleteRoundLinks() {
	var links = $(".delete-round-right");
	$.each(links, function (index, link) {
		bindDeleteRoundLink(link);
	});
};

function bindDeleteRoundLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var roundPicks = $(link).closest('.round-picks');
		var round = $(link).closest('.round');
		var roundNum = parseInt(round.attr("data-round-num"));

		changeRoundNums(roundNum, -1);
		var maxPickNum = getMaxRoundPickNum(roundPicks);
		var roundPickCount = $('.pick', roundPicks).length;
		changeAllPickNums(maxPickNum, -roundPickCount);
		$(roundPicks).remove();
	});
}

function bindReverseRoundLinks() {
	var links = $(".reverse-round");
	$.each(links, function (index, link) {
		bindReverseRoundLink(link);
	});
}

function bindReverseRoundLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var roundPicks = $(link).closest('.round-picks');
		var picks = $(".pick", roundPicks);
		var origPickIx = 0;
		for (var i = picks.length - 1; i >= 0; i--) {
			var origPick = picks[origPickIx++];
			var newPick = $(picks[i]).clone();
			$(newPick).attr("data-pick-id", $(origPick).attr("data-pick-id"));
			$(newPick).attr("data-pick-num", $(origPick).attr("data-pick-num"));
			$(".pick-num", newPick).text($(".pick-num", origPick).text());
			$(roundPicks).append(newPick);
		}
		$(picks).remove();
	});
}

function bindAddPickLinks() {
	var links = $(".add-pick-right");
	$.each(links, function (index, link) {
		bindAddPickLink(link);
	});
};

function bindAddPickLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var pick = $(link).closest('.pick');
		var pickNum = parseInt(pick.attr("data-pick-num"));
		var newPick = $(pick).clone();
		changeAllPickNums(pickNum, 1);
		$(newPick).insertAfter(pick);
		setNewPickNum(newPick, pickNum + 1);
		clearNewPickAttributes(newPick);
		if ($(".delete-pick-right", newPick).hasClass("hide-yo-husbands-too")) {
			$(".delete-pick-right", newPick).removeClass("hide-yo-husbands-too");
			if (pickNum != 1) {
				$(".delete-pick-right", pick).removeClass("hide-yo-husbands-too");
			}
		}
		bindAddPickLink($(".add-pick-right", newPick));
		bindDeletePickLink($(".delete-pick-right", newPick));
		bindOwnerSelects($("select", newPick))
	});
};

function bindDeletePickLinks() {
	var links = $(".delete-pick-right");
	$.each(links, function (index, link) {
		bindDeletePickLink(link);
	});
};

function bindDeletePickLink(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var pick = $(link).closest('.pick');
		var pickNum = parseInt(pick.attr("data-pick-num"));
		changeAllPickNums(pickNum, -1);
		var roundPicks = $(pick).closest('.round-picks');
		$(pick).remove();
		if ($('.pick', roundPicks).length == 1) {
			$(".delete-pick-right", $('.pick', roundPicks)).addClass("hide-yo-husbands-too");
		}
	});
};

function bindOwnerSelects(selects) {
	$(selects).on('change', function (e) {
		var optionSelected = $("option:selected", this);
		var valueSelected = this.value;
		$.each($("option", $(this)), function (ix, option) {
			$(option).removeAttr("selected");
		});
		$(optionSelected).attr("selected", "selected");
	});
}

function changeAllPickNums(pickNum, offset) {
	var picks = $(".pick");
	changePickNums(picks, pickNum, offset);
};

function changePickNums(picks, pickNum, offset) {
	$.each(picks, function (index, pick) {
		if (parseInt($(pick).attr("data-pick-num")) > pickNum) {
			setNewPickNum(pick, parseInt($(pick).attr("data-pick-num")) + offset);
		}
	});
};

function setNewPickNum(pick, newPickNum) {
	$(pick).attr("data-pick-num", newPickNum);
	$(".pick-num", pick).text(newPickNum);
};

function clearNewPickAttributes(pick) {
	$(pick).attr("data-pick-id", 0);
	var pickSelect = $(".pick-owner", pick);
	pickSelect.val("");
	$("option", pickSelect).removeAttr("selected");
	$(".picked-player", pick).remove();
};

function changeRoundNums(roundNum, offset) {
	var rounds = $(".round");
	$.each(rounds, function (index, round) {
		if (parseInt($(round).attr("data-round-num")) > roundNum) {
			setNewRoundNum(round, parseInt($(round).attr("data-round-num")) + offset);
		}
	});
};

function setNewRoundNum(round, newRoundNum) {
	$(round).attr("data-round-num", newRoundNum);
	$(".round-num", round).text(newRoundNum);
};

function getMaxRoundPickNum(roundPicks) {
	var picks = $(".pick", roundPicks);
	var maxPickNum = 0;
	$.each(picks, function (index, pick) {
		var pickNum = parseInt($(pick).attr("data-pick-num"));
		maxPickNum = pickNum > maxPickNum ? pickNum : maxPickNum;
	});
	return maxPickNum;
};

function clearNewRoundPickAttributes(newRoundPicks) {
	var picks = $(".pick", newRoundPicks);
	$.each(picks, function (index, pick) {
		$(pick).attr("data-pick-id", 0);
		$(".delete-pick-right", newRoundPicks).removeClass("hide-yo-husbands-too");
		$(".picked-player", pick).remove();
	});
	$(".delete-round-right", newRoundPicks).removeClass("hide-yo-wives");
}

function bindSubmitDraftPicks() {
	$(".submitDraftPicks").click(function () {
		resetValidations();
		var draftPicksModel = {};
		draftPicksModel.DraftId = $("#setupDraft").attr("data-draft-id");
		var draftPicks = new Array();
		var ownerIds = new Array();
		$.each($(".round-picks"), function (index, roundPicks) {
			var roundNum = parseInt($(".round", roundPicks).attr("data-round-num"));
			$.each($(".pick", roundPicks), function (index, pick) {
				var draftPickId = parseInt($(pick).attr("data-pick-id"));
				var pickNum = parseInt($(pick).attr("data-pick-num"));
				var ownerId = $("select option:selected", pick).val();
				var playerId = null;
				var pickedPlayer = $(".picked-player", pick);
				if (pickedPlayer.length > 0) {
					playerId = $(pickedPlayer).attr("data-player-id");
				}
				draftPicks.push({
					DraftPickId: draftPickId,
					DraftId: draftPicksModel.DraftId,
					PickNum: pickNum,
					RoundNum: roundNum,
					OwnerId: ownerId,
					PlayerId: playerId
				});
				ownerIds.push(ownerId);
			});
		});
		draftPicksModel.DraftPicks = draftPicks;
		if (validateDraftPicksModel(ownerIds)) {
			ajaxPost(draftPicksModel, "Admin/SetupDraft", function (response) {
				$("#setupDraftForm").submit();
			}, null, null, true);
		}
	});
};

function bindDeletePlayers() {
	var links = $(".delete-picked-player");
	$.each(links, function (index, link) {
		bindDeletePlayer(link);
	});
}

function bindDeletePlayer(link) {
	$(link).click(function (e) {
		e.preventDefault();
		var player = $(link).closest('.picked-player');
		var playerId = parseInt(player.attr("data-player-id"));
		//popup confirmation?
		$(player).remove();
	});
};

function validateDraftPicksModel(ownerIds) {
	var isValid = true;
	var blankOwner = $.inArray("", ownerIds);
	if (blankOwner > -1) {
		$(".blank-owner-msg").removeClass("hide-yo-wives");
		markInvalidOwnerId("");
		isValid = false;
	}
	return isValid;
}

function resetValidations() {
	$(".blank-owner-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}
