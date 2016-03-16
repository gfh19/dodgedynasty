﻿var adminMode = null;

function initInput() {
	setPlayerAutoComplete();
	bindInputPrevious();
	bindInputNext();
	bindInputDelete();
}

function bindInputPrevious() {
	$("#inputPrevious").click(function () {
		ajaxPostReplace({ draftPickId: $("#Player_DraftPickId").val() }, adminMode + "/InputPrevious", '#pickInfo');
	});
}

function bindInputNext() {
	$("#inputNext").click(function () {
		ajaxPostReplace({ draftPickId: $("#Player_DraftPickId").val() }, adminMode + "/InputNext", '#pickInfo');
	});
}

function bindInputDelete() {
	$("#inputDelete").click(function (e) {
		e.preventDefault();
		$("#confirmDelete").dialog({
			resizable: false,
			height: 'auto',
			width: '250px',
			modal: true,
			buttons: [
					{
						text: "OK",
						click: function () {
							addWaitCursor();
							ajaxPostReplace({ draftPickId: $("#Player_DraftPickId").val() }, adminMode + "/InputDelete", '#pickInfo',
								removeWaitCursor, removeWaitCursor);
							$(this).dialog("close");
						}
					},
					{ text: "Cancel", click: function () { $(this).dialog("close"); } },
				]
		});
		return false;
	});
}