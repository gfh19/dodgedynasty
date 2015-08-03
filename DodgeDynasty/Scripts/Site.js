var pickTimeSeconds = 0;
var playerHints = [];
var hasModelErrors = false;
var isRefreshPage = false;
var draftActive = false;
var isUserTurn = false;
var currentServerTime = null;
var clientServerTimeOffset = null;
var draftHub;
var connectionStopped = false;

/* Init functions */

$(function () {
	$.ajaxSetup({
		cache: false
	});
	setUserAgentInfo();
	highlightCurrentPageLink();
	bindMenuLinks();
	if (!draftChatKillSwitch) {
		bindDraftChatWindow();
	}
	checkUserTurnDialog();
});

function setUserAgentInfo() {	//Currently just for tablet/phablet CSS
	if (/Android|iPad|iPhone|iPod|KFAPWI|Tablet|Touch/i.test(navigator.userAgent)) {
		//is possible tablet/phablet
		$("body").addClass("possible-touch");
	}
}

function initRefreshedPage() {
	isRefreshPage = true;
	checkUserTurnDialog();
	if (!webSocketsKillSwitch) {
		setTimeout(function () {
			if (draftActive && !connectionStopped && $.connection.hub.state == $.signalR.connectionState.disconnected) {
				console.log("Disconnect detected.  Manual Reconnection Attempted.");
				startHubConnection(function () { window.location.reload() });
			}
		}, 2500);
	}
};

/*		--- WebSockets */

function initPage() {
	if (draftActive && !webSocketsKillSwitch) {
		draftHub = $.connection.draftHub;
		draftHub.client.broadcastDraft = (typeof broadcastDraft !== "undefined") ? broadcastDraft : function () { };
		draftHub.client.broadcastChat = (typeof broadcastChat !== "undefined") ? broadcastChat : function () { };
		draftHub.client.broadcastDisconnect = (typeof broadcastDisconnect !== "undefined") ? broadcastDisconnect : function () { };
		startHubConnection();
	}
}

function startHubConnection(startFn, forceAttempt) {
	var connected = false;
	if (!webSocketsKillSwitch) {
		draftHub = draftHub || $.connection.draftHub;
		if ($.connection.hub.state == $.signalR.connectionState.disconnected) {
			console.log("Attempting socket connection");
			$.connection.hub.start().done(function () {
				connected = true;
				//Quick send broadcast before closing connection?
				if (startFn) startFn();
				console.log("Calling ajax open conn...");
				registerHubConnection(forceAttempt);
			});
		}
		else {
			connected = true;
			if (startFn) startFn();
		}
		setTimeout(function () {
			if (!connected && $.connection.hub.state == $.signalR.connectionState.connecting) {
				console.log("Conn timer expired, closing connection.");
				$.connection.hub.stop();
				refreshTimer = fastRefreshTimer;
				connectionStopped = true;
				//Mark chat unavailable
				toggleChatWindowError(true);
			}
			else if (!connected) {
				console.log("No responses, conn state is: " + $.connection.hub.state);
			}
		}, 15000);
	}
}

function registerHubConnection(forceAttempt) {
	ajaxPost({ connectionId: $.connection.hub.id, forceAttempt: forceAttempt }, "Site/OpenDraftHubConnection",
		function (response) {
			if (response.good) {
				console.log('Socket connection started.  ' + response.conns + ' tab(s) registered.');
				refreshTimer = defaultRefreshTimer;
				//Mark chat available again
				toggleChatWindowError(false);
			}
			else {
				console.log('Too many tabs open.  Closing socket connection. ' + response.conns + ' tabs regged.');
				$.connection.hub.stop();
				refreshTimer = fastRefreshTimer;
				connectionStopped = true;
				//Mark chat unavailable
				toggleChatWindowError(true);
			}
		}, null, "JSON");
}

//From client to server
function broadcastPickMade() {
	startHubConnection(function () { draftHub.server.pick(); });
	return true;
};

//From client to server
function broadcastChatMessage(msg) {
	if (!draftChatKillSwitch) {
		startHubConnection(function () { draftHub.server.chat(msg); });
		return true;
	}
	return false;
};

//Server to client:  Draft Pick broadcast-received.  Bound to server-side (C#) hub client handle
function broadcastDraft() {
	if (typeof pageBroadcastDraftHandler !== "undefined") pageBroadcastDraftHandler();
	checkUserTurnDialog();
}

//Server to client:  Broadcast to shutdown all open connections. And close draft.
function broadcastDisconnect() {
	connectionStopped = true;
	$(".dchat-close-link").click();
	$.connection.hub.stop();
	broadcastDraft();
}

