function initUserInfo() {
	bindColorSelects();
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