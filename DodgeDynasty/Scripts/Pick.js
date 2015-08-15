﻿var initPickCalled = false;

function initPick() {
	if (!initPickCalled) {
		initPickCalled = true;
		setPickTimer(true);
		setPlayerAutoComplete();
		var initTimer = 0;
		if (hasModelErrors === true) {
			initTimer = 30000;
		}
		if (!isUserTurn) {
			setTimeout(function () {
				callRefreshUserPickWithPickTimer();
				hasModelErrors = false;
			},
			initTimer);
			setNewPickUserTurnCookie();
		}
	}
};

function pageBroadcastDraftHandler() {
	getPickInfo();
}

function refreshUserPickWithPickTimer() {
	setTimeout(function () {
		callRefreshUserPickWithPickTimer();
	},
	refreshTimer);
};

function getPickInfo() {
	ajaxGet("Draft/PickPartial", function (response) {
		replaceWith('#pickInfo', response);
		setPlayerAutoComplete();
		setPickTimer(false);
	});
};

function callRefreshUserPickWithPickTimer() {
	if (!isHistoryMode()) {
		if (draftActive && !isUserTurn) {
			getPickInfo();
			refreshUserPickWithPickTimer();
		}
	}
};

function setNewPickUserTurnCookie() {
	var settings;
	if ($.cookie("userTurnSettings")) {
		settings = jQuery.parseJSON($.cookie("userTurnSettings"));
	}
	else {
		settings = { neverShowAgain: false };
	}
	setUserTurnCookie(false, settings.neverShowAgain);
}