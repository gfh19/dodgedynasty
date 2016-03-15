
function updateAddEditLeagueModel(addEditLeagueModel) {
	ajaxPost(addEditLeagueModel, adminMode + "/EditLeague", function (data) {
		location.href = baseURL + adminMode + "/ManageLeagues";
	}, null, null, true);
}
