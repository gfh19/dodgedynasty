﻿//var refreshTimer = 60000;
var pickTimeSeconds = 0;
var playerHints = [];
var hasModelErrors = false;
var isUserTurn = false;
var draftActive = false;
var currentServerTime = null;
var clientServerTimeOffset = null;
var draftHub;

$(function () {
	$.ajaxSetup({
		cache: false
	});
	highlightCurrentPageLink();
});

function initRefreshedPage() {
	checkUserTurnDialog();
	if (draftActive) {
		startHubConnection();
		draftHub.client.broadcastDraft = broadcastDraft;
	}
};

function startHubConnection(startFn) {
	draftHub = $.connection.draftHub;
	if ($.connection.hub.state == $.signalR.connectionState.disconnected) {
		$.connection.hub.start().done(function () {
			if (startFn) startFn();
		});
	}
	else {
		if (startFn) startFn();
	}
}

function setPickTimer(recursive) {
	var hasTimer = $(".start-time").length > 0;
	pickTimeSeconds = (hasTimer) ? $(".start-time").data("pick-time-seconds") : 0;
	if (hasTimer && pickTimeSeconds > 0) {
		if (!draftActive) {
			displayDraftTime();
		}
		else {
			var currentPick = $(".start-time").attr("data-current-pick");
			var startTimeString = $(".start-time").attr("data-pick-start-time");
			var svrCurrTime = $(".start-time").attr("data-current-time");
			if (currentServerTime != svrCurrTime) {
				currentServerTime = svrCurrTime;
				clientServerTimeOffset = Math.floor((new Date(currentServerTime) - new Date()) / 1000);
			}
			var startDateTime = new Date(startTimeString);
			var currentTime = new Date();
			currentTime.setSeconds(currentTime.getSeconds() + clientServerTimeOffset);

			var timeElapsed = Math.floor((currentTime - startDateTime) / 1000);
			var diff = (pickTimeSeconds - timeElapsed);
			if (diff > pickTimeSeconds) {
				if (currentPick == "1") {
					displayDraftTime();
				}
				else {
					diff = pickTimeSeconds;
					displayTimeRemaining(diff);
				}
			}
			else if (diff > 0) {
				displayTimeRemaining(diff);
			}
			else {
				$(".start-time").text("TIME'S UP!");
			}
			if (recursive) {
				setTimeout(function () {
					setPickTimer(true);
				}, 999);
			}
		}
	}
}

function displayTimeRemaining(diff) {
	var minutes = Math.floor(diff / 60);
	var seconds = diff - (minutes * 60);
	$(".start-time").text(minutes + ":" + ((seconds < 10) ? "0" : "") + seconds);
}

function displayDraftTime() {
	//var draftTime = moment([2014, 7, 30, 18, 30, 0]);
	moment.suppressDeprecationWarnings = true;
	var draftTime = moment(new Date($(".start-time").attr("data-draft-time")).toString());
	$(".start-time").text("Draft Begins: " + draftTime.format("ddd, M/DD @ h:mmA"));
}

function highlightCurrentPageLink() {
	var currentLink = $(".menu a").filter(function () {
		return ($(this).prop('pathname').length > 0
			&& $(this).attr("href") != "#"
			&& $(this).prop('pathname') == location.pathname 
			&& $(this).prop('id') != "more-nav");
	});
	$(currentLink).addClass("current-page");
	return;
}

function ajaxGet(url, successFn) {
	$.get(baseURL + url, successFn);
};

function ajaxPost(model, url, successFn, errorFn, dataType, makeSync) {
	dataType = dataType || "html";
	makeSync = makeSync || false;
	$.ajax({
		url: baseURL + url,
		type: "POST",
		data: JSON.stringify(model),
		dataType: dataType,
		contentType: "application/json; charset=utf-8",
		success: successFn,
		error: errorFn,
		async: !makeSync
	});
};

function ajaxGetReplace(url, elementId, successFn) {
	ajaxGet(url, function (response) {
		replaceWith(elementId, response);
		if (successFn) {
			successFn();
		}
	});
}

function ajaxPostReplace(model, url, elementId, successFn, errorFn, dataType, makeSync) {
	ajaxPost(model, url, function (response) {
		replaceWith(elementId, response);
		if (successFn) {
			successFn();
		}
	}, errorFn, dataType, makeSync);
}

function replaceWith(elementId, contents) {
	$(elementId).replaceWith(contents);
}

