var _msgDialogMinWidth = 280;
var _msgDialogMaxWidth = 800;
var _msgTitleMaxWidth = 750;
var _msgTextAreaMaxWidth = 750;

$(function () {
	$("#tabs").tabs();
	bindAddMessageDialog();
	bindDraftChatDisplayLinks();
	displayLineBreaks();
	//aloha.dom.query('.editable', document).forEach(aloha);
	//bindRteButtons();
});

function bindAddMessageDialog() {
	$(".msg-add-link").click(function (e) {
		e.preventDefault();
		showAddMessageDialog();
	});
	$(".add-msg-league").val(0);
	$(".add-msg-title").val("");
	$(".add-msg-text").text("");
}

function showAddMessageDialog() {
	var dialogWidth = $(window).width() - 30;
	dialogWidth = (dialogWidth < _msgDialogMinWidth) ? _msgDialogMinWidth : dialogWidth;
	dialogWidth = (dialogWidth > _msgDialogMaxWidth) ? _msgDialogMaxWidth: dialogWidth;

	$("#addMessageDialog").dialog({
		resizable: true,
		height: 'auto',
		width: dialogWidth + 'px',
		modal: true,
		buttons: [
					{
						text: "Submit", click: function () {
							if ($("#messageForm").valid()) {
								$("#messageForm").submit();
								$(this).dialog("close");
							}
						}
					},
					{
						text: "Cancel", click: function () {
							$(this).dialog("close");
						}
					},
		]
	});

	var contentWidth = $("#addMessageDialog").width();
	$(".add-msg-title").outerWidth(contentWidth > _msgTitleMaxWidth ? _msgTitleMaxWidth : contentWidth);
	$(".add-msg-text").outerWidth(contentWidth > _msgTextAreaMaxWidth ? _msgTextAreaMaxWidth : contentWidth);
	$(".add-msg-title").focus();
}

function bindDraftChatDisplayLinks() {
	$(".draft-chat-link").click(function (e) {
		e.preventDefault();
		var draftChatSection = $(e.toElement).closest('.draft-chat-section');
		var flip = $(".draft-chat-flip", draftChatSection);
		var expandVal = $(flip).attr("data-chat-expand");
		if (expandVal == "collapse") {
			$(flip).attr("data-chat-expand", "expand");
			$(".draft-chat-entries", draftChatSection).removeClass("hide-yo-wives");
			$(flip).text("collapse");
		}
		else {
			$(flip).attr("data-chat-expand", "collapse");
			$(".draft-chat-entries", draftChatSection).addClass("hide-yo-wives");
			$(flip).text("expand");
		}
	});
}

function displayLineBreaks() {
	$.each($(".message-text"), function (ix, msg) {
		$(msg).html($(msg).html().replace(/\n/g, '<br/>'));
	})
}

//function bindRteButtons() {
//	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
//	$('.action-italic').on('click', aloha.ui.command(aloha.ui.commands.italic));
//	$('.action-underline').on('click', aloha.ui.command(aloha.ui.commands.underline));
//	$('.action-unformat').on('click', aloha.ui.command(aloha.ui.commands.unformat));
//	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
//	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
//	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
//}
