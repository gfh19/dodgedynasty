//var refreshTimer = 60000;
var pickTimeSeconds = 0;
var playerHints = [];
var hasModelErrors = false;
var isRefreshPage = false;
var draftActive = false;
var isUserTurn = false;
var currentServerTime = null;
var clientServerTimeOffset = null;
var draftHub;

/* Init functions */

$(function () {
	$.ajaxSetup({
		cache: false
	});
	highlightCurrentPageLink();
	bindMenuLinks();
	bindDraftChatWindow();
	checkUserTurnDialog();
});

function initRefreshedPage() {
	isRefreshPage = true;
	checkUserTurnDialog();
};

/*		--- WebSockets */

function initPage(draftActive) {
	if (draftActive) {
		startHubConnection();
		draftHub.client.broadcastDraft = (typeof broadcastDraft !== "undefined") ? broadcastDraft : function () { };
		//Check draftChat config entry
		draftHub.client.broadcastChat = (typeof broadcastChat !== "undefined") ? broadcastChat : function () { };
	}
}

function startHubConnection(startFn) {
	var connected = false;
	draftHub = $.connection.draftHub;
	if ($.connection.hub.state == $.signalR.connectionState.disconnected) {
		$.connection.hub.start().done(function () {
console.log('Socket connection started.');
			connected = true;
			if (startFn) startFn();
			refreshTimer = defaultRefreshTimer;
			//Mark chat available again
		});
	}
	else {
		connected = true;
		if (startFn) startFn();
	}
	setTimeout(function () {
		if (!connected && $.connection.hub.state == $.signalR.connectionState.connecting) {
			$.connection.hub.stop();
			refreshTimer = fastRefreshTimer;
			//Mark chat unavailable
		}
	}, 12000);
}

//From client to server
function broadcastPickMade() {
	startHubConnection(function () { draftHub.server.pick(); });
	return true;
};

//From client to server
function broadcastChatMessage(msg) {
	startHubConnection(function () { draftHub.server.chat(msg); });
	return true;
};

//Draft Pick broadcast-received:  Function bound to server-side (C#) hub client handle
function broadcastDraft() {
	if (typeof pageBroadcastDraftHandler !== "undefined") pageBroadcastDraftHandler();
	checkUserTurnDialog();
}

//Chat broadcast-received:  Function bound to server-side (C#) hub client handle
function broadcastChat(chat) {
	chat.msg = htmlEncode(chat.msg);
	var copy = $(".dchat-template .dchat-entry").clone();
	$(".dchat-prev-stamp", copy).addClass(chat.css);
	$(".dchat-prev-stamp", copy).text(chat.author + " (" + chat.time + "):");
	$(".dchat-prev-outline", copy).after(" " + chat.msg);

	$(".dchat-body .dchat-entry").last().append($(copy));
	$(".dchat-preview").html($(copy).html())

	scrollDraftChatBottom();
}

/*		--- End WebSockets */

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

