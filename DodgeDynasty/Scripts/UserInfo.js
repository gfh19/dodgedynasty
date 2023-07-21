function initUserInfo() {
	if (!userEditAudioKillSwitch) {
		toggleTestAudio();
	}
	bindColorSelects();
	bindSubmitUserInfo();
	bindUserNameSelect();
	bindHideUserTurnCheckbox();
	bindTestAudio();
	toggleDisableBrowserAudio();

	$("#chkMIPushNotifications").click(enablePushNotifications);

	$("#btnSimulateTurn").click(function (e) {
		e.preventDefault();
		broadcastNotification();
		//if ('Notification' in window) {
		//	if (window.Notification.permission === 'granted') {
		//		new window.Notification('Time is over!');
		//	}
		//}
	});
	$("#btnUnsubscribeNotif").click(function (e) {
		e.preventDefault();
		unsubscribe();
alert('Unsubscribed.');
		//if ('Notification' in window) {
		//	if (window.Notification.permission === 'granted') {
		//		new window.Notification('Time is over!');
		//	}
		//}
	});
}

async function enablePushNotifications(e) {
	subscribe();
	if ('Notification' in window) {
		window.Notification.requestPermission().then((permission) => {
			navigator.serviceWorker.register('/sw.js', { scope: '/' }).then((reg) => {
				window.myRegistration = reg;
				window.myRegistration.showNotification("Hello World", {
					body: "My first notification on iOS",
				}).then(() => {
					alert('much success!');
				});
			});
		});
		/*
		const result = window.Notification.requestPermission(); 
		if (result === "granted") {
			// You must use the service worker notification to show the notification
			// Using new Notification("Hello World", { body: "My first notification on iOS"}) does not work on iOS
			// despite working on other platforms
			await window.myRegistration.showNotification("Hello World", {
				body: "My first notification on iOS",
			});
		}
		*/
	}
	else if (typeof window.Notification !== undefined) {
		//Safari requires promise/doesn't return promise
		window.Notification.requestPermission((permission) => {
			alert('iOS Permission ' + window.Notification.permission + '!');
//Never used, can delete next checkin
		});
	}
	else {
		alert("Notification NOT found!");
	}
}

function bindColorSelects() {
	var selects = $(".mi-color-select");
	$.each(selects, function (index, select) {
		bindColorSelect(select);
	});
}

function bindColorSelect(select) {
	$(select).change(function (e) {
		var loEntry = $(select).closest(".mi-selected-color");
		$(loEntry).removeClass();
		$(loEntry).addClass("mi-selected-color");
		$(loEntry).addClass($(select).val());
	});
}

function bindSubmitUserInfo() {
	$(".submit-info").click(function (e) {
		e.preventDefault();
		$(".submit-info").first().focus();
		resetValidations();
		var userInfoModel = getUserInfoModel();
		var userInfoFormValid = $('#userInfoForm').valid()
		var userInfoValid = validateUserInfoModel(userInfoModel);
		if (userInfoFormValid && userInfoValid) {
			setDisableBrowserAudio();
			setHideUserTurn("#chkMIHideUserTurn");
			updateUserInfoModel(userInfoModel);
		}
	});
}

function resetValidations() {
	$(".blank-team-msg").addClass("hide-yo-wives");
	$(".invalid-border").removeClass("invalid-border");
}

function getUserInfoModel() {
	var userInfoModel = $("#userInfoForm").serializeObject();
	var ownerLeagues = new Array();
	$.each($(".mi-league-entry"), function (index, ownerLeague) {
		var lo = {};
		lo.LeagueId = $(ownerLeague).attr("data-league-id");
		lo.TeamName = $(".mi-team-input", ownerLeague).val();
		lo.CssClass = $(".mi-color-select option:selected", ownerLeague).val();
		lo.IsActive = $(".mi-active-chkbx", ownerLeague).prop('checked');
		lo.AnnouncePrevPick = $(".mi-audio-chkbx", ownerLeague).prop('checked');
		//Set hidden field due to OR condition on audio chkbox display
		$(".mi-prevpick-hidden", ownerLeague).val(lo.AnnouncePrevPick);

		ownerLeagues.push(lo);
	});

	userInfoModel.OwnerLeagues = ownerLeagues;
	return userInfoModel;
}

function validateUserInfoModel(userInfoModel) {
	var isValid = true;

	var blankTeamNames = $(".mi-team-input").filter(function () {
		return $.trim($(this).val()) == "";
	});
	if (blankTeamNames.length > 0) {
		isValid = false;
		$(".blank-team-msg").removeClass("hide-yo-wives");
		$(blankTeamNames).addClass("invalid-border");
	}
	return isValid;
}

function updateUserInfoModel(userInfoModel) {
	$("#userInfoForm").submit();
}

function bindUserNameSelect() {
	$(".mi-user-name").change(function () {
		var userName = $(".mi-user-name").val();
		ajaxGetReplace("Admin/UserInfoPartial?userName=" + userName, "#userInfo", function () {
			$(".mi-user-name").focus();
		});
	});
}

function bindHideUserTurnCheckbox() {
	$("#chkMIHideUserTurn").prop('checked', getUserTurnCookie().neverShowAgain);
}

function toggleDisableBrowserAudio() {
	var settings = getDynastySettingsCookie();
	$("#disable-browser-audio").prop('checked', settings.disableBrowserAudio);
}

function setDisableBrowserAudio() {
	setDynastySettingsCookie({ disableBrowserAudio: $("#disable-browser-audio").prop('checked') });
}