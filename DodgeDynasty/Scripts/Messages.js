var _msgDialogMinWidth = 280;
var _msgDialogMaxWidth = 800;
var _msgTitleMaxWidth = 750;
var _msgTextAreaMaxWidth = 750;

$(function () {
	$("#tabs").tabs();
	aloha.dom.query('.editable', document).forEach(aloha);
	bindAddMessageDialog();
	bindRteButtons();
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

function bindRteButtons() {
	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
	$('.action-italic').on('click', aloha.ui.command(aloha.ui.commands.italic));
	$('.action-underline').on('click', aloha.ui.command(aloha.ui.commands.underline));
	$('.action-unformat').on('click', aloha.ui.command(aloha.ui.commands.unformat));
	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
	$('.action-bold').on('click', aloha.ui.command(aloha.ui.commands.bold));
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
							$("#messageForm").submit();
							$(this).dialog("close");
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
