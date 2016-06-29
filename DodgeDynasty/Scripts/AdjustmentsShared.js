function bindDraftsRanksMoreLinks() {
	$(".dr-more-link").unbind("click");
	var links = $(".dr-more-link");
	$.each(links, function (index, link) {
		$(link).click(function (e) {
			e.preventDefault();
			var paMoreDialog = $(link).siblings(".dr-more-dialog");
			var dialog = '<div class="center hide-yo-kids" title="Drafts/Ranks Found">' + $(paMoreDialog).html() + '</div>';
			$(dialog).dialog({
				resizable: false,
				height: 'auto',
				width: '295px',
				modal: true,
				buttons: [
							{
								text: "OK", click: function () {
									$(this).dialog("close");
								}
							},
				]
			});
		});
	});
}