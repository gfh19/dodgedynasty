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
	toggleSubscribeNotifications();
	bindPushNotifications();
}

function bindPushNotifications() {
    if (isiPhoneiPad()) {
        $(".push-ios-msg").removeClass("hide-yo-kids");
    }

    $("#chkMIPushNotifications").click(function(e) {
        if ($("#chkMIPushNotifications").is(':checked')) {
            enablePushNotifications();
        }
        else {
            unsubscribe();
        }
    });

    $("#btnSimulateTurn").click(function(e) {
        e.preventDefault();
        simulateNotification();
    });
}

async function enablePushNotifications() {
	if ('Notification' in window && !pushNotificationsKillSwitch) {
		window.Notification.requestPermission().then((permission) => {
			if (permission === 'granted') {
				subscribe();
			}
			else {
				setSubscribeStatus(permission);
			}
		});
	}
}

async function simulateNotification() {
	if ('Notification' in window && !pushNotificationsKillSwitch) {
		window.Notification.requestPermission().then((permission) => {
			if (permission === 'granted') {
				navigator.serviceWorker.ready.then((reg) => {
					reg.pushManager.getSubscription().then((subscription) => {
						if (subscription) {
							fetch('/notification/simulate', {
								method: 'GET',
								headers: {
									'content-type': 'application/json',
								},
							}).catch((e) => {
								setSubscribeStatus("Error");
							});
						}
						else {
							setSubscribeStatus("No subscription");
						}
					});
				});
			}
			else {
				setSubscribeStatus(permission);
			}
		});
	}
}

/*  Option 1  */
/*
async function checkServiceWorkerSubscribed() {
	navigator.serviceWorker.ready.then((reg) => {
		reg.pushManager.getSubscription().then((sub) => {
			swSubscription = sub;
		});
	});
}
*/

/*  Option 2  */
async function checkServiceWorkerSubscribed() {
	let reg = await navigator.serviceWorker.ready;
	let sub = await reg.pushManager.getSubscription();
	return sub;
}

async function toggleSubscribeNotifications() {
	const sub = await checkServiceWorkerSubscribed();
	const isSubscribed = sub != null
	$("#chkMIPushNotifications").prop('checked', isSubscribed);
	setSubscribeStatus(isSubscribed ? "Subscribed" : "Unsubscribed");
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