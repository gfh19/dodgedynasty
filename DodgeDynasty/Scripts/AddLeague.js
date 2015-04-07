function initAddLeague() {
	//bindOwnerLinks();
	$('html').keydown(preventBackspaceNav);
	$('html').keypress(preventBackspaceNav);
}
