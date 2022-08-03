var pickTimeSeconds = 0;
var playerHints = [];
var hasModelErrors = false;
var isRefreshPage = false;
var draftActive = false;	//Means ANY draftActive, not necessarily this one (check historyMode)
							//Currently only ever set to true in DraftChatPartial
var isUserTurn = false;
var loggedInUserName = "";
var currentServerTime = null;
var clientServerTimeOffset = null;
var draftHub;
var connectionAttempted = false;
var connectionStopped = false;
var touchScrollDiv = null;
var touchScrollLeft = null;
var lastPickAudio = null;
var pickAudioBed = null;
var adminLastPickTime = null;
var pleaseWaitTimer = 1100;
var pleaseWaitNeeded = false;
var tickingClockAudio = null;
var isTickingClockPlaying = false;


/* Init functions */

$(function () {
	$.ajaxSetup({
		cache: false
	});
	setUserAgentInfo();
	highlightCurrentPageLink();
	bindMenuLinks();
	initWebSockets();
	if (!draftChatKillSwitch) {
		bindDraftChatWindow();
	}
	checkUserTurnDialog();
	initLastPickAudio();
	refreshDraftChat();
});


function setUserAgentInfo() {	//Currently just for tablet/phablet CSS
	if (isTouchDevice()) {
		//is possible tablet/phablet
		$("body").addClass("possible-touch");
	}
}

function initRefreshedPage() {
	isRefreshPage = true;
	checkUserTurnDialog();
	checkStillSocketConnected(false);
	bindGotoPickNum();
};

function bindGotoPickNum() {
	$("#gotoPickNum").unbind("click");
	$("#gotoPickNum").click(function (e) {
		var pickNum = $("#gotoPickNum").text();
		var draftPick = $(".draft-pick[data-pick-num=" + pickNum + "]");
		if (draftPick) {
			$(draftPick).goTo();
		}
	});
}

/*		--- WebSockets */

function initWebSockets() {
	if (draftActive && !webSocketsKillSwitch) {
		draftHub = $.connection.draftHub;
		draftHub.client.broadcastDraft = (typeof broadcastDraft !== "undefined") ? broadcastDraft : function () { };
		draftHub.client.broadcastChat = (typeof broadcastChat !== "undefined") ? broadcastChat : function () { };
		draftHub.client.broadcastDisconnect = (typeof broadcastDisconnect !== "undefined") ? broadcastDisconnect : function () { };
		draftHub.client.broadcastDraftToUser = (typeof broadcastDraftToUser !== "undefined") ? broadcastDraftToUser : function () { };
		startHubConnection();
		checkStillSocketConnected(true);
	}
}

