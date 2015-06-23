$(function () {
	$("#tabs").tabs();
	aloha.dom.query('.editable', document).forEach(aloha);
	bindAddMessageDialog();
});

function bindAddMessageDialog() {
	$(".msg-add-link").click(function (e) {
		e.preventDefault();
		showAddMessageDialog();
	});
}

function showAddMessageDialog() {
	var dialogWidth = $(window).width() - 30;
	if (dialogWidth < 280) { dialogWidth = 280; }
	if (dialogWidth > 1024) { dialogWidth = 1024; }
	
	$("#addMessageDialog").dialog({
		resizable: true,
		height: 'auto',
		width: dialogWidth + 'px',
		modal: true,
		buttons: [
					{ text: "Make Pick", click: function () { location.href = baseURL + "Draft/Pick"; $(this).dialog("close"); } },
					{ text: "Close", click: function () { $(this).dialog("close"); } },
		]
	});
}
