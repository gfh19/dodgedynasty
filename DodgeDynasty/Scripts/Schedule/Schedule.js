function initSchedule() {
	bindSubmitSchedule();
}

function bindSubmitSchedule() {
	$(".submit-schedule").click(function (e) {
		e.preventDefault();
		showPleaseWait();
		$("#scheduleForm").submit();
	});
}