function refreshPageWithPickTimer(url, elementId, timer) {
	timer = (timer === undefined) ? refreshTimer : timer;
	setTimeout(function () {
		callRefreshPageWithPickTimer(url, elementId);
console.log("Refreshed page at " + new Date());
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
	if ((isRefreshPage && isUserTurn) 	//Either we know is user turn on refreshed page
		|| !isRefreshPage)				//Or user turn is unknown, i.e. not on refreshed page
	{
		tryShowUserTurnDialog();
	}
}

function tryShowUserTurnDialog() {
	if ($.cookie("userTurnSettings")) {
		var settings = jQuery.parseJSON($.cookie("userTurnSettings"));
		if (!settings.shown && !settings.neverShowAgain) {
			showUserTurnDialog();
		}
	}
	else {
		showUserTurnDialog();
	}
}

function showUserTurnDialog() {
	if ($("#userTurnDialog").length > 0) {
		setLatestUserTurnPickInfo(function () {
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
		});
	}
}

function setLatestUserTurnPickInfo(showFn) {
	ajaxGetJson("Draft/GetUserTurnPickInfo", function (pickInfo) {
		if (pickInfo && pickInfo.turn) {
			setUserTurnCookie(true, false);
			if (!isElementInView($(".drafter"))) {
				$("#userTurnDialog").attr("title", "Your Turn - Pick #" + pickInfo.num);
				if (pickInfo.hasPrev) {
					$(".ut-last-pick").text("(Last Pick: " + pickInfo.prevName + ")");
				}
				if (showFn) showFn();
			}
		}
	});
}

function setUserTurnCookie(shown, neverShowAgain) {
	var settings = { shown: shown, neverShowAgain: neverShowAgain };
	$.cookie("userTurnSettings", JSON.stringify(settings), { path: baseURL });
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

function bindMenuLinks() {
	$("#toggle-msgs-link").click(function (e) {
		e.preventDefault();
		$(".navbar-toggle").click();
	});
	$(".navbar-toggle").click(function (e) {
		easeHideToggleMsgs()
	});
	$(".menu-broadcast-draft").click(function (e) {
		e.preventDefault();
		broadcastPickMade();
	});
}

function easeHideToggleMsgs() {
	if (!$("#toggle-msgs-link").hasClass("hide-yo-wives")) {
		$(".toggle-msgs").addClass("ease-hide");
		setTimeout(function () {
			$("#toggle-msgs-link").addClass("hide-yo-wives");
		}, 250);
	}
}

function bindDraftChatWindow() {
	$(".dchat-close-link").click(function (e) {
		e.preventDefault();
		$(".dchat-window").addClass("hide-yo-wives");
	});
	$(".dchat-toggle-link").click(function (e) {
		e.preventDefault();
		toggleDraftChatView();
	});
	$(".dchat-send-msg").click(function (e) {
		e.preventDefault();
		sendDraftChat();
	});
	$('.dchat-input').keypress(function (e) {
		var evt = e || window.event;
		var key = evt.charCode || evt.keyCode;

		if ((evt.type == "keydown" || evt.type == "keypress")) {
			if (key == 13) {
				evt.preventDefault();
				$(".dchat-send-msg").click();
			}
		}
	});
}

function toggleDraftChatView() {
	var expanded = toBool($(".dchat-toggle-link").attr("data-expand"));
	var toggleImg = expanded ? "expand.png" : "collapse.png";
	//var dchatHeight = expanded ? "20px" : "175px";
	$(".dchat-toggle-img").attr("src", contentImagesPath + toggleImg);
	if (expanded) {
		$(".dchat-window").removeClass("dchat-ease-expand");
		$(".dchat-window").addClass("dchat-ease-collapse");
		$(".dchat-preview").removeClass("hide-yo-wives");
		$(".dchat-body").addClass("hide-yo-wives");
		$(".dchat-footer").addClass("hide-yo-wives");
		$(".dchat-input").removeClass("invalid-border-small");
	}
	else {
		$(".dchat-window").removeClass("dchat-ease-collapse");
		$(".dchat-window").addClass("dchat-ease-expand");
		$(".dchat-preview").addClass("hide-yo-wives");
		$(".dchat-body").removeClass("hide-yo-wives");
		$(".dchat-footer").removeClass("hide-yo-wives");
		scrollDraftChatBottom();
	}
	$(".dchat-toggle-link").attr("data-expand", !expanded);
}

function sendDraftChat() {
	var msg = $(".dchat-input").val();
	if (msg != undefined && msg.trim().length > 0) {
		broadcastChatMessage(msg);
		$(".dchat-input").removeClass("invalid-border-small");
		$(".dchat-input").val("");
		$(".dchat-input").focus();
	}
	else {
		$(".dchat-input").addClass("invalid-border-small");
	}
}

function scrollDraftChatBottom() {
	var d = $('.dchat-body');
	d.scrollTop(d.prop("scrollHeight"));
}



/* Helper functions */

function ajaxGetJson(url, successFn) {
	$.get(baseURL + url, {}, successFn, "JSON");
};

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

function isElementInView(el) {
	if (el instanceof jQuery) {
		el = el[0];
	}
	if (!el) {
		return false;
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
		if ($("#" + this.name) !== undefined && $("#" + this.name).attr("type") == "checkbox") {
			o[this.name] = $("#" + this.name).prop('checked');
		}
	});
	return o;
};

function toBool(val) {
	return val == true || (val != null && val != undefined && val.toLowerCase() == "true");
}
function htmlEncode(val) {
	return $('<div/>').text(val).html();
}

function htmlDecode(val) {
	return $('<div/>').html(val).text();
}