function startHubConnection(startFn, forceAttempt) {
	var connected = false;
	if (!webSocketsKillSwitch) {
		connectionAttempted = true;
		draftHub = draftHub || $.connection.draftHub;
		if ($.connection.hub.state == $.signalR.connectionState.disconnected) {
			console.log("Attempting socket connection");
			$.connection.hub.start().done(function () {
				connected = true;
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
				if (typeof pageBroadcastDraftHandler !== "undefined") {
					pageBroadcastDraftHandler();
				}
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
				console.log('Unavailable socket. Closing socket connection. ' + response.conns + ' tabs regged.');
				$.connection.hub.stop();
				refreshTimer = fastRefreshTimer;
				connectionStopped = true;
				//Mark chat unavailable
				toggleChatWindowError(true);
				if (typeof pageBroadcastDraftHandler !== "undefined") {
					pageBroadcastDraftHandler();
				}
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
function broadcastDraft(pickInfo) {
	stopTickingClockAudio();
	getLastPickAndPlayAudio(pickInfo, isUserTurn);
	setIsUserTurn(pickInfo);
	if (typeof pageBroadcastDraftHandler !== "undefined" && !isHistoryMode()) {
		pageBroadcastDraftHandler(pickInfo);
	}
	checkUserTurnDialog();
}

function broadcastDraftToUser(userName) {
	if (!userName || !loggedInUserName || userName != loggedInUserName) {
		return;
	}
	if (typeof pageBroadcastDraftHandler !== "undefined" && !isHistoryMode()) {
		pageBroadcastDraftHandler();
	}
}

//Server to client:  Broadcast to shutdown all open connections. And close draft.
function broadcastDisconnect() {
	connectionStopped = true;
	$(".dchat-close-link").click();
	$.connection.hub.stop();
	broadcastDraft();
	draftActive = false;
}

//Chat broadcast-received:  Function bound to server-side (C#) hub client handle
function broadcastChat(chat) {
	chat.msg = htmlEncode(chat.msg);
	var dchatWindow = $(".dchat-window");
	var copy = $(".dchat-template .dchat-entry", dchatWindow).clone();
	$(".dchat-prev-stamp", copy).addClass(chat.css);
	$(".dchat-prev-stamp", copy).text(chat.author + " (" + chat.time + "):");
	$(".dchat-prev-outline", copy).after(" <span class='dchat-msg-text'>" + chat.msg + "</span>");

	$(".dchat-body .dchat-entry", dchatWindow).last().append($(copy));
	$(".dchat-prev-content", dchatWindow).html($(copy).html())

	formatDraftChatText();
	scrollDraftChatBottom();
}

function checkStillSocketConnected(recursive) {
	if (!webSocketsKillSwitch) {
		setTimeout(function () {
			if (draftActive && connectionAttempted && !connectionStopped && $.connection.hub.state == $.signalR.connectionState.disconnected) {
				console.log("Disconnect detected.  Manual Reconnect Attempted.");
				startHubConnection(function () {
					checkUserTurnDialog();
					refreshDraftChat();
				});
			}
		}, 1000);
		if (recursive) {
			setTimeout(function () {
				if (draftActive) {
					checkStillSocketConnected(true);
				}
			}, 10000);
		}
	}
}

function refreshDraftChat() {
	if (!draftChatKillSwitch && draftActive) {
		ajaxGetReplace("Site/DraftChatPartial", "#dchat-partial", function () {
			bindDraftChatWindow();
		});
	}
}

/* Broadcast draft perf opt helper functions */

function updatePageWithDraftPickInfo(pickInfo, updateFn, refreshFn) {
	if (isValidPickInfo(pickInfo)) {
		updateCurrentDraftPickPartial(pickInfo);
		updateNewLastPickEndTime(pickInfo);
		if (updateFn) {
			updateFn(pickInfo);
		}
	}
	else if (refreshFn) {
		refreshFn();
	}
}

function isValidPickInfo(pickInfo) {
	return pickInfo && (pickInfo.status == "success" || pickInfo.status == "completed") && isPickUpToDate(pickInfo);
}

function isPickUpToDate(pickInfo) {
	var lastPickEndTime = $("*[data-last-pick-end-time]").attr("data-last-pick-end-time");
	if (lastPickEndTime && !isNullOrWhitespace(lastPickEndTime)) {
		return pickInfo.prevtm == lastPickEndTime;
	}
	else {
		return pickInfo.prevtm == "01/01/0001 00:00:00";
	}
}

function updateNewLastPickEndTime(pickInfo) {
	$("*[data-last-pick-end-time]").attr("data-last-pick-end-time", pickInfo.petime);
}

function updateCurrentDraftPickPartial(pickInfo) {
	//Assumes pickInfo validated
	if (pickInfo.status == "completed") {
		$(".current-turn").addClass("hide-yo-wives");
		$(".start-time").addClass("hide-yo-wives");
		$(".prev-pick").addClass("hide-yo-wives");
		$(".next-pick-count").addClass("hide-yo-wives");
		$(".cdp-enter-pick").addClass("hide-yo-wives");
		$(".cdp-draft-complete").removeClass("hide-yo-wives");
	}
	else {
		$(".current-turn").removeClass("hide-yo-wives");
		$(".start-time").removeClass("hide-yo-wives");
		$(".prev-pick").removeClass("hide-yo-wives");
		$(".cdp-draft-complete").addClass("hide-yo-wives");
		var currUserId = $("*[data-user-id]").attr("data-user-id");
		if (currUserId == pickInfo.uturnid) {
			$(".cdp-clock-owner").text("YOU!");
		}
		else {
			$(".cdp-clock-owner").text(pickInfo.curoname);
		}
		if ($("#gotoPickNum") && $("#gotoPickNum").length > 0) {
			$("#gotoPickNum").text(pickInfo.curpnum);
		}
		else {
			$(".cdp-picknum").text(pickInfo.curpnum);
		}
		$(".start-time[data-current-pick]").attr("data-current-pick", pickInfo.curpnum);
		$(".start-time[data-current-time]").attr("data-current-time", pickInfo.curtm);
		$(".start-time[data-pick-start-time]").attr("data-pick-start-time", pickInfo.curpstime);
		$(".cdp-prev-player").text(pickInfo.pname);
		$(".cdp-prev-owner").text(pickInfo.oname);
		if (pickInfo.pctr) {
			var currUserPctr = pickInfo.pctr[currUserId];
			if (currUserPctr === 0) {
				$(".next-pick-count").addClass("hide-yo-wives");
				$(".cdp-enter-pick").removeClass("hide-yo-wives");
			}
			else if (currUserPctr > 0) {
				$(".cdp-enter-pick").addClass("hide-yo-wives");
				$(".next-pick-count").removeClass("hide-yo-wives");
				if (currUserPctr === 1) {
					$(".cdp-pickctr-text").text("(You're up next...)");
				}
				else {
					$(".cdp-pickctr-text").text("(You're up after " + currUserPctr + " picks)");
				}
			}
			else {
				//else no more picks for this particular user
				$(".next-pick-count").addClass("hide-yo-wives");
				$(".cdp-enter-pick").addClass("hide-yo-wives");
			}
		}
	}
}

/* End Broadcast draft perf opt helper functions */

/*		--- End WebSockets */


function isHistoryMode() {
	return window.location.search.indexOf("historyMode=true") > 0;
}

function setPickTimer(recursive) {
	var hasTimer = $(".start-time").length > 0;
	pickTimeSeconds = (hasTimer) ? $(".start-time").data("pick-time-seconds") : 0;
	if (hasTimer && pickTimeSeconds > 0) {
		if (!draftActive || isHistoryMode()) {
			displayDraftTime();
		}
		else {
			//Calculate current time (compare to server) for accurate current minus pick start value
			var startTimeString = $(".start-time").attr("data-pick-start-time");
			var svrCurrTime = $(".start-time").attr("data-current-time");
			if (currentServerTime != svrCurrTime) {
				currentServerTime = svrCurrTime;
				clientServerTimeOffset = Math.floor((new Date(currentServerTime) - new Date()) / 1000);
			}
			var startDateTime = new Date(startTimeString);
			var draftStartTime = new Date($(".start-time").attr("data-draft-time"));
			var currentTime = new Date();
			currentTime.setSeconds(currentTime.getSeconds() + clientServerTimeOffset);

			//Calculate draft pick time remaining & display
			var timeElapsed = Math.floor((currentTime - startDateTime) / 1000);
			var timeRemaining = (pickTimeSeconds - timeElapsed);
			if (timeRemaining > pickTimeSeconds) {
				var currentPick = $(".start-time").attr("data-current-pick");
				if (currentPick == "1") {
					displayDraftTime();
				}
				else if (draftStartTime - currentTime > 0) {
					displayDraftTime();
				}
				else {
					timeRemaining = pickTimeSeconds;
					displayTimeRemaining(timeRemaining);
				}
			}
			else if (timeRemaining > 0) {
				displayTimeRemaining(timeRemaining);
				if (timeRemaining <= 16) {
					setTimeout(playTickingClockAudio, 500);
				}
			}
			else {
				$(".start-time").text("TIME'S UP!");
			}
			if (recursive) {
				setTimeout(function () {
					setPickTimer(true);
				}, 1000);
			}
		}
	}
}

function playTickingClockAudio() {
	if (!isTickingClockPlaying && tickingClockAudio && lastPickAudio && toBool(lastPickAudio.access) && toBool(lastPickAudio.success)) {
		isTickingClockPlaying = true;
		tickingClockAudio.play();
		tickingClockAudio.addEventListener("ended", function () {
			isTickingClockPlaying = false;
		});
	}
}

function displayTimeRemaining(timeRemaining) {
	var minutes = Math.floor(timeRemaining / 60);
	var seconds = timeRemaining - (minutes * 60);
	$(".start-time").text(minutes + ":" + ((seconds < 10) ? "0" : "") + seconds);
}

function displayDraftTime() {
	//var draftTime = moment([2014, 7, 30, 18, 30, 0]);
	moment.suppressDeprecationWarnings = true;
	var draftTime = moment(new Date($(".start-time").attr("data-draft-time")).toString());
	$(".start-time").text("Draft Begins: " + draftTime.format("ddd, M/D @ h:mmA"));
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

function refreshPageWithPickTimer(url, elementId, timer, successFn, errorFn, preRefreshFn) {
	timer = timer || refreshTimer;
	if (draftActive) {
		setTimeout(function () {
			callRefreshPageWithPickTimer(url, elementId, successFn, errorFn, preRefreshFn);
			console.log("Refreshed page at " + new Date());
		},
		timer);
	}
};

function callRefreshPageWithPickTimer(url, elementId, successFn, errorFn, preRefreshFn) {
	if (!isHistoryMode()) {
		if (draftActive) {
			callRefreshPage(url, elementId, successFn, errorFn, preRefreshFn);
			refreshPageWithPickTimer(url, elementId, null, successFn, errorFn, preRefreshFn);
		}
	}
};

function callRefreshPage(url, elementId, successFn, errorFn, preRefreshFn) {
	saveTouchScrollPos();
	var refreshUrl = getDynamicUrl(url);
	if (preRefreshFn) {
		preRefreshFn();
	}
	ajaxGetReplace(refreshUrl, elementId, function () {
		restoreTouchScrollPos();
		setPickTimer(false);
		removeWaitCursor();
		if (successFn) {
			successFn();
		}
	}, function () {
		if (errorFn) {
			errorFn();
		}
	});
}

function getDynamicUrl(url) {
	var returnUrl = url;
	if ((typeof url === "function")) {
		returnUrl = url();
	}
	return returnUrl;
}

function saveTouchScrollPos()
{
	if ($(".possible-touch").length > 0 && touchScrollDiv != null) {
		touchScrollLeft = $(touchScrollDiv, ".possible-touch").scrollLeft();
	}
	else {
		touchScrollLeft = null;
	}
}

function restoreTouchScrollPos(elementId) {
	if (touchScrollLeft != null && touchScrollDiv != null && $(".possible-touch").length > 0) {
		$(touchScrollDiv, ".possible-touch").scrollLeft(touchScrollLeft);
	}
	touchScrollLeft = null;
}

function setPlayerAutoComplete(fname, lname, pos, nfl) {
	fname = fname || "#Player_FirstName";
	lname = lname || "#Player_LastName";
	pos = pos || "#Player_Position";
	nfl = nfl || "#Player_NFLTeam";
	$.each([fname, lname], function (ix, nameElem) {
		$(nameElem).autocomplete({
			source: function (request, response) {
				var filteredArray = $.map(playerHints, function (item) {
					var response = null;
					var nameParts = [item.firstName, item.lastName, item.firstName + ' ' + item.lastName];
					$.each(nameParts, function (index, elem) {
						if (formatAutoCompName(elem).match("^" + formatAutoCompName(request.term))) {
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
	})
};

function setIsUserTurn(pickInfo) {
	if (isValidPickInfo(pickInfo)) {
		var currUserId = $("*[data-user-id]").attr("data-user-id");
		isUserTurn = pickInfo.uturnid == currUserId;
	}
}

function checkUserTurnDialog() {
	if (draftActive)
	{
		if (!isRefreshPage		//Either user turn is unknown, i.e. not on refreshed page  
		|| isHistoryMode()		//Or on refresh page in historyMode (doesn't get refreshed) 
		|| isUserTurn)			//Or we know is user turn on refreshed page
		{
			tryShowUserTurnDialog();
		}
		//else if open and not user turn, close it
		else if (isUserTurnDialogOpen()) {
			closeUserTurnDialog();
		}
	}
}

function tryShowUserTurnDialog() {
	if ($.cookie("userTurnSettings")) {
		var settings = jQuery.parseJSON($.cookie("userTurnSettings"));
		if (!settings.shown && !settings.neverShowAgain) {
			showUserTurnDialog();
		}
		//else if open and not user turn, close it
		else if (isUserTurnDialogOpen())
		{
			ajaxGetJson("Draft/GetUserTurnPickInfo", function (pickInfo) {
				if (pickInfo && !pickInfo.turn) {
					closeUserTurnDialog();
				}
			});
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
				autoOpen: true,
				buttons: [
							{
								id: "btnUserMakePick",
								text: "Make Pick", click: function () {
									setHideUserTurn("#chkHideUserTurn");
									location.href = baseURL + "Draft/Pick";
									$(this).dialog("close");
								}
							},
							{
								text: "Close", click: function () {
									setHideUserTurn("#chkHideUserTurn");
									$(this).dialog("close");
								}
							},
				],
				open: function () {
					$("#btnUserMakePick").focus();
				}
			});
		});
	}
}

function setLatestUserTurnPickInfo(showFn) {
	if (!isElementInView($(".current-turn")) || isHistoryMode()) {
		ajaxGetJson("Draft/GetUserTurnPickInfo", function (pickInfo) {
			if (pickInfo && pickInfo.turn) {
				setUserTurnCookie(true, false);
				$("#userTurnDialog").attr("title", "Your Turn - Pick #" + pickInfo.num);
				if (pickInfo.hasPrev) {
					$(".ut-last-pick").text("(Last Pick: " + pickInfo.prevName + ")");
				}
				if (showFn) showFn();
			}
			//else if open and not user turn, close it
			else if (isUserTurnDialogOpen()) {
				closeUserTurnDialog();
			}
		});
	}
	else {
		if (isUserTurn) {
			setUserTurnCookie(true, false);
		}
		else if (isUserTurnDialogOpen()) {
			closeUserTurnDialog();
		}
	}
}

function getUserTurnCookie() {
	var settings;
	if ($.cookie("userTurnSettings")) {
		settings = jQuery.parseJSON($.cookie("userTurnSettings"));
	}
	else {
		settings = { shown: false, neverShowAgain: false };
	}
	return settings;
}

function setUserTurnCookie(shown, neverShowAgain) {
	var settings = { shown: shown, neverShowAgain: neverShowAgain };
	$.cookie("userTurnSettings", JSON.stringify(settings), { path: baseURL });
}

function isUserTurnDialogOpen() {
	return $("#userTurnDialog").dialog({autoOpen: false}).dialog("isOpen");
}

function setHideUserTurn(chkId) {
	setUserTurnCookie(getUserTurnCookie().shown, $(chkId).prop('checked'));
}

function closeUserTurnDialog() {
	$("#userTurnDialog").dialog("close");
}

function getDynastySettingsCookie() {
	var settings;
	if ($.cookie("dynastySettings")) {
		settings = jQuery.parseJSON($.cookie("dynastySettings"));
	}
	else {
		settings = {
			disableBrowserAudio: false
		};
	}
	return settings;
}

function setDynastySettingsCookie(settings) {
	var cookieSettings = {};
	cookieSettings.disableBrowserAudio = settings.disableBrowserAudio || false;
	$.cookie("dynastySettings", JSON.stringify(cookieSettings), { path: baseURL });
}

function markInvalidId(userId, selects) {
	selects = selects || $("select");
	if (userId === "") {
		//Find any blank spans
		$.each(selects, function (index, user) {
			if ($(user).val() === "") {
				$(user).addClass("invalid-border");
			}
		});
	}
	else {
		//Find any matching selected options
		var invalidEntries = $("option:selected[value=" + userId + "]", selects).closest("select");
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

function showStaleDraftDialog() {
	showConfirmDialog("The draft has been updated, and the page you're viewing may be stale. <br/><br/>Would you like to refresh this page?",
			"Draft Updated", function () {
				location.reload(true);
			}, null, "Refresh", "Cancel");
}

function refreshCurrentDraftPickPartial() {
	callRefreshPage("Draft/CurrentDraftPickPartial", ".draft-info");
}

function populateWithDraftPickInfo(draftPick, pickInfo, showPosCol) {
	$(".player-nflteam", draftPick).text(pickInfo.team + "-");
	$(".player-pos", draftPick).text(pickInfo.pos);
	$(".player-name", draftPick).text(pickInfo.pname);
	$(draftPick).addClass("filled");
	if (showPosCol == "full") {
		$(draftPick).addClass("show-pos-colors");
	}

	$(".draft-pick-meta", draftPick).addClass("filled");
	if (showPosCol == "header") {
		$(".draft-pick-meta", draftPick).addClass("show-pos-colors");
	}
	var posClass = "";
	if (toBool(pickInfo.combwrte) && pickInfo.pos == "te") {
		posClass = "dp-wrte";
	}
	else {
		posClass = "dp-" + pickInfo.pos.toLowerCase();
	}
	$(draftPick).addClass(posClass);
	$(".draft-pick-meta", draftPick).addClass(posClass);
}

/*		--- Draft Chat */

function bindDraftChatWindow() {
	formatDraftChatText();
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
		startHubConnection(refreshDraftChat, true);
		toggleChatWindowError(false);
	});
	initToggleDraftChatView();
}

function formatDraftChatText(chatText) {
	chatText = chatText || $(".dchat-window .dchat-msg-text");
	$.each($(chatText), function (ix, msg) {
		$(msg).html($(msg).html().autoLink());
	});
}

var siteLinkPattern = /(^|[\s\n]|<[A-Za-z]*\/?>)((((?:https?):\/\/)|(www.))[\-A-Z0-9+\u0026\u2019@#\/%?=()~_|!:,.;]*\.[\-A-Z0-9+\u0026@#\/%=~()_|]{2,})/gi;

(function () {
	var autoLink,
	  slice = [].slice;

	autoLink = function () {
		var callback, k, linkAttributes, option, options, v;
		options = 1 <= arguments.length ? slice.call(arguments, 0) : [];
		if (!(options.length > 0)) {
			return this.replace(siteLinkPattern, function (a, b, c, d, e) {
				if (c.indexOf("http:") >= 0 || c.indexOf("https:") >= 0) {
					return " <a target='_blank' href='" + c + "'>" + c + "</a>";
				}
				else {
					return " <a target='_blank' href='http://" + c + "'>" + c + "</a>";
				}
			});
		}
		option = options[0];
		callback = option["callback"];
		linkAttributes = ((function () {
			var results;
			results = [];
			for (k in option) {
				v = option[k];
				if (k !== 'callback') {
					results.push(" " + k + "='" + v + "'");
				}
			}
			return results;
		})()).join('');
		return this.replace(pattern, function (match, space, url) {
			var link;
			link = (typeof callback === "function" ? callback(url) : void 0) || ("<a href='" + url + "'" + linkAttributes + ">" + url + "</a>");
			return "" + space + link;
		});
	};

	String.prototype['autoLink'] = autoLink;

}).call(this);


function getActiveDchatWindow() {
	return $(".dchat-window").hasClass("hide-yo-wives") ? $(".dchat-window-error") : $(".dchat-window");
}

function initToggleDraftChatView() {
	var dchatWindow = $(".dchat-window");
	var initExpand = toBool($(".dchat-toggle-link", dchatWindow).attr("data-expand"));
	if (initExpand) {
		expandChatWindow(dchatWindow, false, true);
	}
	else {
		collapseChatWindow(dchatWindow, false, true);
	}
}

function toggleDraftChatView(isInit) {
	var isErrorWindow = $(".dchat-window").hasClass("hide-yo-wives");
	var dchatWindow = isErrorWindow ? $(".dchat-window-error") : $(".dchat-window");
	var expanded = toBool($(".dchat-toggle-link", dchatWindow).attr("data-expand"));

	if (expanded) {
		collapseChatWindow(dchatWindow, isErrorWindow, false);
	}
	else {
		expandChatWindow(dchatWindow, isErrorWindow, false);
	}
	if (!isInit) {
		$(".dchat-toggle-link", dchatWindow).attr("data-expand", !expanded);
		setSiteCookieChatExpanded(!expanded);
	}
}

function expandChatWindow(dchatWindow, isErrorWindow, isInit) {
	$(".dchat-toggle-img", dchatWindow).attr("src", contentImagesPath + "collapse.png");
	$(dchatWindow).removeClass("dchat-collapse");
	$(dchatWindow).removeClass("dchat-ease-collapse");
	if (isInit) {
		$(dchatWindow).addClass("dchat-expand");
	}
	else {
		$(dchatWindow).addClass("dchat-ease-expand");
	}
	$(".dchat-prev-content", dchatWindow).addClass("hide-yo-wives");
	if (isErrorWindow) {
		$(".dchat-preview", dchatWindow).removeClass("invalid-border-small");
	}
	$(".dchat-body", dchatWindow).removeClass("hide-yo-wives");
	$(".dchat-footer", dchatWindow).removeClass("hide-yo-wives");
	scrollDraftChatBottom();
}

function collapseChatWindow(dchatWindow, isErrorWindow, isInit) {
	$(".dchat-toggle-img", dchatWindow).attr("src", contentImagesPath + "expand.png");
	$(dchatWindow).removeClass("dchat-expand");
	$(dchatWindow).removeClass("dchat-ease-expand");
	if (isInit) {
		$(dchatWindow).addClass("dchat-collapse");
	}
	else {
		$(dchatWindow).addClass("dchat-ease-collapse");
	}
	$(".dchat-prev-content", dchatWindow).removeClass("hide-yo-wives");
	if (isErrorWindow) {
		$(".dchat-preview", dchatWindow).addClass("invalid-border-small");
	}
	$(".dchat-body", dchatWindow).addClass("hide-yo-wives");
	$(".dchat-footer", dchatWindow).addClass("hide-yo-wives");
	$(".dchat-input", dchatWindow).removeClass("invalid-border-small");
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

function setSiteCookieChatExpanded(chatEnabled) {
	ajaxPost({ ChatEnabled: chatEnabled }, "Site/PostSiteCookieChatEnabled");
}

/*	End Draft Chat */



/* Draft Pick Audio */

function initLastPickAudio() {
	if (!audioKillSwitch && draftActive && !isMobileBrowser() && !(window.location.href.indexOf("/Admin/Input") > 0)
		&& !getDynastySettingsCookie().disableBrowserAudio) {
		ajaxGetJson("Draft/GetLastDraftPickAudio", function (pickAudio) {
			lastPickAudio = pickAudio;
		});
		pickAudioBed = new Audio(baseURL + "Media/NFL Draft Tone.mp3");
		tickingClockAudio = new Audio(baseURL + "Media/tickingclock.mp3");
	}
	else {
		lastPickAudio = null;
	}
}

function stopTickingClockAudio() {
	if (tickingClockAudio) {
		tickingClockAudio.pause();
		tickingClockAudio.currentTime = 0;
		isTickingClockPlaying = false;
	}
}

function getLastPickAndPlayAudio(pickInfo, origIsUserTurn) {
	if (!audioKillSwitch && draftActive && !isMobileBrowser() && !(window.location.href.indexOf("/Admin/Input") > 0)
		&& (!(window.location.href.indexOf("/Draft/Pick") > 0) || !origIsUserTurn)
		&& !getDynastySettingsCookie().disableBrowserAudio) {
		//Only make audio call if userid is audio eligible
		var currUserId = $("*[data-user-id]").attr("data-user-id");
		if (!pickInfo || !pickInfo.auduids || pickInfo.auduids.indexOf(parseInt(currUserId)) >= 0) {
			ajaxGetJson("Draft/GetLastDraftPickAudio", function (pickAudio) {
				if (pickAudio && pickAudio.playerId) {
					if (!lastPickAudio || lastPickAudio.playerId != pickAudio.playerId) {
						lastPickAudio = pickAudio;
						playPickAudio();
					}
				}
			});
		}
		else if (pickInfo) {
			lastPickAudio = null;
		}
	}
}

function playPickAudio() {
	if (!audioKillSwitch) {
		//Check if has access & this tab's call is only one to succeed, else don't play audio on this tab
		if (toBool(lastPickAudio.access) && toBool(lastPickAudio.success)) {
			var pickText = lastPickAudio.name;
			if (lastPickAudio.pos) {
				pickText = pickText + ",%20" + lastPickAudio.pos;
			}
			if (lastPickAudio.team) {
				pickText = pickText + ",%20" + lastPickAudio.team;
			}
			if (!textToVoiceKillSwitch && lastPickAudio.url) {
				var audioUrl = lastPickAudio.url.replaceAll("<<audiotext>>", pickText);
				var pickPlayerAudio = new Audio(audioUrl);
				pickPlayerAudio.play();
				pickPlayerAudio.addEventListener('ended', function (e) {
					checkFinalDraftPick();
				});
				setTimeout(function () {
					pickAudioBed.play();
					updateLastDraftPickAudioCount(lastPickAudio.apiCode);
				}, 125);
			}
			else {
				pickAudioBed.play();
			}
		}
		else if (toBool(lastPickAudio.access)) {
			var text = "Audio already played on another browser/tab.";
			if (lastPickAudio.error) {
				text += " (Error: " + lastPickAudio.error + ")";
			}
			console.log(text);
		}
	}
}

function updateLastDraftPickAudioCount(apiCode) {
	if (apiCode) {
		ajaxPost({ apiCode: apiCode }, "Draft/UpdateLastDraftPickAudioCount", function () { });
	}
}

function checkFinalDraftPick() {
	if (toBool(lastPickAudio.final)) {
		var audioUrl = lastPickAudio.url.replaceAll("<<audiotext>>", "The%20draft%20has%20completed,%20have%20a%20great%20season");
		var pickPlayerAudio = new Audio(audioUrl);
		setTimeout(function () {
			pickPlayerAudio.play();
		}, 600);
	}
}

function bindTestAudio() {
	$(".test-audio-btn").click(function () {
		var testAudioBed = new Audio(baseURL + "Media/NFL Draft Tone.mp3");
		addWaitCursor();
		try {
			testAudioBed.play();
		}
		finally {
			setTimeout(removeWaitCursor, 2000);
		}
	});
}

function toggleTestAudio() {
	if (isMobileBrowser()) {
		$(".test-audio-enabled").hide();
	}
	else {
		$(".test-audio-disabled").hide();
	}
	$(".test-audio").removeClass("hide-yo-wives");
}

/*  End Draft Pick Audio */



/* Helper functions */

function ajaxGetJson(url, successFn, errorFn) {
	$.get(baseURL + url, {}, successFn, "JSON").fail(errorFn);
};

function ajaxGet(url, successFn, errorFn) {
	$.get(baseURL + url, successFn).fail(errorFn);
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

function ajaxGetReplace(url, elementId, successFn, errorFn) {
//For refreshed page functnlty use "callRefreshPage" instead
	ajaxGet(url, function (response) {
		replaceWith(elementId, response);
		if (successFn) {
			successFn();
		}
	}, errorFn);
}

function ajaxPostReplace(model, url, elementId, successFn, errorFn, dataType, makeSync) {
//Going to default to refreshed page functnlty (i.e. setPickTimer, restoreTouchScroll)
	saveTouchScrollPos();
	ajaxPost(model, url, function (response) {
		replaceWith(elementId, response);
		restoreTouchScrollPos();
		setPickTimer(false);
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

function toBool(val) {
	return val == true || (val != null && val != undefined && val.toLowerCase() == "true");
}

function htmlEncode(val) {
	return $('<div/>').text(val).html();
}

function htmlDecode(val) {
	return $('<div/>').html(val).text();
}

function formatAutoCompName(name) {
	return name.replace(/\.|'|-/g, '').escapeRegEx().trim().toLowerCase();
}

function formatName(name) {
	return name.replace(/\.|'|-|\*|\(|\)|\"|,|\d|	| /g, "").trim().toLowerCase();
}

function isTouchDevice() {
	return (/Android|iPad|iPhone|iPod|KFAPWI|Tablet|Touch/i.test(navigator.userAgent));
}

function isMobileBrowser() {
	return (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent));
}

function isBrowserIE() {
	return (/Trident|MSIE |Edge/i.test(window.navigator.userAgent));
}

function addWaitCursor() {
	$('body').css("cursor", "");
	$('body').addClass('wait');
}

function removeWaitCursor() {
	$('body').css("cursor", "");
	$('body').removeClass('wait');
}

function addQSValue(urlString, qsNameValue) {
	if (urlString.indexOf("?") >= 0) {
		return urlString + "&" + qsNameValue;
	}
	return urlString + "?" + qsNameValue;
}

//Already exists as javascript "toggle"; refactor
function toggleDisplay(element, condition) {
	if (condition) {
		$(element).show();
	}
	else {
		$(element).hide();
	}
}

function getRandom(min, max) {
	return Math.floor(Math.random() * (max - min)) + min;
}

function getRandomInt() {
	return getRandom(0, 100000000);
}

function hasAttr(elem, attribute) {
	var attr = $(elem).attr(attribute);
	return (typeof attr !== typeof undefined && attr !== false);
}

function isAdmin(adminMode) {
	return (adminMode && adminMode.trim().toLowerCase() == "admin");
}

function removeHeaderFreeze() {
	$("body").removeClass("header-freeze");
}

function changeViewport(minScale, maxScale, initScale, width) {
	width = width || "device-width";
	initScale = initScale || "1";
	if (minScale != 0) {
		minScale = minScale || "1.0";
	}
	maxScale = maxScale || "1.2";

	$('head').append('<meta name="viewport" content="width=' + width + 
		', initial-scale=' + initScale + 
		', minimum-scale=' + minScale +
		', maximum-scale=' + maxScale + '">');
}

$.fn.permanentlyHide = function () {
	$(this).addClass("hide-yo-wives");
	return this;
}

function isNullOrWhitespace(str) {
	return !(typeof str != undefined && str != null && str.trim().length > 0);
}


/* Dialogs */

function showPleaseWait(title, timer) {
	addWaitCursor();
	pleaseWaitNeeded = true;
	timer = timer || pleaseWaitTimer;
	setTimeout(function () {
		if (pleaseWaitNeeded) {
			showLoadingDialog(title);
		}
	}, timer);
}

function showConfirmDialog(dialogText, title, okFn, cancelFn, okText, cancelText) {
	title = title || "Confirmation";
	okFn = okFn || function () { $(this).dialog("close"); };
	cancelFn = cancelFn || function () { $(this).dialog("close"); };
	okText = okText || "OK";
	cancelText = cancelText || "Cancel";
	var confirmDialog = '<div class="center hide-yo-kids" title="' + title + '"><p>' + dialogText + '</p></div>';

	$(confirmDialog).dialog({
		resizable: false,
		height: 'auto',
		width: '295px',
		modal: true,
		buttons: [
					{
						text: okText, click: okFn
					},
					{
						text: cancelText, click: cancelFn
					},
		]
	});
}

function showAlertDialog(dialogText, title, okFn) {
	title = title || "Alert";
	okFn = okFn || function () { $(this).dialog("close"); };
	var dialog = '<div class="center hide-yo-kids" title="' + title + '"><p>' + dialogText + '</p></div>';

	$(dialog).dialog({
		resizable: false,
		height: 'auto',
		width: '295px',
		modal: true,
		buttons: [
					{
						text: "OK", click: okFn
					},
		]
	});
}

function showMessageDialog(dialogText, title) {
	title = title || "Information";
	var dialog = '<div class="center hide-yo-kids" title="' + title + '"><p>' + dialogText + '</p></div>';

	$(dialog).dialog({
		resizable: false,
		height: 'auto',
		width: '295px',
		modal: true
	});
}

function showLoadingDialog(title) {
	title = title || "Loading";
	var dialog = '<div class="center hide-yo-kids" title="' + title +
		'"><p>Please Wait... <img style="vertical-align: top;" src="' + baseURL + 'Content/images/ajax-loader.gif"/></p></div>';

	$(dialog).dialog({
		resizable: false,
		height: 'auto',
		width: '215px',
		modal: true
	});
}

function closeAllDialogs() {
	$(".ui-dialog-content").dialog("close");
}

function closePleaseWait() {
	pleaseWaitNeeded = false;
	removeWaitCursor();
	closeAllDialogs();
}

/* End Dialogs */

/*  End Helper Function */



/* Plugins */

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

$.fn.goTo = function () {
	$('html, body').animate({
		scrollTop: ($(this).offset().top - ($(window).height() - $(this).outerHeight(true)) / 2) + 'px'
	}, 'fast');
	return this;
}

$.fn.where = function (prop, val) {
	return $.grep($(this), function (e) { return e[prop] == val; });
}

if (!String.prototype.startsWith) {
	String.prototype.startsWith = function (searchString, position) {
		position = position || 0;
		return this.indexOf(searchString, position) === position;
	};
}

if (!String.prototype.endsWith) {
	String.prototype.endsWith = function (searchString, position) {
		var subjectString = this.toString();
		if (position === undefined || position > subjectString.length) {
			position = subjectString.length;
		}
		position -= searchString.length;
		var lastIndex = subjectString.indexOf(searchString, position);
		return lastIndex !== -1 && lastIndex === position;
	};
}

String.prototype.removeTrailing = function (substr) {
	var ix = this.lastIndexOf(substr);
	var resp = this + "";
	if (ix >= 0) {
		resp = this.substring(0, ix);
		if (this.length - substr.length != resp.length) {
			resp = this + "";
		}
	}
	return resp;
}

String.prototype.removeAll = function (substr) {
	return this.replaceAll(substr, "")
}

String.prototype.replaceAll = function (substr, newstr) {
	var rx = new RegExp(substr, "g");
	return this.replace(rx, newstr);
}

String.prototype.escapeRegEx = function () {
	return this.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
};

/* End Plugins */