function refreshPageWithPickTimer(url, elementId, timer) {
	timer = (timer === undefined) ? refreshTimer : timer;
	setTimeout(function () {
		callRefreshPageWithPickTimer(url, elementId);
	},
	timer);
};

function callRefreshPageWithPickTimer(url, elementId) {
	if (draftHistoryId == '') {
		if (draftActive) {
			callRefreshPage(url, elementId);
			refreshPageWithPickTimer(url, elementId);
		}
	}
};

function callRefreshPage(url, elementId) {
	ajaxGetReplace(url, elementId, function () {
		setPickTimer(false);
	});
}

function setPlayerAutoComplete(fname, lname, pos, nfl) {
	fname = fname || "#Player_FirstName";
	lname = lname || "#Player_LastName";
	pos = pos || "#Player_Position";
	nfl = nfl || "#Player_NFLTeam";
	$(fname).autocomplete({
		source: function (request, response) {
			var filteredArray = $.map(playerHints, function (item) {
				var response = null;
				var nameParts = [item.firstName, item.lastName];
				$.each(nameParts, function (index, elem) {
					if (elem.trim().toUpperCase().match("^" + request.term.trim().toUpperCase())) {
						response = item;
					}
				});
				return response;
			});
			response(filteredArray);
		},
		select: function (event, ui) {
			$(nfl).val(ui.item.nflTeamDisplay);
			$(pos).val(ui.item.pos);
			$(lname).val(ui.item.lastName);
			$(fname).val(ui.item.firstName);
			setTimeout(function () { $("#inputSubmit").focus(); }, 0);
			return false;
		}
	});
};

function checkUserTurnDialog() {
	if ($("#userTurnDialog").length > 0) {
		if ($.cookie("userTurnSettings")) {
			var settings = jQuery.parseJSON($.cookie("userTurnSettings"));
			if (!settings.shown && !settings.neverShowAgain) {
				setUserTurnCookie(true, false);
				if (!isElementInView($(".drafter"))) {
					showUserTurnDialog();
				}
			}
		}
		else {
			setUserTurnCookie(true, false);
			if (!isElementInView($(".drafter"))) {
				showUserTurnDialog();
			}
		}
	}
}

function setUserTurnCookie(shown, neverShowAgain) {
	var settings = { shown: shown, neverShowAgain: neverShowAgain };
	$.cookie("userTurnSettings", JSON.stringify(settings), { path: baseURL });
}

function showUserTurnDialog() {
	$("#userTurnDialog").dialog({
		resizable: false,
		height: 'auto',
		width: '250px',
		modal: false,
		buttons: [
					{ text: "Make Pick", click: function () { location.href = baseURL + "Draft/Pick"; $(this).dialog("close"); } },
					{ text: "Close", click: function () { $(this).dialog("close"); } },
				]
	});
}

function isElementInView(el) {
	if (el instanceof jQuery) {
		el = el[0];
	}
	var rect = el.getBoundingClientRect();
	return (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
        rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
    );
}

function preventBackspaceNav(e) {
	var evt = e || window.event;
	var key = evt.charCode || evt.keyCode;

	if ((evt.type == "keydown" || evt.type == "keypress") && ($('input:focus').length == 0)) {
		if (key == 8) {
			evt.preventDefault();
		}
	}
}

function markInvalidId(userId) {
	if (userId === "") {
		//Find any blank spans
		$.each($("select"), function (index, user) {
			if ($(user).val() === "") {
				$(user).addClass("invalid-border");
			}
		});
	}
	else {
		//Find any matching selected options
		var invalidEntries = $("select option:selected[value=" + userId + "]").closest("select");
		$(invalidEntries).addClass("invalid-border");
	}
}

$.fn.serializeObject = function () {
	var o = {};
	var a = this.serializeArray();
	$.each(a, function () {
		if (o[this.name] !== undefined) {
			if (!o[this.name].push) {
				o[this.name] = [o[this.name]];
			}
			o[this.name].push(this.value || '');
		} else {
			o[this.name] = this.value || '';
		}
		if ($("#" + this.name) !== undefined && $("#" + this.name).attr("type") == "checkbox")
		{
			o[this.name] = $("#" + this.name).prop('checked');
		}
	});
	return o;
};

function toBool(val) {
	return val == true || (val != null && val != undefined && val.toLowerCase() == "true");
}

function broadcastPickMade() {
	startHubConnection(function () { draftHub.server.pick(); });
	return true;
};