//Chat broadcast-received:  Function bound to server-side (C#) hub client handle
function broadcastChat(chat) {
	chat.msg = htmlEncode(chat.msg);
	var dchatWindow = $(".dchat-window");
	var copy = $(".dchat-template .dchat-entry", dchatWindow).clone();
	$(".dchat-prev-stamp", copy).addClass(chat.css);
	$(".dchat-prev-stamp", copy).text(chat.author + " (" + chat.time + "):");
	$(".dchat-prev-outline", copy).after(" " + chat.msg);

	$(".dchat-body .dchat-entry", dchatWindow).last().append($(copy));
	$(".dchat-prev-content", dchatWindow).html($(copy).html())

	scrollDraftChatBottom();
}

/*		--- End WebSockets */

function setPickTimer(recursive) {
	var hasTimer = $(".start-time").length > 0;
	pickTimeSeconds = (hasTimer) ? $(".start-time").data("pick-time-seconds") : 0;
	if (hasTimer && pickTimeSeconds > 0) {
		var isHistoryMode = window.location.search.indexOf("historyMode=true") > 0;
		if (!draftActive || isHistoryMode) {
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
	if (draftActive &&
		(!isRefreshPage			//Either user turn is unknown, i.e. not on refreshed page  
		|| isUserTurn))			//Or we know is user turn on refreshed page
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
			if (!isElementInView($(".current-turn"))) {
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

/*		--- Draft Chat */

function bindDraftChatWindow() {
	$(".dchat-close-link").click(function (e) {
		e.preventDefault();
		var dchatWindow = getActiveDchatWindow();
		$(dchatWindow).addClass("hide-yo-wives");
	});
	$(".dchat-toggle-link").click(function (e) {
		e.preventDefault();
		toggleDraftChatView();
	});
	$(".dchat-preview").click(function (e) {
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
	$('.force-dchat').click(function (e) {
		e.preventDefault();
		startHubConnection(null, true);
		toggleChatWindowError(false);
	});
}

function getActiveDchatWindow() {
	return $(".dchat-window").hasClass("hide-yo-wives") ? $(".dchat-window-error") : $(".dchat-window");
}

function toggleDraftChatView() {
	var isErrorWindow = $(".dchat-window").hasClass("hide-yo-wives");
	var dchatWindow = isErrorWindow ? $(".dchat-window-error") : $(".dchat-window");
	var expanded = toBool($(".dchat-toggle-link", dchatWindow).attr("data-expand"));
	var toggleImg = expanded ? "expand.png" : "collapse.png";
	$(".dchat-toggle-img", dchatWindow).attr("src", contentImagesPath + toggleImg);

	if (expanded) {
		$(dchatWindow).removeClass("dchat-ease-expand");
		$(dchatWindow).addClass("dchat-ease-collapse");
		$(".dchat-prev-content", dchatWindow).removeClass("hide-yo-wives");
		if (isErrorWindow) {
			$(".dchat-preview", dchatWindow).addClass("invalid-border-small");
		}
		$(".dchat-body", dchatWindow).addClass("hide-yo-wives");
		$(".dchat-footer", dchatWindow).addClass("hide-yo-wives");
		$(".dchat-input", dchatWindow).removeClass("invalid-border-small");
	}
	else {
		$(dchatWindow).removeClass("dchat-ease-collapse");
		$(dchatWindow).addClass("dchat-ease-expand");
		$(".dchat-prev-content", dchatWindow).addClass("hide-yo-wives");
		if (isErrorWindow) {
			$(".dchat-preview", dchatWindow).removeClass("invalid-border-small");
		}
		$(".dchat-body", dchatWindow).removeClass("hide-yo-wives");
		$(".dchat-footer", dchatWindow).removeClass("hide-yo-wives");
		scrollDraftChatBottom();
	}
	$(".dchat-toggle-link", dchatWindow).attr("data-expand", !expanded);
}

function toggleChatWindowError(hasError) {
	if (hasError && !$(".dchat-window").hasClass("hide-yo-wives")) {
		$(".dchat-window").addClass("hide-yo-wives");
		$(".dchat-window-error").removeClass("hide-yo-wives");
	}
	else if (!$(".dchat-window-error").hasClass("hide-yo-wives")) {
		$(".dchat-window-error").addClass("hide-yo-wives");
		$(".dchat-window").removeClass("hide-yo-wives");
	}
}

function sendDraftChat() {
	var msg = $(".dchat-input").val();
	if (msg != undefined && msg.trim().length > 0) {
		broadcastChatMessage(msg);
		$(".dchat-input").removeClass("invalid-border-small");
		$(".dchat-input").val("");
	}
	else {
		$(".dchat-input").addClass("invalid-border-small");
	}
}

function scrollDraftChatBottom() {
	var dchatWindow = getActiveDchatWindow();
	var d = $('.dchat-body', dchatWindow);
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